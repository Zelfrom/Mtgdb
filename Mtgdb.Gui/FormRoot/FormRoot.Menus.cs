﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupButtonClicks()
		{
			_buttonTooltips.Checked = true;
			_buttonDownload.Enabled = false;

			foreach (var button in _saveLoadButtons)
				button.MouseEnter += saveLoadMouseEnter;

			_buttonDownload.Click += downloadClick;

			_buttonOpenDeck.Click += openDeckClick;
			_buttonMenuOpenDeck.Click += openDeckClick;
			_buttonSaveDeck.Click += saveDeckClick;
			_buttonMenuSaveDeck.Click += saveDeckClick;

			_buttonMenuOpenCollection.Click += openCollectionClick;
			_buttonMenuSaveCollection.Click += saveCollectionClick;

			_buttonStat.Click += statClick;
			_buttonPrint.Click += printClick;
			_buttonClear.Click += clearClick;
			_buttonUndo.Click += undoClick;
			_buttonRedo.Click += redoClick;

			_buttonHelp.Click += helpClick;

			_buttonGeneralSettings.Click += configClick;
			_buttonDisplaySettings.Click += configClick;
			_buttonTooltips.CheckedChanged += tooltipsChecked;

			_buttonMenuPaste.Click += pasteClick;
			_buttonPaste.Click += pasteClick;
			_buttonMenuPasteAppend.Click += pasteClick;
		}

		private void pasteClick(object sender, EventArgs e)
		{
			getSelectedForm()?.PasteDeck(append: sender == _buttonMenuPasteAppend);
		}

		private void configClick(object sender, EventArgs e)
		{
			if (_buttonGeneralSettings == sender)
				System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.xml"));
			else if (_buttonDisplaySettings == sender)
				System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.Display.xml"));
		}

		private void tooltipsChecked(object sender, EventArgs e)
		{
			HideTooltips = !((CheckBox)sender).Checked;

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain)_tabs.TabIds[i];
				formMain?.ButtonTooltip();
			}
		}

		private void helpClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(AppDir.Root.AddPath("help\\html\\home.html"));
		}

		private void redoClick(object sender, EventArgs e)
		{
			if (_undoingOrRedoing)
				return;

			_undoingOrRedoing = true;
			getSelectedForm()?.ButtonRedo();
			_undoingOrRedoing = false;
		}

		private void undoClick(object sender, EventArgs e)
		{
			if (_undoingOrRedoing)
				return;

			_undoingOrRedoing = true;
			getSelectedForm()?.ButtonUndo();
			_undoingOrRedoing = false;
		}

		private void clearClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonClearDeck();
		}

		private void printClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonPrint();
		}

		private void statClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonPivot();
		}

		private void saveDeckClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonSaveDeck();
		}

		private void openDeckClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonLoadDeck();
		}

		private void saveCollectionClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonSaveCollection();
		}

		private void openCollectionClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonLoadCollection();
		}

		private void downloadClick(object sender, EventArgs e)
		{
			_downloaderSubsystem.ShowDownloader(this, auto: false);

			ThreadPool.QueueUserWorkItem(_ =>
			{
				Thread.Sleep(2000);
				this.Invoke(updateDownloadButton);
			});
		}

		private void setupLanguageMenu()
		{
			foreach (var langMenuItem in getLanguageMenuItems())
				langMenuItem.Click += languageMenuClick;
		}

		private void languageMenuClick(object sender, EventArgs e)
		{
			var button = (ButtonBase)sender;
			Language = button.Text.ToLowerInvariant();
		}

		private IEnumerable<CheckBox> getLanguageMenuItems()
		{
			return _menuLanguage.Controls.OfType<CheckBox>();
		}



		private void setupExternalLinks()
		{
			_buttonVisitForge.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=26");
			_buttonVisitForge.Click += buttonVisitClick;

			_buttonVisitXMage.SetTag(@"http://www.xmage.de");
			_buttonVisitXMage.LinkClicked += buttonVisitClick;

			_buttonVisitMagarena.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=82");
			_buttonVisitMagarena.LinkClicked += buttonVisitClick;

			_buttonVisitCockatrice.SetTag(@"https://cockatrice.github.io/");
			_buttonVisitCockatrice.LinkClicked += buttonVisitClick;

			_buttonVisitDotP2014.SetTag(@"https://www.slightlymagic.net/forum/viewtopic.php?f=99&t=10999&start=270#p213467");
			_buttonVisitDotP2014.LinkClicked += buttonVisitClick;

			_buttonDonatePayPal.SetTag(@"http://paypal.me/nidalgo");
			_buttonDonatePayPal.Click += buttonVisitClick;

			_buttonDonateYandexMoney.SetTag(@"https://money.yandex.ru/to/410012387625926?_openstat=template%3Bipulldown%3Btopupme");
			_buttonDonateYandexMoney.Click += buttonVisitClick;

			_buttonVisitMtgo.SetTag(AppDir.Root.AddPath("help\\html\\Import_collection_&_decks_from_Magic_The_Gathering_Online.html"));
			_buttonVisitMtgo.Click += buttonVisitClick;
		}

		private static void buttonVisitClick(object sender, EventArgs e)
		{
			var control = (Control)sender;
			var url = control.GetTag<string>();
			System.Diagnostics.Process.Start(url);
		}



		private void saveLoadMouseEnter(object sender, EventArgs e)
		{
			for (int i = 0; i < _saveLoadButtons.Length; i++)
			{
				bool visible = sender == _saveLoadButtons[i];
				var menuButtons = _saveLoadMenuButtons[i];

				for (int j = 0; j < menuButtons.Length; j++)
					menuButtons[j].Visible = visible;
			}
		}

		private void setupButtons()
		{
			foreach (var button in _saveLoadButtons)
				setupButton(button);

			setupButton(_buttonOpenDeck);
			setupButton(_buttonSaveDeck);

			setupButton(_buttonStat);
			setupButton(_buttonPrint);
			setupButton(_buttonClear);
			setupButton(_buttonDownload);
			setupButton(_buttonConfig);
			setupButton(_buttonHelp);
			setupButton(_buttonLanguage);
			setupButton(_buttonDonate);
			setupButton(_buttonTooltips);
			setupButton(_buttonPaste);
			setupButton(_buttonMenuPaste);
			setupButton(_buttonMenuPasteAppend);

			_buttonSubsystem.SetupPopup(new Popup(_menuConfig, _buttonConfig));

			_buttonSubsystem.SetupPopup(new Popup(_menuLanguage, _buttonLanguage,
				closeMenuOnClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuDonate, _buttonDonate,
				alignment: HorizontalAlignment.Center,
				openOnHover: false,
				closeMenuOnClick: true,
				borderOnHover: false));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen,
				_buttonOpenDeck,
				borderOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen,
				_buttonSaveDeck,
				borderOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuPaste,
				_buttonPaste,
				borderOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SubscribeToEvents();
		}

		private void setupButton(CheckBox button)
		{
			var image = (Bitmap) button.Image;
			var hoveredImage = image?.TransformColors(1.1f, 1.05f);

			_buttonSubsystem.SetupButton(button,
				new ButtonImages(
					image,
					image,
					hoveredImage,
					hoveredImage));
		}

		private void formKeyDown(object sender, KeyEventArgs e)
		{
			var form = getSelectedForm();
			if (form == null)
				return;

			bool handled = true;

			if (e.KeyData == Keys.F1)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"help\\html\\Search_input_keyboard_shortcuts.html"));
			else if (e.KeyData == (Keys.Control | Keys.F4))
				CloseTab();
			else if (e.KeyData == (Keys.Control | Keys.Tab))
				NextTab();
			else if (e.KeyData == (Keys.Control | Keys.T))
				NewTab(onCreated: null);
			else if (e.KeyData == (Keys.Alt | Keys.Left) || e.KeyData == (Keys.Control | Keys.Z))
				form.ButtonUndo();
			else if (e.KeyData == (Keys.Alt | Keys.Right) || e.KeyData == (Keys.Control | Keys.Y))
				form.ButtonRedo();
			else if (e.KeyData == (Keys.Control | Keys.F))
				form.FocusSearch();
			else if (e.KeyData == Keys.Enter)
				form.ApplySearch();
			else if (e.KeyData == Keys.Escape)
				form.StopDragging();
			else if (e.KeyData == (Keys.Control | Keys.S))
				form.ButtonSaveDeck();
			else if (e.KeyData == (Keys.Control | Keys.O))
				form.ButtonLoadDeck();
			else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.S))
				form.ButtonSaveCollection();
			else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.O))
				form.ButtonLoadCollection();
			else if (e.KeyData == (Keys.Control | Keys.P))
				form.ButtonPrint();
			else if (e.KeyData == (Keys.Control | Keys.V) || e.KeyData == (Keys.Shift | Keys.Insert))
				form.PasteDeck(append: false);
			else if (e.KeyData == (Keys.Control | Keys.Shift | Keys.V))
				form.PasteDeck(append: true);

			else if (!e.Modifiers.Equals(Keys.Alt))
				handled = false;

			e.Handled = handled;
		}

		private void unsubsribeButtonEvents()
		{
			_buttonSubsystem.UnsubscribeFromEvents();
		}



		public string Language
		{
			get { return _language; }
			set
			{
				if (value == _language)
					return;

				_language = value;

				var menuItem = getLanguageMenuItems()
					.Single(_ => Str.Equals(_.Text, value));

				_buttonLanguage.Image = menuItem.Image;
				_buttonLanguage.Text = value.ToUpperInvariant();
				setupButton(_buttonLanguage);

				LanguageChanged?.Invoke();
			}
		}

		private readonly ButtonBase[] _deckButtons;

		public event Action LanguageChanged;

		private readonly ButtonSubsystem _buttonSubsystem;
		private readonly CheckBox[] _saveLoadButtons;
		private readonly CheckBox[][] _saveLoadMenuButtons;
	}
}
