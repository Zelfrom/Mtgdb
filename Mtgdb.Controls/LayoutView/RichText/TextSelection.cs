﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mtgdb.Controls
{
	public class TextSelection
	{
		public event Action<TextSelection> Changed;



		public TextSelection Clone()
		{
			return new TextSelection
			{
				Text = Text,
				Begin = Begin,
				End = End,
				IsCaretVisible = IsCaretVisible
			};
		}

		public void Clear()
		{
			Begin = -1;
			End = -1;

			Changed?.Invoke(this);
		}

		public void SetSelection(TextSelection copy)
		{
			Text = copy.Text;
			Begin = copy.Begin;
			End = copy.End;

			PrintedTokens.Clear();
			PrintedTokens.AddRange(copy.PrintedTokens);
			Changed?.Invoke(this);
		}

		public void SelectAll()
		{
			Begin = 0;
			End = Text.Length;

			Changed?.Invoke(this);
		}

		public void ShiftSelectionLeft()
		{
			if (IsEmpty)
				return;

			if (End == 0)
				return;

			End = PrintedTokens
				.Select(t => t.Token.Index)
				.Where(F.IsLessThan(End))
				.DefaultIfEmpty(0)
				.Last();

			Changed?.Invoke(this);
		}

		public void ShiftSelectionRight()
		{
			if (IsEmpty)
				return;

			if (End >= Text.Length)
				return;

			End = PrintedTokens
				.Select(t => t.Token.Index + t.Token.Length)
				.Where(F.IsGreaterThan(End))
				.DefaultIfEmpty(Text.Length)
				.First();

			Changed?.Invoke(this);
		}

		public void ShiftSelectionUp()
		{
			if (IsEmpty)
				return;

			if (End == 0)
				return;

			var rect = PrintedTokens
				.Where(t => t.Token.Index + t.Token.Length <= End)
				.Select(t => t.Rect)
				.DefaultIfEmpty(RectangleF.Empty)
				.Last();

			if (rect.IsEmpty)
				return;

			var tokenAbove = getTokenAbove(new PointF(rect.Right, rect.Top));
			End = tokenAbove?.Index ?? 0;

			Changed?.Invoke(this);
		}

		public void ShiftSelectionDown()
		{
			if (IsEmpty)
				return;

			if (End >= Text.Length)
				return;

			var rect = PrintedTokens
				.Where(t => t.Token.Index <= End)
				.Select(t => t.Rect)
				.DefaultIfEmpty(RectangleF.Empty)
				.Last();

			if (rect.IsEmpty)
				return;

			var tokenBelow = getTokenBelow(rect.Location);
			End = tokenBelow?.Right ?? Text.Length;

			Changed?.Invoke(this);
		}

		public void ShiftSelectionToStart()
		{
			if (IsEmpty)
				return;

			if (End == 0)
				return;

			End = 0;

			Changed?.Invoke(this);
		}

		public void ShiftSelectionToEnd()
		{
			if (IsEmpty)
				return;

			if (End >= Text.Length)
				return;

			End = Text.Length;

			Changed?.Invoke(this);
		}

		public void MoveSelectionDown()
		{
			if (IsEmpty)
				return;

			var rect = PrintedTokens
				.Where(t => t.Token.Index <= End)
				.Select(t => t.Rect)
				.DefaultIfEmpty(RectangleF.Empty)
				.Last();

			if (rect.IsEmpty)
				return;

			var tokenBelow = getTokenBelow(rect.Location);
			Begin = End =
				tokenBelow?.Right ?? Text.Length;

			Changed?.Invoke(this);
		}

		public void MoveSelectionUp()
		{
			if (IsEmpty)
				return;

			var rect = PrintedTokens
				.Where(t => t.Token.Index + t.Token.Length <= End)
				.Select(t => t.Rect)
				.DefaultIfEmpty(RectangleF.Empty)
				.Last();

			if (rect.IsEmpty)
				return;

			var tokenAbove = getTokenAbove(new PointF(rect.Right, rect.Top));
			Begin = End =
				tokenAbove?.Index ?? 0;

			Changed?.Invoke(this);
		}

		public void MoveSelectionRight()
		{
			if (IsEmpty)
				return;

			Begin = End =
				PrintedTokens
					.Select(t => t.Token.Index + t.Token.Length)
					.Where(F.IsGreaterThan(End))
					.DefaultIfEmpty(Text.Length)
					.First();

			Changed?.Invoke(this);
		}

		public void MoveSelectionLeft()
		{
			if (IsEmpty)
				return;

			Begin = End =
				PrintedTokens
					.Select(t => t.Token.Index)
					.Where(F.IsLessThan(End))
					.DefaultIfEmpty(0)
					.Last();

			Changed?.Invoke(this);
		}

		private RichTextToken getTokenAbove(PointF point)
		{
			for (int i = 0; i < PrintedTokens.Count; i++)
			{
				if (PrintedTokens[i].Rect.Y < point.Y)
					continue;

				if (i == 0)
					return null;

				int jMax = i - 1;
				var y = PrintedTokens[jMax].Rect.Y;

				for (int j = jMax; j >= 0; j--)
				{
					var rect = PrintedTokens[j].Rect;

					if (rect.Right <= point.X || rect.Y < y)
						return PrintedTokens[Math.Min(j + 1, jMax)].Token;
				}

				return null;
			}

			return null;
		}

		private RichTextToken getTokenBelow(PointF point)
		{
			for (int i = 0; i < PrintedTokens.Count; i++)
			{
				if (PrintedTokens[i].Rect.Y <= point.Y)
					continue;

				var jMin = i + 1;
				var y = PrintedTokens[jMin].Rect.Y;

				for (int j = jMin; j < PrintedTokens.Count; j++)
				{
					var rect = PrintedTokens[j].Rect;

					if (rect.X > point.X || rect.Y > y)
						return PrintedTokens[Math.Max(j - 1, jMin)].Token;
				}

				return null;
			}

			return null;
		}

		public void Tick()
		{
			IsCaretVisible = !IsCaretVisible;
			Changed?.Invoke(this);
		}

		public void Hide()
		{
			if (!IsCaretVisible)
				return;

			IsCaretVisible = false;
			Changed?.Invoke(this);
		}

		public bool IsCaretVisible { get; private set; }

		public string SelectedText =>
			Start < 0
				? null
				: Text.Substring(Start, Length);

		public string Text { get; set; }

		public int Begin { get; set; }
		public int End { get; set; }

		public int Start => Math.Min(Begin, End);
		public int Length => Math.Abs(Begin - End);
		public bool IsRightToLeft => End < Begin;

		public List<(RichTextToken Token, RectangleF Rect)> PrintedTokens { get; }
			= new List<(RichTextToken Token, RectangleF Rect)>();

		public bool IsEmpty => Begin < 0;
	}
}