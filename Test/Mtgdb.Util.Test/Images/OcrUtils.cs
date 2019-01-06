﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Test;
using NUnit.Framework;
using Tesseract;

namespace Mtgdb.Util
{
	[TestFixture]
	public class OcrUtils : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			string tessdataPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "tools", "tessdata");

			_engine = new TesseractEngine(
				tessdataPath,
				"eng",
				EngineMode.CubeOnly);

			_distance = new DamerauLevenshteinDistance();
		}

		private void setup()
		{
			if (_isSetup)
				return;

			LoadCards();
			ImgRepo.LoadFiles();
			ImgRepo.LoadZoom();

			_isSetup = true;
		}

		[Test]
		public void DetectArtist()
		{
			setup();

			string[] setCodes =
			{
				"AKH",
				"AER",
				"BFZ",
				"C14",
				"C15",
				"C16",
				"CN2",
				"CP1",
				"CP2",
				"CP3",
				"DD3_DVD",
				"DD3_EVG",
				"DD3_GVL",
				"DD3_JVC",
				"DDO",
				"DDP",
				"DDQ",
				"DDQ",
				"DDS",
				"DTK",
				"E01",
				"EMA",
				"EMN",
				"HOU",
				"KLD",
				"MPS",
				"MM2",
				"MM3",
				"OGW",
				"ORI",
				"PCA",
				"SOI",
				"W17"
			};

			Rectangle[] rectangles =
			{
				new Rectangle(162, 988, 280, 23),
				//new Rectangle(114, 954, 260, 28)
			};

			Func<Bitmap, BmpProcessor>[][] preFilters =
			{
				new Func<Bitmap, BmpProcessor>[] { }
			};

			Func<Bitmap, BmpProcessor>[][] postFilters =
			{
				new Func<Bitmap, BmpProcessor>[]
				{
					scaled => new BwFilter(scaled, 0.55f),
					scaled => new BwFilter(scaled, 0.6f)
				}
			};

			var result = new StringBuilder();

			foreach (string setCode in setCodes)
			{
				var cards = Repo.SetsByCode[setCode].Cards
					.OrderBy(c => c.ImageName).ToList();

				_artists = cards
					.Where(c => c.Artist != null)
					.Select(c => c.Artist)
					.Distinct(Str.Comparer)
					.ToArray();

				_artistDistance = new float[_artists.Length];

				string[] texts = new string[rectangles.Length];

				for (int m = 0; m < cards.Count; m++)
				{
					var card = cards[m];
					var model = card.ImageModel(Ui);
					var path = model.ImageFile.FullPath;

					Stopwatch sw = new Stopwatch();

					for (int r = 0; r < rectangles.Length; r++)
					{
						string text = ocr(path, rectangles[r], preFilters[r], postFilters[r]);

						sw.Start();

						text = text ?? string.Empty;

						text = new Regex(@"\s+").Replace(text, " ");
						texts[r] = text;
					}

					for (int a = 0; a < _artists.Length; a++)
					{
						_artistDistance[a] = texts.Min(text => _distance.GetDistances(_artists[a], text)).Distance;
					}

					int artistIndex = Enumerable.Range(0, _artists.Length)
						.AtMin(a => _artistDistance[a])
						.Find();

					string detectedArtist = _artists[artistIndex];

					sw.Stop();
					long elapsedMatching = sw.ElapsedMilliseconds;

					if (!Str.Equals(card.Artist, detectedArtist))
					{
						string message = setCode + "\t" + card.ImageName + "\t" + card.Artist + "\t" + detectedArtist + "\t" + model.ImageFile.FullPath;
						Log.Debug(message);
						result.AppendLine(message);
					}
				}
			}
		}

		[TestCase("UMA", "D:\\Distrib\\games\\mtg\\Gatherer.Original\\UMA")]
		public void RenameSetImages(string setCode, string unnamedImagesDirectory)
		{
			setup();

			var fileNames =
				Directory.EnumerateFiles(unnamedImagesDirectory, "*.jpg", SearchOption.TopDirectoryOnly).Concat(
					Directory.EnumerateFiles(unnamedImagesDirectory, "*.png", SearchOption.TopDirectoryOnly));

			var cardsByName = Repo.SetsByCode[setCode].CardsByName;

			var cardNames = cardsByName.Keys.ToArray();
			var distances = new float[cardNames.Length];

			foreach (var fileName in fileNames)
			{
				var extension = Path.GetExtension(fileName);
				var directory = Path.GetDirectoryName(fileName);

				var name = ocr(fileName, new Rectangle(20, 20, 170, 17));

				for (int i = 0; i < cardNames.Length; i++)
					distances[i] = _distance.GetPrefixDistance(name, cardNames[i]);

				var matchedNameIndex = Enumerable.Range(0, cardNames.Length)
					.AtMin(i => distances[i])
					.Find();

				var matchedName = cardNames[matchedNameIndex];
				var imageName = cardsByName[matchedName][0].ImageName;
				string renamed = Path.Combine(directory, imageName + extension);

				try
				{
					File.Move(fileName, renamed);
				}
				catch (IOException)
				{
				}
			}
		}

		[TestCase("D:\\Distrib\\games\\mtg\\Gatherer.Original\\GRN.png", "GRN")]
		public void RenameImages(string unnamedImagesDirectory, string setCode)
		{
			var fileNames =
				Directory.EnumerateFiles(unnamedImagesDirectory, "*.jpg", SearchOption.TopDirectoryOnly).Concat(
					Directory.EnumerateFiles(unnamedImagesDirectory, "*.png", SearchOption.TopDirectoryOnly));

			var repo = new CardRepository();
			repo.FilterSetCode = code => code == setCode;

			repo.LoadFile();
			repo.Load();

			var cardNames = repo.Cards.Select(c => c.NameEn).Distinct().ToArray();
			float[] distances = new float[cardNames.Length];

			foreach (var fileName in fileNames)
			{
				var extension = Path.GetExtension(fileName);
				var directory = Path.GetDirectoryName(fileName);

				var name = ocr(fileName, new Rectangle(20, 20, 170, 17));

				for (int i = 0; i < cardNames.Length; i++)
					distances[i] = _distance.GetPrefixDistance(name, cardNames[i]);

				var mostSimilarName = cardNames[Enumerable.Range(0, cardNames.Length).AtMin(i => distances[i]).Find()];

				string renamed = Path.Combine(directory, mostSimilarName + extension);

				if (Str.Equals(renamed, fileName))
					continue;

				while (File.Exists(renamed))
					renamed = Path.Combine(directory, Path.GetFileNameWithoutExtension(renamed) + '-' + extension);

				try
				{
					File.Move(fileName, renamed);
				}
				catch (IOException)
				{
				}
			}
		}

		private string ocr(
			string bitmapPath,
			Rectangle rect,
			IList<Func<Bitmap, BmpProcessor>> preScaleFilters = null,
			IList<Func<Bitmap, BmpProcessor>> postScaleFilters = null)
		{
			preScaleFilters = preScaleFilters ?? new List<Func<Bitmap, BmpProcessor>> { bmp => null };
			postScaleFilters = postScaleFilters ?? new List<Func<Bitmap, BmpProcessor>> { bmp => null };

			Bitmap textArea;

			using (var bitmap = new Bitmap(bitmapPath))
				textArea = getPart(bitmap, rect);

			using (textArea)
			{
				foreach (var preScaleFilter in preScaleFilters)
					preScaleFilter(textArea)?.Execute();

				//textArea.Save("D:\\temp\\img\\text.png");

				float[] factors =
				{
					//2.3f,
					//2.4f,
					//2.5f,
					2.6f,
					//2.7f
				};

				string[] texts = new string[factors.Length * postScaleFilters.Count];
				float[] textConfidences = new float[texts.Length];

				var sw = new Stopwatch();

				for (int f = 0; f < factors.Length; f++)
				{
					long elapsedPreprocess = sw.ElapsedMilliseconds;

					sw.Start();

					var scaledSize = rect.Size.MultiplyBy(factors[f]).Round();
					for (int p = 0; p < postScaleFilters.Count; p++)
					{
						int i = f * postScaleFilters.Count + p;

						var scaled = textArea.FitIn(scaledSize);
						postScaleFilters[p](scaled)?.Execute();

						//scaled.Save("D:\\temp\\img\\bw.png");

						using (var page = _engine.Process(scaled, PageSegMode.SingleLine))
						{
							texts[i] = page.GetText();
							textConfidences[i] = page.GetMeanConfidence();
						}
					}

					sw.Stop();

					long elapsedOcr = sw.ElapsedMilliseconds;
				}

				int textIndex = Enumerable.Range(0, textConfidences.Length)
					.AtMax(i => texts[i].Trim().Count(char.IsLetter) > 2)
					.ThenAtMax(i => textConfidences[i])
					.Find();

				return texts[textIndex].Trim();
			}
		}

		private static Bitmap getPart(Bitmap bitmap, Rectangle rect)
		{
			var textArea = new Bitmap(rect.Width, rect.Height);
			var g = Graphics.FromImage(textArea);
			g.DrawImage(bitmap, new Rectangle(Point.Empty, rect.Size), rect, GraphicsUnit.Pixel);
			return textArea;
		}

		private TesseractEngine _engine;
		private string[] _artists;
		private float[] _artistDistance;
		private DamerauLevenshteinDistance _distance;
		private bool _isSetup;
	}
}