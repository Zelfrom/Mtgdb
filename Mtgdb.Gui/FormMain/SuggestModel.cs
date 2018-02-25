﻿using System;
using System.Threading;
using Lucene.Net.Contrib;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;

namespace Mtgdb.Gui
{
	public class SuggestModel
	{
		private const int SuggestCount = 20;

		private readonly LuceneSpellchecker _spellchecker;

		private SearchStringState _searchState;
		public SearchStringState SearchStateCurrent { get; set; }

		private string _language;

		public event Action<IntellisenseSuggest, SearchStringState> Suggested;
		
		private readonly Thread _suggestThread;

		public Token Token { get; private set; }

		public SuggestModel(LuceneSearcher searcher)
		{
			_spellchecker = searcher.Spellchecker;
			_suggestThread = new Thread(_ => suggestThread());
		}

		private void suggestLoopIteration()
		{
			if (_spellchecker == null || SearchStateCurrent == null || isSuggestUpToDate())
				Thread.Sleep(100);
			else
				suggest();
		}

		private void suggest()
		{
			var searchState =_searchState = SearchStateCurrent;
			_language = Ui.LanguageController.Language;

			var suggest = _spellchecker.Suggest(searchState.Text, searchState.Caret, _language, SuggestCount);
			Token = suggest.Token;

			if (isSuggestUpToDate())
				Suggested?.Invoke(suggest, searchState);
		}

		private bool isSuggestUpToDate()
		{
			if (_language != Ui.LanguageController.Language)
				return false;

			if (_searchState == null && SearchStateCurrent == null)
				return true;

			if (_searchState == null || SearchStateCurrent == null)
				return false;

			if (_searchState.Text != SearchStateCurrent.Text)
				return false;
			
			if (_searchState.Caret != SearchStateCurrent.Caret)
				return false;

			return true;
		}

		public void StartSuggestThread()
		{
			_suggestThread.Start();
		}

		public void AbortSuggestThread()
		{
			_suggestThread.Abort();
		}

		private void suggestThread()
		{
			try
			{
				while (true)
					suggestLoopIteration();
			}
			catch (ThreadAbortException)
			{
			}
		}

		public UiModel Ui { get; set; }
	}
}