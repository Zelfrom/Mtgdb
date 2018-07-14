using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Store;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardSpellchecker : LuceneSpellchecker<int, Card>
	{
		// 0.39 bbd
		private const string IndexVerision = "0.39";

		public CardSpellchecker(CardRepository repo, CardDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			_repo = repo;
		}



		protected override IEnumerable<Card> GetObjectsToIndex()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException();

			return _repo.SetsByCode.Values
				.Where(FilterSet)
				.SelectMany(s => s.Cards);
		}

		protected override Directory LoadSpellcheckerIndex()
		{
			Directory index;

			if (_version.IsUpToDate)
			{
				index = FSDirectory.Open(_version.Directory);
				Spellchecker.Load(index);
				return index;
			}

			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			_version.CreateDirectory();

			index = base.LoadSpellcheckerIndex();

			index.SaveTo(_version.Directory);
			_version.SetIsUpToDate();

			return index;
		}

		public void InvalidateIndex() =>
			_version.Invalidate();



		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}

		public string IndexDirectory => _version.Directory;
		public bool IsUpToDate => _version.IsUpToDate;
		public Func<Set, bool> FilterSet { get; set; } = set => true;

		private IndexVersion _version;
		private readonly CardRepository _repo;
	}
}