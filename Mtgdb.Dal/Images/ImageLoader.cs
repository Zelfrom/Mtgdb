﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Mtgdb.Controls;
using NLog;

namespace Mtgdb.Dal
{
	public class ImageLoader
	{
		public ImageLoader(ImageCacheConfig config)
		{
			Capacity = config.GetCacheCapacity();
		}

		public Bitmap GetSmallImage(ImageModel model)
		{
			if (model == null)
				return null;

			Bitmap image;
			lock (_imagesByPath)
				image = tryGetFromCache(model.ImageFile.FullPath, model.Rotation);

			if (image != null)
			{
				FoundInCache?.Invoke();
				return image;
			}

			if (!File.Exists(model.ImageFile.FullPath))
				return null;

			image = LoadImage(model, CardSize);

			lock (_imagesByPath)
				if (addFirst(model.ImageFile.FullPath, model.Rotation, image))
					if (_ratings.Count >= Capacity)
						removeLast();

			return image;
		}

		public Bitmap LoadImage(ImageModel model, Size size)
		{
			var original = Open(model);

			if (original == null)
				return null;

			var chain = model.ImageFile.IsArt
				? TransformArt(original, size)
				: Transform(original, size);

			using (chain)
			{
				chain.DisposeDifferentOriginal();
				return chain.Result;
			}
		}

		public static Bitmap Open(ImageModel model)
		{
			try
			{
				byte[] bytes;

				lock (SyncRoot)
					bytes = File.ReadAllBytes(model.ImageFile.FullPath);

				var result = new Bitmap(new MemoryStream(bytes));

				if (model.Rotation != RotateFlipType.RotateNoneFlipNone)
					result.RotateFlip(model.Rotation);

				return result;
			}
			catch
			{
				// Некорректный файл изображения.
				// Штатная ситуация из за возможного прерывания скачивания файла.
				return null;
			}
		}



		public BitmapTransformationChain TransformForgeImage(Bitmap original, Size size)
		{
			var frameDetector = new BmpFrameDetector(original, SizeCropped);
			frameDetector.Execute();
			var frame = frameDetector.Frame;

			var chain = new BitmapTransformationChain(original, logException);
			chain.TransformCopying(_ => resize(_, size, frame));
			return chain;
		}

		public BitmapTransformationChain TransformArt(Bitmap original, Size size)
		{
			var chain = new BitmapTransformationChain(original, logException);

			if (!original.Size.FitsIn(size))
				chain.TransformCopying(_ => resize(_, size));

			return chain;
		}

		public BitmapTransformationChain Transform(Bitmap original, Size size)
		{
			var chain = new BitmapTransformationChain(original, logException);
			chain.TransformCopying(_ => resize(_, size));
			chain.TransformInplace(removeCorners);

			return chain;
		}



		private static Bitmap resize(Bitmap original, Size requiredSize, Size frame = default(Size))
		{
			if (frame == default(Size) && original.Size == requiredSize)
				return original;

			return original.FitIn(requiredSize, frame);
		}

		private void removeCorners(Bitmap bitmap)
		{
			var remover = new BmpCornerRemoval(bitmap);
			remover.Execute();

			if (remover.ImageChanged)
				CornerRemoved?.Invoke();
		}



		private Bitmap tryGetFromCache(string path, RotateFlipType rotations)
		{
			if (!_imagesByPath.TryGetValue(new Tuple<string, RotateFlipType>(path, rotations), out var cacheEntry))
				return null;

			shiftFromLast(cacheEntry);

			return cacheEntry.Image;
		}

		private static void shiftFromLast(ImageCacheEntry cacheEntry)
		{
			var ratingEntry = cacheEntry.RatingEntry;
			var previousEntry = ratingEntry.Previous;

			if (previousEntry != null)
				ratingEntry.SwapWith(previousEntry);
		}

		private bool addFirst(string path, RotateFlipType rotations, Bitmap image)
		{
			var key = new Tuple<string, RotateFlipType>(path, rotations);

			if (_imagesByPath.ContainsKey(key))
				return false;

			_ratings.AddFirst(key);
			_imagesByPath[key] = new ImageCacheEntry(image, _ratings.First);

			return true;
		}

		private void removeLast()
		{
			var keyToRemove = _ratings.Last.Value;
			_ratings.RemoveLast();
			_imagesByPath.Remove(keyToRemove);
		}

		private static void logException(Exception ex) => _logger.Error(ex);

		public event Action CornerRemoved;
		public event Action FoundInCache;

		public static readonly object SyncRoot = new object();

		private readonly Dictionary<Tuple<string, RotateFlipType>, ImageCacheEntry> _imagesByPath =
			new Dictionary<Tuple<string, RotateFlipType>, ImageCacheEntry>();

		private readonly LinkedList<Tuple<string, RotateFlipType>> _ratings =
			new LinkedList<Tuple<string, RotateFlipType>>();


		public static readonly Size SizeCropped = new Size(470, 659);
		private Size _cardSize = new Size(223, 311);
		private Size _zoomedCardSize = new Size(446, 622);

		public Size CardSize => _cardSize.ByDpi();
		public Size ZoomedCardSize => _zoomedCardSize.ByDpi();

		public int Capacity { get; }

		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
	}
}