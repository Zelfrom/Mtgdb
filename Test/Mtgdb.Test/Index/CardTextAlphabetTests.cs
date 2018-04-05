﻿using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class CardTextAlphabetTests : TestsBase
	{
		[OneTimeSetUp]
		public static void Setup()
		{
			LoadTranslations();
		}

		[Test]
		public void All_symbols_in_card_texts_are_considered_in_code()
		{
			var alphabet = new HashSet<char>();
				
			var languages = new HashSet<string>(CardLocalization.GetAllLanguages(), Str.Comparer);
			languages.Remove("cn");
			languages.Remove("tw");
			languages.Remove("jp");
			languages.Remove("kr");

			foreach (var set in Repo.SetsByCode.Values)
			{
				alphabet.UnionWith(set.Name);
				alphabet.UnionWith(set.Code);
			}

			foreach (var card in Repo.Cards)
			{
				alphabet.UnionWithNullable(card.NameEn);
				alphabet.UnionWithNullable(card.TypeEn);
				alphabet.UnionWithNullable(card.FlavorEn);
				alphabet.UnionWithNullable(card.TextEn);
				alphabet.UnionWithNullable(card.Artist);

				foreach (string lang in languages)
				{
					alphabet.UnionWithNullable(card.GetName(lang));
					alphabet.UnionWithNullable(card.GetType(lang));
					alphabet.UnionWithNullable(card.GetFlavor(lang));
					alphabet.UnionWithNullable(card.GetText(lang));
				}
			}

			var chars = alphabet.Select(char.ToLowerInvariant).Distinct().OrderBy(c => c).ToArray();

			Log.Info(() => new string(chars));

			var specialChars = new List<char>();

			var latin = new HashSet<char>("abcdefghijklmnopqrstuvwxyz");
			var cyrillic = new HashSet<char>("абвгдежзийклмнопрстуфхцчшщьыъэюя");
			var numbers = new HashSet<char>("01234567890");
			var knownSpecialChars = new HashSet<char>("ºß");

			foreach (char c in chars)
			{
				if (latin.Contains(c))
					continue;

				if (cyrillic.Contains(c))
					continue;

				if (numbers.Contains(c))
					continue;

				if (c == '\n')
					continue;

				if (c == '\r')
					continue;

				if (MtgdbTokenizerPatterns.Replacements.ContainsKey(c))
					continue;

				if (MtgdbTokenizerPatterns.WordCharsSet.Contains(c))
					continue;

				if (MtgdbTokenizerPatterns.SingletoneWordChars.Contains(c))
					continue;

				if (knownSpecialChars.Contains(c))
					continue;

				specialChars.Add(c);
			}

			var specialCharsStr = new string(specialChars.ToArray());
			Log.Debug(specialCharsStr);

			var notConsideredChars = new string(specialCharsStr.Where(char.IsLetterOrDigit).ToArray());
			Assert.That(notConsideredChars, Is.Empty);
		}
	}
}