﻿using System.Drawing;

namespace Mtgdb.Controls
{
	partial class DeckListControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Mtgdb.Controls.LayoutOptions layoutOptions3 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions3 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions3 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions3 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions3 = new Mtgdb.Controls.SortOptions();
			this._panelLayout = new System.Windows.Forms.TableLayoutPanel();
			this._panelSearch = new Mtgdb.Controls.BorderedPanel();
			this._panelSearchIcon = new Mtgdb.Controls.BorderedPanel();
			this._textBoxSearch = new Mtgdb.Controls.FixedRichTextBox();
			this._viewDecks = new Mtgdb.Controls.LayoutViewControl();
			this._labelFilterByDeckMode = new System.Windows.Forms.Label();
			this._menuFilterByDeckMode = new System.Windows.Forms.ComboBox();
			this._textBoxName = new System.Windows.Forms.TextBox();
			this._listBoxSuggest = new System.Windows.Forms.ListBox();
			this._panelLayout.SuspendLayout();
			this._panelSearch.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelLayout
			// 
			this._panelLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelLayout.ColumnCount = 3;
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.Controls.Add(this._panelSearch, 0, 0);
			this._panelLayout.Controls.Add(this._viewDecks, 0, 1);
			this._panelLayout.Controls.Add(this._labelFilterByDeckMode, 1, 0);
			this._panelLayout.Controls.Add(this._menuFilterByDeckMode, 2, 0);
			this._panelLayout.Location = new System.Drawing.Point(0, 0);
			this._panelLayout.Margin = new System.Windows.Forms.Padding(0);
			this._panelLayout.Name = "_panelLayout";
			this._panelLayout.RowCount = 2;
			this._panelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelLayout.Size = new System.Drawing.Size(732, 311);
			this._panelLayout.TabIndex = 0;
			// 
			// _panelSearch
			// 
			this._panelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelSearch.BackColor = System.Drawing.Color.White;
			this._panelSearch.Controls.Add(this._panelSearchIcon);
			this._panelSearch.Controls.Add(this._textBoxSearch);
			this._panelSearch.Location = new System.Drawing.Point(0, 0);
			this._panelSearch.Margin = new System.Windows.Forms.Padding(0);
			this._panelSearch.Name = "_panelSearch";
			this._panelSearch.Size = new System.Drawing.Size(434, 24);
			this._panelSearch.TabIndex = 1;
			this._panelSearch.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _panelSearchIcon
			// 
			this._panelSearchIcon.BackgroundImage = global::Mtgdb.Controls.Properties.Resources.search_40;
			this._panelSearchIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelSearchIcon.Location = new System.Drawing.Point(1, 1);
			this._panelSearchIcon.Margin = new System.Windows.Forms.Padding(1);
			this._panelSearchIcon.Name = "_panelSearchIcon";
			this._panelSearchIcon.Size = new System.Drawing.Size(22, 22);
			this._panelSearchIcon.TabIndex = 1;
			this._panelSearchIcon.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _textBoxSearch
			// 
			this._textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBoxSearch.DetectUrls = false;
			this._textBoxSearch.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._textBoxSearch.Location = new System.Drawing.Point(25, 5);
			this._textBoxSearch.Margin = new System.Windows.Forms.Padding(1, 5, 1, 1);
			this._textBoxSearch.Multiline = false;
			this._textBoxSearch.Name = "_textBoxSearch";
			this._textBoxSearch.Size = new System.Drawing.Size(408, 18);
			this._textBoxSearch.TabIndex = 0;
			this._textBoxSearch.TabStop = false;
			this._textBoxSearch.Text = "";
			this._textBoxSearch.WordWrap = false;
			// 
			// _viewDecks
			// 
			this._viewDecks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._viewDecks.BackColor = System.Drawing.Color.White;
			this._panelLayout.SetColumnSpan(this._viewDecks, 3);
			layoutOptions3.AlignTopLeftHoveredIcon = global::Mtgdb.Controls.Properties.Resources.corner_hovered_32;
			layoutOptions3.AlignTopLeftIcon = global::Mtgdb.Controls.Properties.Resources.corner_32;
			layoutOptions3.AllowPartialCards = true;
			layoutOptions3.CardInterval = new System.Drawing.Size(2, 0);
			layoutOptions3.PartialCardsThreshold = new System.Drawing.Size(150, 0);
			this._viewDecks.LayoutOptions = layoutOptions3;
			this._viewDecks.Location = new System.Drawing.Point(0, 24);
			this._viewDecks.Margin = new System.Windows.Forms.Padding(0);
			this._viewDecks.Name = "_viewDecks";
			searchOptions3.Allow = false;
			searchOptions3.Button = buttonOptions3;
			this._viewDecks.SearchOptions = searchOptions3;
			selectionOptions3.Alpha = ((byte)(192));
			selectionOptions3.BackColor = System.Drawing.Color.LightSkyBlue;
			selectionOptions3.ForeColor = System.Drawing.Color.Black;
			selectionOptions3.HotTrackBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
			selectionOptions3.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			selectionOptions3.RectAlpha = ((byte)(16));
			selectionOptions3.RectBorderColor = System.Drawing.Color.MediumBlue;
			selectionOptions3.RectFillColor = System.Drawing.Color.RoyalBlue;
			this._viewDecks.SelectionOptions = selectionOptions3;
			this._viewDecks.Size = new System.Drawing.Size(732, 287);
			this._viewDecks.SortOptions = sortOptions3;
			this._viewDecks.TabIndex = 2;
			// 
			// _labelFilterByDeckMode
			// 
			this._labelFilterByDeckMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._labelFilterByDeckMode.AutoSize = true;
			this._labelFilterByDeckMode.Location = new System.Drawing.Point(437, 0);
			this._labelFilterByDeckMode.Name = "_labelFilterByDeckMode";
			this._labelFilterByDeckMode.Size = new System.Drawing.Size(102, 24);
			this._labelFilterByDeckMode.TabIndex = 3;
			this._labelFilterByDeckMode.Text = "Filter cards by deck:";
			this._labelFilterByDeckMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _menuFilterByDeckMode
			// 
			this._menuFilterByDeckMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._menuFilterByDeckMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuFilterByDeckMode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuFilterByDeckMode.FormattingEnabled = true;
			this._menuFilterByDeckMode.Items.AddRange(new object[] {
            "Ignored",
            "Cards in currently open deck",
            "Cards in saved decks matching filter"});
			this._menuFilterByDeckMode.Location = new System.Drawing.Point(542, 1);
			this._menuFilterByDeckMode.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this._menuFilterByDeckMode.Name = "_menuFilterByDeckMode";
			this._menuFilterByDeckMode.Size = new System.Drawing.Size(190, 21);
			this._menuFilterByDeckMode.TabIndex = 4;
			// 
			// _textBoxName
			// 
			this._textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._textBoxName.Location = new System.Drawing.Point(24, 51);
			this._textBoxName.Margin = new System.Windows.Forms.Padding(10, 2, 0, 0);
			this._textBoxName.Multiline = true;
			this._textBoxName.Name = "_textBoxName";
			this._textBoxName.Size = new System.Drawing.Size(177, 20);
			this._textBoxName.TabIndex = 3;
			this._textBoxName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// _listBoxSuggest
			// 
			this._listBoxSuggest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._listBoxSuggest.Location = new System.Drawing.Point(209, 30);
			this._listBoxSuggest.Margin = new System.Windows.Forms.Padding(0);
			this._listBoxSuggest.Name = "_listBoxSuggest";
			this._listBoxSuggest.Size = new System.Drawing.Size(250, 93);
			this._listBoxSuggest.TabIndex = 4;
			// 
			// DeckListControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._listBoxSuggest);
			this.Controls.Add(this._textBoxName);
			this.Controls.Add(this._panelLayout);
			this.Name = "DeckListControl";
			this.Size = new System.Drawing.Size(732, 311);
			this._panelLayout.ResumeLayout(false);
			this._panelLayout.PerformLayout();
			this._panelSearch.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _panelLayout;
		private BorderedPanel _panelSearch;
		private FixedRichTextBox _textBoxSearch;
		private LayoutViewControl _viewDecks;
		private BorderedPanel _panelSearchIcon;
		private System.Windows.Forms.TextBox _textBoxName;
		private System.Windows.Forms.ListBox _listBoxSuggest;
		private System.Windows.Forms.Label _labelFilterByDeckMode;
		private System.Windows.Forms.ComboBox _menuFilterByDeckMode;
	}
}
