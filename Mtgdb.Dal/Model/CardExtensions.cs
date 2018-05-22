namespace Mtgdb.Dal
{
	public static class CardExtensions
	{
		public static int MaxCountInDeck(this Card c)
		{
			if (Str.Equals(c.Rarity, "Basic Land") || Str.Equals(c.NameEn, "Relentless Rats"))
				return int.MaxValue;

			return 4;
		}

		public static int MinCountInDeck(this Card c)
		{
			return 0;
		}
	}
}