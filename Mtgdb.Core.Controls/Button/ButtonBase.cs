using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ButtonBase : Control
	{
		public ButtonBase()
		{
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor |
				ControlStyles.Selectable,
				value: true);

			BackColor = Color.Transparent;
			ForeColor = SystemColors.ControlText;

			Padding = new Padding(4);

			updateHighlightColors();
			updateDisabledBorder();

			EnabledChanged += enabledChanged;
			Paint += paint;
			MouseDown += mouseDown;
			MouseClick += mouseClick;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;
			GotFocus += gotFocus;
			LostFocus += lostFocus;
			KeyDown += keyDown;
			ColorSchemeController.SystemColorsChanged += systemColorsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			EnabledChanged -= enabledChanged;
			Paint -= paint;
			MouseDown -= mouseDown;
			MouseClick -= mouseClick;
			MouseEnter -= mouseEnter;
			MouseLeave -= mouseLeave;
			GotFocus -= gotFocus;
			LostFocus -= lostFocus;
			KeyDown -= keyDown;
			ColorSchemeController.SystemColorsChanged -= systemColorsChanged;

			base.Dispose(disposing);
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var (imageSize, textSize) = measure(e.Graphics);
			var (textRect, imageRect) = layout();

			paintText();
			paintImage();
			paintBorder();
			paintFocusRectangle();

			void paintImage()
			{
				var image = Enabled ? Image : _imageDisabled;
				if (image != null)
					e.Graphics.DrawImage(image, imageRect);
			}

			void paintText()
			{
				if (!string.IsNullOrEmpty(Text))
				{
					var foreColor = Enabled ? ForeColor : DisabledForeColor;
					var format = TextFormat;

					switch (TextAlign)
					{
						case StringAlignment.Far:
							format |= TextFormatFlags.Right;
							break;

						case StringAlignment.Center:
							format |= TextFormatFlags.HorizontalCenter;
							break;
					}

					e.Graphics.DrawText(Text, Font, textRect, foreColor, format);
				}
			}

			void paintBorder()
			{
				var borderColor = Enabled ? BorderColor : _disabledBorderColor;
				this.PaintBorder(e.Graphics, VisibleBorders, borderColor, BorderStyle);
			}

			void paintFocusRectangle()
			{
				if (ContainsFocus && _checkedBackColor.A > 0)
				{
					var rectangle = new Rectangle(default, Size);
					int width = 2;
					rectangle.Inflate(-(width - 1), -(width - 1));
					var pen = new Pen(_checkedBackColor) { Width = width, DashStyle = DashStyle.Dot };
					
					e.Graphics.DrawRectangle(pen, rectangle);
				}
			}

			(Rectangle textRect, Rectangle imageRect) layout()
			{
				Rectangle textRectangle;
				Rectangle imageRectangle;

				int x = Padding.Left;
				int y = Padding.Top;
				switch (TextImageRelation)
				{
					case TextImageRelation.ImageBeforeText:

						if (textSize == default)
							imageRectangle = alignBoth(imageSize);
						else
							imageRectangle = alignVertically(imageSize);

						if (imageSize != default)
							x = Width - Padding.Right - textSize.Width;

						textRectangle = alignVertically(textSize);
						break;

					case TextImageRelation.TextBeforeImage:
						textRectangle = alignVertically(textSize);

						if (textSize == default)
							imageRectangle = alignBoth(imageSize);
						else
						{
							x = Width - Padding.Right - imageSize.Width;
							imageRectangle = alignVertically(imageSize);
						}
						break;

					case TextImageRelation.ImageAboveText:
						if (textSize == default)
							imageRectangle = alignBoth(imageSize);
						else
							imageRectangle = alignHorizontally(imageSize);

						if (imageSize != default)
							y = Height - Padding.Bottom - textSize.Height;

						textRectangle = alignHorizontally(textSize);
						break;

					case TextImageRelation.TextAboveImage:
						textRectangle = alignHorizontally(textSize);
						if (textSize == default)
							imageRectangle = alignBoth(imageSize);
						else
						{
							y = Height - Padding.Bottom - imageSize.Height;
							imageRectangle = alignHorizontally(imageSize);
						}

						break;

					case TextImageRelation.Overlay:
						imageRectangle = alignBoth(imageSize);
						textRectangle = alignHorizontally(textSize);
						break;

					default:
						throw new NotSupportedException();
				}

				return (textRect: textRectangle, imageRect: imageRectangle);

				Rectangle alignVertically(Size itemSize) =>
					new Rectangle(new Point(x, centerTop(itemSize)), itemSize);

				Rectangle alignHorizontally(Size itemSize) =>
					new Rectangle(new Point(centerLeft(itemSize), y), itemSize);

				Rectangle alignBoth(Size itemSize) =>
					new Rectangle(new Point(centerLeft(itemSize), centerTop(itemSize)), itemSize);

				int centerTop(Size itemSize) =>
					Padding.Top + (Height - Padding.Vertical - itemSize.Height) / 2;

				int centerLeft(Size itemSize) =>
					Padding.Left + (Width - Padding.Horizontal - itemSize.Width) / 2;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) =>
			this.PaintPanelBack(e.Graphics, e.ClipRectangle, BackgroundImage, getBgColor(), PaintBackground);

		private Color getBgColor()
		{
			if (MouseOver)
				return _mouseOverBackColor;

			if (Checked)
				return _checkedBackColor;

			return BackColor;
		}

		private void mouseLeave(object sender, EventArgs e) =>
			MouseOver = false;

		private void mouseEnter(object sender, EventArgs e) =>
			MouseOver = true;

		private void mouseDown(object sender, MouseEventArgs e)
		{
			Focus();
			if (e.Button == MouseButtons.Left)
				PressDown?.Invoke(this, EventArgs.Empty);
		}

		private void lostFocus(object sender, EventArgs e) =>
			Invalidate();

		private void gotFocus(object sender, EventArgs e) =>
			Invalidate();

		private void mouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				onPressed();
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Enter:
				case Keys.Space:
					PressDown?.Invoke(this, EventArgs.Empty);
					onPressed();
					break;
			}
		}

		private void onPressed()
		{
			if (AutoCheck)
				Checked = !Checked;

			Pressed?.Invoke(this, EventArgs.Empty);
		}

		private void systemColorsChanged()
		{
			updateHighlightColors();
			updateDisabledBorder();
			updateDisabledImage();
			Invalidate();
		}

		private void enabledChanged(object sender, EventArgs e) =>
			Invalidate();

		private void updateSize()
		{
			if (AutoSize)
				Size = MeasureContent();
		}

		public Size MeasureContent()
		{
			var g = CreateGraphics();
			var (imageSize, textSize) = measure(g);

			switch (TextImageRelation)
			{
				case TextImageRelation.ImageBeforeText:
				case TextImageRelation.TextBeforeImage:
					return new Size(
						imageSize.Width + textSize.Width + Padding.Horizontal,
						Math.Max(imageSize.Height, textSize.Height) + Padding.Vertical);

				case TextImageRelation.ImageAboveText:
				case TextImageRelation.TextAboveImage:
					return new Size(
						Math.Max(imageSize.Width, textSize.Width) + Padding.Horizontal,
						imageSize.Height + textSize.Height + Padding.Vertical);

				case TextImageRelation.Overlay:
					return new Size(
						Math.Max(imageSize.Width, textSize.Width) + Padding.Horizontal,
						Math.Max(imageSize.Height, textSize.Height) + Padding.Vertical);

				default:
					throw new NotSupportedException();
			}
		}

		private void updateHighlightColors()
		{
			if (DesignMode)
				return;

			_mouseOverBackColor = blend(BackColor, _highlightBackColor, _highlightMouseOverOpacity);
			_checkedBackColor = blend(BackColor, _highlightBackColor, _highlightCheckedOpacity);
		}

		private static Color blend(Color bg, Color fore, int foreOpacity)
		{
			if (bg == Color.Empty || bg == Color.Transparent)
				return Color.FromArgb(foreOpacity, fore);

			byte o1 = bg.A;
			return Color.FromArgb(
				(255 * 255 - (255 - o1) * (255 - foreOpacity)) / 255,
				blendBytes(bg.R, fore.R),
				blendBytes(bg.G, fore.G),
				blendBytes(bg.B, fore.B));

			int blendBytes(byte b1, byte b2)
			{
				int share2 = 255 * foreOpacity;
				int share1 = o1 * (255 - foreOpacity);

				return (b1 * share1 + b2 * share2) / (share1 + share2);
			}
		}

		private void updateDisabledBorder() =>
			_disabledBorderColor = blend(Color.Transparent, _borderColor, _disabledOpacity);

		private void updateDisabledImage() =>
			_imageDisabled = _image?.SetOpacity((float) _disabledOpacity / 255);

		private (Size imageSize, Size textSize) measure(Graphics g)
		{
			var imageSize = Image?.Size ?? default;
			var textSize = string.IsNullOrEmpty(Text)
				? default
				: g.MeasureText(Text, Font, _infiniteSize, TextFormat);

			return (imageSize, textSize);
		}

		private void updateImage() =>
			Image = _buttonImages?.GetImage(Checked);


		public event EventHandler CheckedChanged;

		public event EventHandler PressDown;
		public event EventHandler Pressed;


		[Category("Settings"), DefaultValue("")]
		public override string Text
		{
			get => base.Text;
			set
			{
				base.Text = value;
				updateSize();
				Invalidate();
			}
		}

		private Bitmap _imageDisabled;
		private Bitmap _image;

		[Category("Settings"), DefaultValue(null)]
		public virtual Bitmap Image
		{
			get => _image;
			set
			{
				if (_image == value)
					return;

				_image = value;

				updateDisabledImage();
				updateSize();
				Invalidate();
			}
		}

		public override Color BackColor
		{
			get => base.BackColor;
			set
			{
				if (base.BackColor == value)
					return;

				base.BackColor = value;
				updateHighlightColors();
			}
		}


		private Color _mouseOverBackColor;
		private Color _checkedBackColor;
		private Color _disabledBorderColor;
		private Color _highlightBackColor = SystemColors.HotTrack;

		[Category("Settings"), DefaultValue(typeof(Color), "HotTrack")]
		public Color HighlightBackColor
		{
			get => _highlightBackColor;
			set
			{
				if (_highlightBackColor == value)
					return;

				_highlightBackColor = value;
				updateHighlightColors();
			}
		}

		private int _highlightMouseOverOpacity = 64;

		[DefaultValue(64), Category("Settings")]
		public virtual int HighlightMouseOverOpacity
		{
			get => _highlightMouseOverOpacity;
			set
			{
				if (_highlightMouseOverOpacity != value)
				{
					_highlightMouseOverOpacity = value;
					updateHighlightColors();
				}
			}
		}

		private int _highlightCheckedOpacity = 128;

		[DefaultValue(128), Category("Settings")]
		public virtual int HighlightCheckedOpacity
		{
			get => _highlightCheckedOpacity;
			set
			{
				if (_highlightCheckedOpacity != value)
				{
					_highlightCheckedOpacity = value;
					updateHighlightColors();
				}
			}
		}

		private Color _borderColor = SystemColors.ActiveBorder;

		[Category("Settings"), DefaultValue(typeof(Color), "ActiveBorder")]
		public Color BorderColor
		{
			get => _borderColor;
			set
			{
				if (_borderColor == value)
					return;

				_borderColor = value;
				updateDisabledBorder();
			}
		}

		[Category("Settings"), DefaultValue(typeof(Color), "GrayText")]
		public Color DisabledForeColor { get; set; } = SystemColors.GrayText;

		private int _disabledOpacity = 128;

		[DefaultValue(128), Category("Settings")]
		public virtual int DisabledOpacity
		{
			get => _disabledOpacity;
			set
			{
				if (_disabledOpacity != value)
				{
					_disabledOpacity = value;
					updateDisabledBorder();
					updateDisabledImage();
				}
			}
		}

		private bool _checked;

		[Category("Settings"), DefaultValue(false)]
		public bool Checked
		{
			get => _checked;
			set
			{
				if (_checked == value)
					return;

				_checked = value;
				updateImage();
				Invalidate();
				CheckedChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private bool _mouseOver;

		private bool MouseOver
		{
			get => _mouseOver;
			set
			{
				if (_mouseOver != value)
				{
					_mouseOver = value;
					Invalidate();
				}
			}
		}

		[Category("Settings"), DefaultValue(false)]
		public override bool AutoSize
		{
			get => base.AutoSize;
			set => base.AutoSize = value;
		}

		[Category("Settings"), DefaultValue(typeof(AnchorStyles), "None")]
		public virtual AnchorStyles VisibleBorders { get; set; } = AnchorStyles.None;

		[Category("Settings"), DefaultValue(true)]
		public bool PaintBackground { get; set; } = true;

		[Category("Settings"), DefaultValue(true)]
		public virtual bool AutoCheck { get; set; } = true;

		[Category("Settings"), DefaultValue(false)]
		public virtual bool? VisibleAllBorders
		{
			get
			{
				switch (VisibleBorders)
				{
					case ControlHelpers.AnchorAll:
						return true;
					case AnchorStyles.None:
						return false;
					default:
						return null;
				}
			}

			set
			{
				if (!value.HasValue)
					return;

				if (value.Value)
					VisibleBorders = ControlHelpers.AnchorAll;
				else
					VisibleBorders = AnchorStyles.None;
			}
		}

		[Category("Settings"), DefaultValue(typeof(DashStyle), "Solid")]
		public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

		private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
		[Category("Settings"), DefaultValue(typeof(TextImageRelation), "ImageBeforeText")]
		public virtual TextImageRelation TextImageRelation
		{
			get => _textImageRelation;
			set
			{
				if (_textImageRelation == value)
					return;

				_textImageRelation = value;
				updateSize();
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(typeof(StringAlignment), "Near")]
		public virtual StringAlignment TextAlign { get; set; } = StringAlignment.Near;

		private ButtonImages _buttonImages;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ButtonImages ButtonImages
		{
			get => _buttonImages;
			set
			{
				if (_buttonImages == value)
					return;

				_buttonImages = value;
				updateImage();
			}
		}

		private const TextFormatFlags TextFormat =
			TextFormatFlags.NoClipping |
			TextFormatFlags.NoPrefix |
			TextFormatFlags.VerticalCenter |
			TextFormatFlags.TextBoxControl;

		private static readonly Size _infiniteSize = new Size(int.MaxValue, int.MaxValue);
	}
}