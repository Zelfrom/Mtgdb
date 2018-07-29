﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public sealed partial class FormZoom : Form
	{
		[UsedImplicitly] // by WinForms designer
		public FormZoom()
		{
			InitializeComponent();
		}

		public FormZoom(
			CardRepository cardRepository, 
			ImageRepository imageRepository,
			ImageLoader imageLoader)
			: this()
		{
			_cardRepository = cardRepository;
			_imageRepository = imageRepository;
			_imageLoader = imageLoader;

			BackgroundImageLayout = ImageLayout.Zoom;
			TransparencyKey = BackColor = _defaultBgColor;

			_pictureBox.MouseClick += click;
			MouseWheel += mouseWheel;
			DoubleBuffered = true;

			var hotSpot = new Size(14, 8).ByDpi();
			var cursorImage = Resources.rightclick_48.HalfResizeDpi();
			Cursor = CursorHelper.CreateCursor(cursorImage, hotSpot);

			_openFileButton.Image = Dpi.ScalePercent > 100
				? Resources.image_file_32.HalfResizeDpi()
				: Resources.image_file_16.ResizeDpi();

			_showInExplorerButton.Image = Dpi.ScalePercent > 100
				? Resources.open_32.HalfResizeDpi()
				: Resources.open_16.ResizeDpi();

			_showArtButton.Image = Dpi.ScalePercent > 100
				? Resources.art_64.HalfResizeDpi()
				: Resources.art_32.ResizeDpi();

			var cloneImg = Resources.clone_48.HalfResizeDpi();

			_showDuplicatesButton.Image = cloneImg;
			_showOtherSetsButton.Image = cloneImg;

			_showArtButton.CheckedChanged += showArtChanged;
			updateShowArt();
		}

		public async Task LoadImages(Card card, UiModel ui)
		{
			_location = Cursor.Position;
			await runLoadImagesTask(card, ui);
		}

		private async Task runLoadImagesTask(Card card, UiModel ui)
		{
			_card = card;
			_ui = ui;

			_cts?.Cancel();

			_cardForms = _cardRepository.GetForms(card, ui);

			foreach (var oldImg in _images)
				oldImg.Dispose();

			_images.Clear();
			_models.Clear();
			_imageIndex = 0;

			var loadingCancellation = new CancellationTokenSource();
			var waitingCancellation = new CancellationTokenSource();

#pragma warning disable 4014
			TaskEx.Run(async () =>
#pragma warning restore 4014
			{
				await loadImages(loadingCancellation.Token);
				waitingCancellation.Cancel();
			});

			await someImageLoaded(waitingCancellation.Token);

			_cts = loadingCancellation;
		}

		private async Task someImageLoaded(CancellationToken cancellation)
		{
			while (_images.Count == 0 && !cancellation.IsCancellationRequested)
				await TaskEx.Delay(50);
		}

		private void showArtChanged(object sender, EventArgs e)
		{
			updateShowArt();

			TaskEx.Run(async () =>
			{
				await runLoadImagesTask(_card, _ui);

				this.Invoke(delegate
				{
					updateImage();
					applyZoom();
				});
			});
		}

		private void updateShowArt() =>
			_showArt = _showArtButton.Checked;

		public void ShowImages()
		{
			updateImage();
			applyZoom();

			System.Windows.Forms.Application.DoEvents();

			Show();
			Focus();
		}


		private async Task loadImages(CancellationToken token)
		{
			bool repoLoadingComplete = isRepoLoadingComplete();
			await load(token);

			if (repoLoadingComplete)
				return;

			while (!isRepoLoadingComplete())
				await TaskEx.Delay(100);

			await load(token);
		}

		private bool isRepoLoadingComplete()
		{
			return _imageRepository.IsLoadingArtComplete && _imageRepository.IsLoadingZoomComplete;
		}

		private async Task load(CancellationToken token)
		{
			int index = 0;
			for (int j = 0; j < _cardForms.Count; j++)
			{
				if (token.IsCancellationRequested)
					return;

				if (_showArt)
					foreach (var model in _cardRepository.GetImagesArt(_cardForms[j], _imageRepository))
					{
						while (index > _imageIndex + 10 && !token.IsCancellationRequested)
							await TaskEx.Delay(100);

						if (token.IsCancellationRequested)
							return;

						var size = model.ImageFile.IsArt
							? getSizeArt()
							: _imageLoader.ZoomedCardSize;

						var image = _imageLoader.LoadImage(model, size);

						if (image == null)
							continue;

						add(index, model, image);
						index++;
					}

				foreach (var model in _cardRepository.GetZoomImages(_cardForms[j], _imageRepository))
				{
					while (index > _imageIndex + 10 && !token.IsCancellationRequested)
						await TaskEx.Delay(100);

					if (token.IsCancellationRequested)
						return;

					var size = model.ImageFile.IsArt
						? getSizeArt()
						: _imageLoader.ZoomedCardSize;

					var image = _imageLoader.LoadImage(model, size);

					if (image == null)
						continue;

					add(index, model, image);
					index++;
				}
			}
		}

		private void add(int index, ImageModel model, Bitmap image)
		{
			if (index < _images.Count)
			{
				_images[index] = image;
				_models[index] = model;
			}
			else
			{
				_images.Add(image);
				_models.Add(model);
			}
		}

		private static Size getSizeArt()
		{
			var screenArea = getScreenArea();

			if (screenArea.Width > screenArea.Height)
				return new SizeF(screenArea.Height + (screenArea.Width - screenArea.Height) * 0.75f, screenArea.Height).Round();
			else
				return new SizeF(screenArea.Width, screenArea.Width + (screenArea.Height - screenArea.Width) * 0.75f).Round();
		}

		private void updateImage()
		{
			_image = _images[_imageIndex];
			_pictureBox.Image = _image;
		}

		private void mouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta == 0)
				return;

			bool changed;
			if (e.Delta < 0)
				changed = previousImage();
			else
				changed = nextImage();

			if (changed)
			{
				updateImage();
				applyZoom();
				System.Windows.Forms.Application.DoEvents();
			}
		}

		private bool filter(ImageModel imageModel)
		{
			if (!_showOtherSetsButton.Checked && imageModel.ImageFile.SetCode != _card.SetCode)
				return false;

			if (_showDuplicatesButton.Checked)
				return true;

			var currentSetCode = _models[_imageIndex].ImageFile.SetCode;

			var setRepresentative = _models.Where(_ => _.ImageFile.SetCode == currentSetCode)
				.AtMin(_ => _.ImageFile.VariantNumber).Find();

			return imageModel.ImageFile.SetCode != currentSetCode || imageModel == setRepresentative;
		}

		private bool nextImage()
		{
			for (int i = _imageIndex + 1; i < _images.Count; i++)
				if (filter(_models[i]))
				{
					_imageIndex = i;
					return true;
				}

			return false;
		}

		private bool previousImage()
		{
			for (int i = _imageIndex - 1; i >= 0; i--)
				if (filter(_models[i]))
				{
					_imageIndex = i;
					return true;
				}

			return false;
		}

		private void applyZoom()
		{
			var formLocation = new Point(
				(int)(_location.X - _image.Size.Width * 0.5f),
				(int)(_location.Y - _image.Size.Height * 0.5f));

			var formArea = new Rectangle(formLocation, _image.Size);

			var screenArea = getScreenArea();
			var workingArea = new Rectangle(
				screenArea.Left,
				screenArea.Top,
				screenArea.Width,
				screenArea.Height);

			if (formArea.Bottom > workingArea.Bottom)
				formArea.Offset(0, workingArea.Bottom -formArea.Bottom);
				
			if (formArea.Right > workingArea.Right)
				formArea.Offset(workingArea.Right - formArea.Right, 0);

			if (formArea.Left < workingArea.Left)
				formArea.Offset(workingArea.Left - formArea.Left, 0);

			if (formArea.Top < workingArea.Top)
				formArea.Offset(0, workingArea.Top - formArea.Top);

			_pictureBox.Size = _image.Size;
			_pictureBox.Location = Point.Empty;

			Size = formArea.Size;
			Location = formArea.Location;
		}

		private static Rectangle getScreenArea()
		{
			var screen = Screen.FromPoint(Cursor.Position);
			var workingArea = screen.WorkingArea;
			return workingArea;
		}

		private void click(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			hideImage();
		}

		private void hideImage()
		{
			_cts?.Cancel();
			
			_pictureBox.Image = null;
			System.Windows.Forms.Application.DoEvents();
			Hide();
		}

		private void openInExplorerClick(object sender, EventArgs e)
		{
			string fullPath = _models[_imageIndex].ImageFile.FullPath;

			if (!File.Exists(fullPath))
				return;

			string workingDirectory = Path.GetDirectoryName(fullPath);
			if (workingDirectory == null)
				return;

			Process.Start(
				new ProcessStartInfo("explorer.exe", $@"/select,""{fullPath}"""));
		}

		private void openFileClick(object sender, EventArgs e)
		{
			string fullPath = _models[_imageIndex].ImageFile.FullPath;
			Process.Start(new ProcessStartInfo(fullPath));
		}



		private readonly CardRepository _cardRepository;
		private readonly ImageRepository _imageRepository;
		private readonly ImageLoader _imageLoader;
		private int _imageIndex;
		private Bitmap _image;

		private readonly List<Bitmap> _images = new List<Bitmap>();
		private readonly List<ImageModel> _models = new List<ImageModel>();

		private static readonly Color _defaultBgColor = Color.FromArgb(254, 247, 253);
		private Card _card;

		private List<Card> _cardForms;
		private Point _location;
		private bool _showArt;
		private UiModel _ui;

		private CancellationTokenSource _cts;
	}
}