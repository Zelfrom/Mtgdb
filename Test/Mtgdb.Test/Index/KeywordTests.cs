﻿using System.Linq;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class KeywordTests : TestsBase
	{
		[OneTimeSetUp]
		public static void Setup()
		{
			LoadTranslations();
		}

		[Test]
		public void Card_keywords_contain_expected_value(
			[Values("LEA")] string setcode,
			[Values("Badlands")] string name,
			[Values(nameof(KeywordDefinitions.ManaCost))] string field,
			[Values(null)] string expectedValue)
		{
			var card = Repo.SetsByCode[setcode].CardsByName[name].First();
			var keywords = new CardKeywords();
			keywords.Parse(card);

			if (expectedValue == null)
			{
				Assert.That(keywords.KeywordsByProperty, Does.Not.ContainKey(field));
				return;
			}

			var values = keywords.KeywordsByProperty[field];

			Assert.That(values, Is.Not.Null);
			Assert.That(values, Does.Contain(expectedValue));
		}
	}
}