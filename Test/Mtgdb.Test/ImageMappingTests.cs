﻿using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class ImageMappingTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadCards();

			ImgRepo.LoadFiles(Sequence.From("dev", "xlhq"));
			ImgRepo.LoadSmall();
			ImgRepo.LoadZoom();
		}

		[Test, Order(1)]
		public void No_cards_without_image()
		{
			foreach (var set in Repo.SetsByCode)
				foreach (var card in set.Value.Cards)
				{
					var small = Repo.GetSmallImage(card, ImgRepo);
					var zooms = Repo.GetZoomImages(card, ImgRepo);

					string message = $"{card.SetCode} {card.ImageName}";

					Assert.That(small, Is.Not.Null, message);
					Assert.That(zooms, Is.Not.Null, message);
					Assert.That(zooms, Is.Not.Empty, message);
				}
		}

		[Test, Order(2)]
		public void Zoom_images_match_small_ones()
		{
			foreach (var set in Repo.SetsByCode)
				foreach (var card in set.Value.Cards)
				{
					var small = Repo.GetSmallImage(card, ImgRepo);
					var zooms = Repo.GetZoomImages(card, ImgRepo);

					var smallPath = small.ImageFile.FullPath;
					var zoomPath = zooms[0].ImageFile.FullPath;

					smallPath = smallPath.ToLower(Str.Culture)
						.Replace("gatherer.original", "gatherer")
						.Replace("\\lq\\", string.Empty);

					zoomPath = zoomPath.ToLower(Str.Culture)
						.Replace("gatherer.preprocessed", "gatherer")
						.Replace("\\mq\\", string.Empty);

					if (!Str.Equals(smallPath, zoomPath))
						Assert.Fail(smallPath + Str.Endl + zoomPath);
				}
		}

		[TestCase("UGL", XlhqTorrentsDir, "UGL", "UGL Tokens")]
		[TestCase("DDE", XlhqTorrentsDir, "DDE", "DDE Tokens")]
		[TestCase("C17", XlhqDir, "C17 - Commander 2017\\300DPI Cards")]
		[TestCase("IMA", XlhqDir, "IMA - Iconic Masters\\300DPI Cards")]
		[TestCase("UST", XlhqDir, "UST - Unstable\\300DPI Cards")]
		[TestCase("CED", XlhqDir, "CED - Collectors' Edition\\300DPI")]
		[TestCase("XLN", XlhqDir, "XLN - Ixalan\\300DPI Cards")]
		[TestCase("CMA", XlhqDir, "CMA - Commander Anthology\\300DPI Cards")]
		[TestCase("DDT", XlhqDir, "DDT - Duel Decks Merfolk vs Goblins\\300DPI Cards")]
		[TestCase("E02", XlhqDir, "E02 - Explorers of Ixalan\\300DPI Cards")]
		[TestCase("RIX", XlhqDir, "RIX - Rivals of Ixalan\\300DPI Cards")]
		[TestCase("V17", XlhqDir, "V17 - From the Vault Transform\\300DPI Cards")]
		[TestCase("A25", XlhqDir, "A25 - 25 Masters\\300DPI Cards")]
		[TestCase("DDU", XlhqDir, "DDU - Duel Decks Elves vs Inventors\\300DPI Cards")]
		[TestCase("DOM", GathererDir, "DOM")]
		public void Set_images_are_from_expected_directory(string setCode, string baseDir, params string[] expectedSubdirs)
		{
			var expectedDirsSet = expectedSubdirs
				.Select(_ => Path.Combine(baseDir, _))
				.ToList();
				
			var set = Repo.SetsByCode[setCode];
			foreach (var card in set.Cards)
			{
				var imageModel = Repo.GetSmallImage(card, ImgRepo);
				var dir = Path.GetDirectoryName(imageModel.ImageFile.FullPath);
				Assert.That(expectedDirsSet, Does.Contain(dir).IgnoreCase, card.ImageName);
			}
		}

		private const string XlhqDir = "D:\\Distrib\\games\\mtg\\Mega\\XLHQ";
		private const string XlhqTorrentsDir = "D:\\Distrib\\games\\mtg\\XLHQ-Sets-Torrent.Unpacked";
		private const string GathererDir = "D:\\Distrib\\games\\mtg\\Gatherer.Original";
	}
}