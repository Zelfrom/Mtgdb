using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public static class ControlHelpers
	{
		public const AnchorStyles AnchorAll =
			AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		public static int GetTrueIndexPositionFromPoint(this FixedRichTextBox rtb, System.Drawing.Point pt)
		{
			Point wpt = new Point(pt.X, pt.Y);
			int index = (int) SendMessage(new HandleRef(rtb, rtb.Handle), EmCharFromPos, 0, wpt);

			return index;
		}

		[DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, Point lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(System.Drawing.Point pt);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetForegroundWindow(HandleRef hWnd);

		public static Form ParentForm(this Control c)
		{
			var current = c;
			while (current.Parent != null)
				current = current.Parent;

			return current as Form;
		}

		public static bool Invoke(this Control value, Action method)
		{
			if (value.IsDisposed || value.Disposing || !value.IsHandleCreated)
				return false;

			try
			{
				value.Invoke(method);
			}
			catch (ObjectDisposedException)
			{
			}

			return true;
		}

		public static void TouchColorProperties(this Control control)
		{
			var color = control.BackColor;
			control.BackColor = Color.Black;
			control.BackColor = color;

			color = control.ForeColor;
			control.ForeColor = Color.Black;
			control.ForeColor = color;
		}

		public static List<T> Reorder<T>(this IList<T> originalArray, int fromIndex, int toIndex)
		{
			var copy = originalArray.ToList();

			if (fromIndex >= 0)
				copy.RemoveAt(fromIndex);

			if (toIndex >= 0)
				copy.Insert(toIndex, originalArray[fromIndex]);
			else
				copy.Add(originalArray[fromIndex]);

			return copy;
		}



		public static System.Drawing.Point PointToClient(this Control control, Control targetControl, System.Drawing.Point targetLocation)
		{
			targetLocation = targetControl.PointToScreen(targetLocation);
			targetLocation = control.PointToClient(targetLocation);
			return targetLocation;
		}


		public static void SetTag<TValue>(this Control control, string key, TValue value)
		{
			if (control.Tag == null)
				control.Tag = new Dictionary<string, object>();

			var dict = (Dictionary<string, object>) control.Tag;
			dict[key] = value;
		}

		public static void SetTag<TValue>(this Control control, TValue value) =>
			SetTag(control, typeof(TValue).FullName, value);

		public static TValue GetTag<TValue>(this Control control, string key)
		{
			if (control.Tag == null)
				control.Tag = new Dictionary<string, object>();

			var dict = (Dictionary<string, object>) control.Tag;

			if (!dict.TryGetValue(key, out var result))
				return default;

			return (TValue) result;
		}

		public static TValue GetTag<TValue>(this Control control) =>
			GetTag<TValue>(control, typeof(TValue).FullName);

		public static void PaintBorder(this Control c, Graphics graphics, AnchorStyles borders, Color borderColor, DashStyle dashStyle)
		{
			if (borderColor == Color.Transparent || borderColor == Color.Empty || borderColor.A == 0)
				return;

			var pen = new Pen(borderColor)
			{
				DashStyle = dashStyle
			};

			if ((borders & AnchorStyles.Top) > 0)
				graphics.DrawLine(pen, 0, 0, c.Width - 1, 0);

			if ((borders & AnchorStyles.Bottom) > 0)
				graphics.DrawLine(pen, 0, c.Height - 1, c.Width - 1, c.Height - 1);

			if ((borders & AnchorStyles.Left) > 0)
				graphics.DrawLine(pen, 0, 0, 0, c.Height - 1);

			if ((borders & AnchorStyles.Right) > 0)
				graphics.DrawLine(pen, c.Width - 1, 0, c.Width - 1, c.Height - 1);
		}

		public static void PaintPanelBack(this Control c, Graphics g, Rectangle clipRect, Image backImage, Color backColor, bool paintBack)
		{
			if (!paintBack || backColor.A < byte.MaxValue)
				ButtonRenderer.DrawParentBackground(g, clipRect, c);

			if (!paintBack && VisualStyleRenderer.IsSupported)
				return;

			if (backColor != Color.Empty && backColor != Color.Transparent && backColor.A > 0)
				g.FillRectangle(new SolidBrush(backColor), c.ClientRectangle);

			if (backImage != null)
				g.DrawImage(backImage, backImage.GetRect());
		}

		public static bool IsUnderMouse(this Control c) =>
			c.Handle.Equals(WindowFromPoint(Cursor.Position));

		public static bool IsChildUnderMouse(this Control c)
		{
			var position = Cursor.Position;
			var handle = WindowFromPoint(position);

			while (true)
			{
				var clientPosition = c.PointToClient(position);
				var child = c.GetChildAtPoint(clientPosition);

				if (child == null || child == c)
				{
					bool result = c.Handle.Equals(handle);
					return result;
				}

				c = child;
			}
		}

		public static bool TryCopyToClipboard(this string selectedText)
		{
			if (!string.IsNullOrEmpty(selectedText))
			{
				try
				{
					Clipboard.SetText(selectedText);
					return true;
				}
				catch (ExternalException)
				{
				}
			}

			return false;
		}

		private const int EmCharFromPos = 0x00D7;

		[StructLayout(LayoutKind.Sequential)]
		private class Point
		{
			[UsedImplicitly]
			public int x;

			[UsedImplicitly]
			public int y;

			public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}
	}
}