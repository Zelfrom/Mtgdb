﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public partial class DeckListLayout : LayoutControl
	{
		public DeckListLayout()
		{
			InitializeComponent();

			_fieldName.FieldName = nameof(DeckModel.Name);
			_fieldGeneratedMana.FieldName = nameof(DeckModel.Mana);

			_fieldLegality.FieldName = nameof(DeckModel.Legal);
			_fieldSaved.FieldName = nameof(DeckModel.Saved);



			_fieldLandCount.FieldName = nameof(DeckModel.LandCount);
			_fieldCreatureCount.FieldName = nameof(DeckModel.CreatureCount);
			_fieldOtherCount.FieldName = nameof(DeckModel.OtherSpellCount);

			_fieldMainCount.FieldName = nameof(DeckModel.MainCount);
			_fieldMainCollectedCount.FieldName = nameof(DeckModel.MainCollectedCount);
			_fieldMainCollectedCountPercent.FieldName = nameof(DeckModel.MainCollectedCountPercent);

			_fieldSideCount.FieldName = nameof(DeckModel.SideCount);
			_fieldSideCollectedCount.FieldName = nameof(DeckModel.SideCollectedCount);
			_fieldSideCollectedCountPercent.FieldName = nameof(DeckModel.SideCollectedCountPercent);



			_fieldLandPrice.FieldName = nameof(DeckModel.LandPrice);
			_fieldCreaturePrice.FieldName = nameof(DeckModel.CreaturePrice);
			_fieldOtherPrice.FieldName = nameof(DeckModel.OtherSpellPrice);

			_fieldMainPrice.FieldName = nameof(DeckModel.MainPrice);
			_fieldMainCollectedPrice.FieldName = nameof(DeckModel.MainCollectedPrice);
			_fieldMainCollectedPricePercent.FieldName = nameof(DeckModel.MainCollectedPricePercent);

			_fieldSidePrice.FieldName = nameof(DeckModel.SidePrice);
			_fieldSideCollectedPrice.FieldName = nameof(DeckModel.SideCollectedPrice);
			_fieldSideCollectedPricePercent.FieldName = nameof(DeckModel.SideCollectedPricePercent);



			_fieldLandUnknownPrice.FieldName = nameof(DeckModel.LandUnknownPriceCount);
			_fieldCreatureUnknownPrice.FieldName = nameof(DeckModel.CreatureUnknownPriceCount);
			_fieldOtherUnknownPrice.FieldName = nameof(DeckModel.OtherSpellUnknownPriceCount);

			_fieldMainUnknownPrice.FieldName = nameof(DeckModel.MainUnknownPriceCount);
			_fieldMainCollectedUnknownPrice.FieldName = nameof(DeckModel.MainCollectedUnknownPriceCount);
			_fieldMainCollectedUnknownPricePercent.FieldName = nameof(DeckModel.MainCollectedUnknownPricePercent);

			_fieldSideUnknownPrice.FieldName = nameof(DeckModel.SideUnknownPriceCount);
			_fieldSideCollectedUnknownPrice.FieldName = nameof(DeckModel.SideCollectedUnknownPriceCount);
			_fieldSideCollectedUnknownPricePercent.FieldName = nameof(DeckModel.SideCollectedUnknownPricePercent);

			_fieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.Remove_16,
				ShowOnlyWhenHotTracked = false
			});

			_fieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.Open_16,
				ShowOnlyWhenHotTracked = false
			});

			_fieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.Add_16,
				ShowOnlyWhenHotTracked = false
			});

			_fieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.Rename_16,
				Margin = new Size(8, 0),
				ShowOnlyWhenHotTracked = false
			});

			SubscribeToFieldEvents();
		}

		protected override void LoadData(object dataSource)
		{
			const string formatPercent = "0%";
			const string formatPrice = "$0.##";

			var deck = (DeckModel) dataSource;

			_fieldName.DataText = deck?.Name.NullIfEmpty() ?? "[no name]";
			_fieldGeneratedMana.DataText = deck?.Mana;

			var saved = deck?.Saved?.Invoke(DeckDocumentAdapter.Format);

			_fieldLegality.DataText = deck?.Legal.Invoke2(string.Join, ", ");
			_fieldSaved.DataText = saved != null
				? "saved\n" + saved
				: null;

			_fieldLandCount.DataText = deck?.LandCount.ToString(Str.Culture);
			_fieldCreatureCount.DataText = deck?.CreatureCount.ToString(Str.Culture);
			_fieldOtherCount.DataText = deck?.OtherSpellCount.ToString(Str.Culture);

			_fieldMainCount.DataText = deck?.MainCount.ToString(Str.Culture);
			_fieldMainCollectedCount.DataText = deck?.MainCollectedCount.ToString(Str.Culture);
			_fieldMainCollectedCountPercent.DataText = deck?.MainCollectedCountPercent.ToString(formatPercent, Str.Culture).Replace("NaN", "-");

			_fieldSideCount.DataText = deck?.SideCount.ToString(Str.Culture);
			_fieldSideCollectedCount.DataText = deck?.SideCollectedCount.ToString(Str.Culture);
			_fieldSideCollectedCountPercent.DataText = deck?.SideCollectedCountPercent.ToString(formatPercent, Str.Culture).Replace("NaN", "-");



			_fieldLandPrice.DataText = deck?.LandPrice.ToString(formatPrice, Str.Culture);
			_fieldCreaturePrice.DataText = deck?.CreaturePrice.ToString(formatPrice, Str.Culture);
			_fieldOtherPrice.DataText = deck?.OtherSpellPrice.ToString(formatPrice, Str.Culture);

			_fieldMainPrice.DataText = deck?.MainPrice.ToString(formatPrice, Str.Culture);
			_fieldMainCollectedPrice.DataText = deck?.MainCollectedPrice.ToString(formatPrice, Str.Culture);
			_fieldMainCollectedPricePercent.DataText = deck?.MainCollectedPricePercent.ToString(formatPercent, Str.Culture).Replace("NaN", "-");

			_fieldSidePrice.DataText = deck?.SidePrice.ToString(formatPrice, Str.Culture);
			_fieldSideCollectedPrice.DataText = deck?.SideCollectedPrice.ToString(formatPrice, Str.Culture);
			_fieldSideCollectedPricePercent.DataText = deck?.SideCollectedPricePercent.ToString(formatPercent, Str.Culture).Replace("NaN", "-");



			_fieldLandUnknownPrice.DataText = deck?.LandUnknownPriceCount.ToString(Str.Culture);
			_fieldCreatureUnknownPrice.DataText = deck?.CreatureUnknownPriceCount.ToString(Str.Culture);
			_fieldOtherUnknownPrice.DataText = deck?.OtherSpellUnknownPriceCount.ToString(Str.Culture);

			_fieldMainUnknownPrice.DataText = deck?.MainUnknownPriceCount.ToString(Str.Culture);
			_fieldMainCollectedUnknownPrice.DataText = deck?.MainCollectedUnknownPriceCount.ToString(Str.Culture);
			_fieldMainCollectedUnknownPricePercent.DataText = deck?.MainCollectedUnknownPricePercent.ToString(formatPercent, Str.Culture).Replace("NaN", "-");

			_fieldSideUnknownPrice.DataText = deck?.SideUnknownPriceCount.ToString(Str.Culture);
			_fieldSideCollectedUnknownPrice.DataText = deck?.SideCollectedUnknownPriceCount.ToString(Str.Culture);
			_fieldSideCollectedUnknownPricePercent.DataText = deck?.SideCollectedUnknownPricePercent.ToString(formatPercent, Str.Culture).Replace("NaN", "-");
		}

		public override IEnumerable<FieldControl> Fields => _panelLayout.Controls.Cast<FieldControl>();

		public override IEnumerable<ButtonLayout> GetCustomButtons(FieldControl field)
		{
			var deckModel = (DeckModel) DataSource;

			var baseResult = base.GetCustomButtons(field);

			if (field != _fieldSaved)
				return baseResult;

			var list = baseResult.ToList();

			if (deckModel.IsCurrent)
			{
				//foreach (var btn in list)
				//	btn.Margin = new Size(24, 0);

				list[CustomButtonRemove].Size = Size.Empty;
				list[CustomButtonOpen].Size = Size.Empty;
			}
			else
				list[CustomButtonAdd].Size = Size.Empty;

			return list;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image ImageDeckBoxOpened { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image ImageDeckBox { get; set; }

		public override Image BackgroundImage
		{
			get =>
				((DeckModel) DataSource)?.IsCurrent == true
					? ImageDeckBoxOpened
					: ImageDeckBox;

			set => throw new NotSupportedException();
		}

		public override void CopyFrom(LayoutControl other)
		{
			base.CopyFrom(other);
			ImageDeckBox = ((DeckListLayout) other).ImageDeckBox;
			ImageDeckBoxOpened = ((DeckListLayout) other).ImageDeckBoxOpened;
		}

		public override bool ShowSortButton(FieldControl field) =>
			field.IsHotTracked && field.AllowSort || field.SortOrder != SortOrder.None;

		public const int CustomButtonRemove = 0;
		public const int CustomButtonOpen = 1;
		public const int CustomButtonAdd = 2;
		public const int CustomButtonRename = 3;
	}
}