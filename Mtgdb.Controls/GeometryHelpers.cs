using System;
using System.Drawing;
using System.Linq;

namespace Mtgdb.Controls
{
	public static class GeometryHelpers
	{
		public static Point Round(this PointF location)
		{
			return new Point((int) Math.Round(location.X), (int) Math.Round(location.Y));
		}

		public static Size Round(this SizeF size)
		{
			return new Size((int) Math.Round(size.Width), (int) Math.Round(size.Height));
		}

		public static int Round(this float value)
		{
			return (int) Math.Round(value);
		}

		public static SizeF MultiplyBy(this SizeF original, SizeF scale)
		{
			return new SizeF(
				original.Width * scale.Width,
				original.Height * scale.Height);
		}

		public static SizeF MultiplyBy(this Size original, SizeF scale)
		{
			var product = MultiplyBy((SizeF) original, scale);
			return product;
		}

		public static PointF MultiplyBy(this Point original, SizeF scale)
		{
			return new PointF(
				original.X * scale.Width,
				original.Y * scale.Height);
		}



		public static SizeF MultiplyBy(this SizeF size, float value)
		{
			return new SizeF(value * size.Width, value * size.Height);
		}

		public static SizeF ToPower(this SizeF size, float value)
		{
			return new SizeF((float) Math.Pow(size.Width, value), (float) Math.Pow(size.Height, value));
		}

		public static SizeF MultiplyBy(this Size size, float value)
		{
			return new SizeF(value * size.Width, value * size.Height);
		}

		public static Size MultiplyBy(this Size size, Size value)
		{
			return new Size(value.Width * size.Width, value.Height * size.Height);
		}

		public static Size MultiplyBy(this Point point, Size value)
		{
			return new Size(value.Width * point.X, value.Height * point.Y);
		}

		public static SizeF DivideBy(this Size size, SizeF value)
		{
			return new SizeF(size.Width / value.Width, size.Height / value.Height);
		}

		public static SizeF DivideBy(this SizeF size, SizeF value)
		{
			return new SizeF(size.Width / value.Width, size.Height / value.Height);
		}

		public static SizeF DivideBy(this SizeF size, float value)
		{
			return new SizeF(size.Width / value, size.Height / value);
		}

		public static float Max(this SizeF size)
		{
			return Math.Max(size.Width, size.Height);
		}

		public static float Min(this SizeF size)
		{
			return Math.Min(size.Width, size.Height);
		}

		public static Rectangle Plus(this Rectangle rect, Point offset)
		{
			rect.Offset(offset);
			return rect;
		}

		public static Point RightBottom(this Rectangle rect)
		{
			return new Point(rect.Right, rect.Bottom);
		}

		public static Point Plus(this Point left, Point right)
		{
			return new Point(left.X + right.X, left.Y + right.Y);
		}

		public static Point Plus(this Point left, Size right)
		{
			return new Point(left.X + right.Width, left.Y + right.Height);
		}

		public static Point Minus(this Point left, Point right)
		{
			return new Point(left.X - right.X, left.Y - right.Y);
		}

		public static Point Minus(this Point left, Size right)
		{
			return new Point(left.X - right.Width, left.Y - right.Height);
		}

		public static PointF Minus(this PointF left, SizeF right)
		{
			return new PointF(left.X - right.Width, left.Y - right.Height);
		}

		public static SizeF Plus(this SizeF left, SizeF right)
		{
			return new SizeF(left.Width + right.Width, left.Height + right.Height);
		}

		public static SizeF Plus(this SizeF left, float right)
		{
			return new SizeF(left.Width + right, left.Height + right);
		}

		public static Size Plus(this Size left, Size right)
		{
			return new Size(left.Width + right.Width, left.Height + right.Height);
		}

		public static Size Minus(this Size left, Size right)
		{
			return new Size(left.Width - right.Width, left.Height - right.Height);
		}

		public static Size ToSize(this Point value)
		{
			return new Size(value);
		}

		public static PointF MultiplyBy(this PointF location, float value)
		{
			return new PointF(value * location.X, value * location.Y);
		}

		public static int SizeInPixels(this Font font)
		{
			return ((int) (font.SizeInPoints * 96 / 72)).ByDpiWidth();
		}

		public static Size FitIn(this Size originalSize, Size viewPortSize)
		{
			var factor = Math.Min(
				(float) viewPortSize.Width / originalSize.Width,
				(float) viewPortSize.Height / originalSize.Height);

			var zoomed = new Size(
				(int) Math.Round(originalSize.Width * factor),
				(int) Math.Round(originalSize.Height * factor));

			return zoomed;
		}

		public static SizeF FitIn(this Size originalSize, SizeF viewPortSize)
		{
			var factor = Math.Min(
				viewPortSize.Width / originalSize.Width,
				viewPortSize.Height / originalSize.Height);

			var zoomed = new SizeF(
				originalSize.Width * factor,
				originalSize.Height * factor);

			return zoomed;
		}

		public static Rectangle FitIn(this Size original, Rectangle viewPort)
		{
			return new Rectangle(viewPort.Location, original.FitIn(viewPort.Size));
		}

		public static bool FitsIn(this Size value, Size size)
		{
			return value.Width <= size.Width && value.Height <= size.Height;
		}

		public static bool ContainsPoint(this Point[] poly, Point point)
		{
			var coef = poly
				.Skip(1)
				.Select((p, i) => (point.Y - poly[i].Y) * (p.X - poly[i].X) - (point.X - poly[i].X) * (p.Y - poly[i].Y))
				.ToList();

			if (coef.Any(p => p == 0))
				return true;

			for (int i = 1; i < coef.Count; i++)
			{
				if (coef[i] * coef[i - 1] < 0)
					return false;
			}

			return true;
		}

		public static Rectangle GetBoundingRectangle(this Point[] poly)
		{
			int left = poly.Min(_ => _.X);
			int top = poly.Min(_ => _.Y);
			int right = poly.Max(_ => _.X);
			int bottom = poly.Max(_ => _.Y);

			return new Rectangle(left, top, right - left, bottom - top);
		}

		public static Point ProjectTo(this Point desiredLocation, Rectangle rect)
		{
			int x = desiredLocation.X;
			int y = desiredLocation.Y;

			if (x < rect.Left)
				x = rect.Left;
			if (x > rect.Right - 1)
				x = rect.Right - 1;
			if (y < rect.Top)
				y = rect.Top;
			if (y > rect.Bottom - 1)
				y = rect.Bottom - 1;

			desiredLocation = new Point(x, y);
			return desiredLocation;
		}

		public static Rectangle Round(this RectangleF rect)
		{
			return new Rectangle(strongRound(rect.Left), strongRound(rect.Top), strongRound(rect.Width), strongRound(rect.Height));
		}

		private static int strongRound(double value)
		{
			return Math.Sign(value) * (int) (Math.Abs(value) + 0.5);
		}
	}
}