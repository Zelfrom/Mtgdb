﻿using System;
using System.ComponentModel;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupTooltips()
		{
			TooltipController.SetTooltip("Undo: Ctrl+Z, Alt+Left",
				"Click to undo your last action",
				_buttonUndo);

			TooltipController.SetTooltip("Redo: Ctrl+Y, Alt+Right",
				"Click to repeat the action cancelled with undo",
				_buttonRedo);

			TooltipController.SetTooltip("Tabbed Document Interface (TDI)",
				"Add tab: Ctrl+T, click '+' button\r\n" +
				"Remove tab: Ctrl+F4, click 'x' button, Middle mouse click\r\n" +
				"Select next tab: Ctrl+Tab\r\n" +
				"Use drag-n-drop to reorder tabs.\r\n" +
				"Drag the card here to select or create another tab\r\n" +
				"where you want to drop the card.",
				_tabs);

			TooltipController.SetTooltip("Deck statistics",
				"Opens a Pivot report window. Use it to view \r\n" +
				"mana curve, price breakdown, or create \r\n" +
				"a custom report by moving field captions between\r\n" +
				"Row, Column and Summary areas of grid.",
				_buttonStat);

			TooltipController.SetTooltip("Print deck: Ctrl+P",
				"The print buttons doesn't actually print, instead\r\n" +
				"it creates images of cards by groups of 8\r\n" +
				"that can be printed on A4 paper.",
				_buttonPrint);

			TooltipController.SetTooltip("Clear deck",
				"Use it to start creating a new deck from scratch",
				_buttonClear);

			TooltipController.SetTooltip("Enable / disable tooltips",
				"Tooltips are helpful but also annoying.\r\n" +
				"Uncheck this button to disable tooltips.",
				_buttonTooltips);

			TooltipController.SetTooltip("Show / hide filter panels",
				"filter panels are located on top and right edges of the window.",
				_buttonFilterPanels);

			TooltipController.SetTooltip("Update",
				"Shows a window where you can\r\n" +
				"  * Check for a new version of Mtgdb.Gui\r\n" +
				"  * Download the most recent cards database from Mtgjson.com\r\n" +
				"  * Download card images\r\n" +
				"  * Download artworks",
				_buttonDownload);

			TooltipController.SetTooltip("Advanced settings",
				"Opens configuration file.\r\n" +
				"Use it to tell the program where to find your custom card images or tweak some other settings.\r\n\r\n" +
				"Configuration file is opened by whatever application you have associated with *.xml files. " +
				"If it's Internet Explorer, you need to assign *.xml extension to a text editor instead. " +
				"I recommend using an editor with XML syntax highlighting e.g. Notepad++.\r\n\r\n" +
				"To apply your changes save the modified configuration file and restart the program.",
				_buttonConfig
			);

			TooltipController.SetTooltip("Open new window",
				"You can drag-n-drop Cards and Tabs between windows.",
				_buttonOpenWindow);

			Load += loadTooltips;
			Closing += closeTooltips;
		}

		private void loadTooltips(object sender, EventArgs e)
		{
			TooltipController.SubscribeToEvents();
			TooltipController.StartThread();
		}

		private void closeTooltips(object sender, CancelEventArgs e)
		{
			TooltipController.AbortThread();
		}
	}
}
