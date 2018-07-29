﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Ui;
using Newtonsoft.Json;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class DeckListModel
	{
		public event Action Changed;

		static DeckListModel()
		{
			_serializer = new JsonSerializer();

			_serializer.Converters.Add(
				new UnformattedJsonConverter(type =>
					typeof(IEnumerable<int>).IsAssignableFrom(type)));
		}

		[UsedImplicitly]
		public DeckListModel(
			CardRepository repo,
			IDeckTransformation transformation,
			CollectionEditorModel collection)
		{
			_repo = repo;
			_transformation = transformation;
			_collectionEditor = collection;

			_collectionEditor.CollectionChanged += collectionChanged;
			_state.Collection = new CollectionSnapshot(_collectionEditor);
			_repo.PriceLoadingComplete += priceLoadingComplete;
		}

		private void priceLoadingComplete()
		{
			lock (_syncModels)
				foreach (var model in _deckModels)
				{
					model.FillCardNames();
					model.ClearCaches();
				}
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card)
		{
			if (!listChanged && !countChanged)
				return;

			ThreadPool.QueueUserWorkItem(_ =>
			{
				_abort = true;
				lock (_syncCollection)
				{
					_abort = false;

					var snapshot = new CollectionSnapshot(_collectionEditor);

					var affectedCardIds = snapshot.GetAffectedCardIds(_state.Collection);

					if (affectedCardIds.Count == 0)
						return;

					var affectedNames = affectedCardIds
						.Select(id => _repo.CardsById[id].NameEn)
						.ToHashSet(Str.Comparer);

					lock (_syncModels)
						foreach (var model in _deckModels)
						{
							if (_abort)
								return;

							if (model.MayContainCardNames(affectedNames))
								model.Collection = snapshot;
						}

					_state.Collection = snapshot;
					Save();
				}
			});
		}

		public bool Add(Deck deck)
		{
			var duplicate = findDupliate(deck);
			if (duplicate != null)
			{
				duplicate.Saved = deck.Saved;
				return false;
			}

			var model = CreateModel(deck);
			var index = _deckModels.Count;

			lock (_syncModels)
			{
				deck.Id = Interlocked.Increment(ref _state.Id);
				_deckModels.Add(model);
				_indexByDeck.Add(model, index);
				_decksByName.Add(deck.Name, model);
			}

			return true;
		}

		public DeckModel CreateModel(Deck deck) =>
			new DeckModel(deck, _repo, _state.Collection, _transformation);

		public void Remove(DeckModel deck)
		{
			lock (_syncModels)
			{
				_deckModels.RemoveAt(_indexByDeck[deck]);
				_indexByDeck.Remove(deck);
				_decksByName.Remove(deck.Name, deck);
			}
		}

		public void Rename(DeckModel deck, string name)
		{
			lock (_syncModels)
			{
				_decksByName.Remove(deck.Name, deck);

				deck.Name = name;
				var duplicate = findDupliate(deck.OriginalDeck);
				if (duplicate != null)
					duplicate.Saved = deck.Saved;
				else
					_decksByName.Add(deck.Name, deck);
			}
		}

		public IReadOnlyList<DeckModel> GetModelCopies()
		{
			lock (_syncModels)
				return _state.Decks.Select(CreateModel).ToReadOnlyList();
		}

		public IEnumerable<DeckModel> GetModels()
		{
			lock (_syncModels)
				foreach (var model in _deckModels)
					yield return model;
		}


		public void Save()
		{
			Changed?.Invoke();

			var serialized = new StringBuilder();

			lock (_syncModels)
			{
				_state.Decks = _deckModels.Select(_ => _.OriginalDeck).ToList();
				
				using(var writer = new StringWriter(serialized))
				using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
					_serializer.Serialize(jsonWriter, _state);
			}
			
			File.WriteAllText(_fileName, serialized.ToString());
		}

		public void Load()
		{
			if (File.Exists(_fileName))
			{
				string serialized = File.ReadAllText(_fileName);

				var deserialized = deserialize(serialized);

				lock (_syncModels)
				{
					_state = deserialized;

					_deckModels = _state.Decks
						.Select(d => new DeckModel(d, _repo, _collectionEditor, _transformation))
						.ToList();

					_decksByName = _deckModels.ToMultiDictionary(_ => _.Name, Str.Comparer);

					_indexByDeck = Enumerable.Range(0, _deckModels.Count)
						.ToDictionary(i => _deckModels[i]);
				}
			}

			IsLoaded = true;
			Loaded?.Invoke();
		}

		private State deserialize(string serialized)
		{
			try
			{
				return JsonConvert.DeserializeObject<State>(serialized);
			}
			catch (JsonException)
			{
				var decks = JsonConvert.DeserializeObject<List<Deck>>(serialized);
				return new State
				{
					Collection = _state.Collection,
					Decks = decks,
					IdCounter = decks.Max(_=>_.Id)
				};
			}
		}

		public bool IsLoaded { get; private set; }
		public event Action Loaded;

		public void TransformDecks(Func<bool> interrupt)
		{
			if (!IsLoaded)
				return;

			lock (_syncModels)
			{
				var count = _deckModels.Count;

				for (int i = 0; i < count; i++)
				{
					if (interrupt())
						return;

					_deckModels[i].UpdateTransformedDeck();
				}
			}
		}

		private DeckModel findDupliate(Deck deck)
		{
			if (!_decksByName.TryGetValues(deck.Name, out var decks))
				return null;

			var duplicate = decks.FirstOrDefault(_ => _.IsEquivalentTo(deck));
			return duplicate;
		}

		private List<DeckModel> _deckModels = new List<DeckModel>();

		private MultiDictionary<string, DeckModel> _decksByName =
			new MultiDictionary<string, DeckModel>(Str.Comparer);

		private Dictionary<DeckModel, int> _indexByDeck =
			new Dictionary<DeckModel, int>();

		private readonly CardRepository _repo;
		private readonly IDeckTransformation _transformation;
		private readonly CollectionEditorModel _collectionEditor;

		private static readonly string _fileName = AppDir.History.AddPath("decks.json");
		private State _state = new State();

		private readonly object _syncCollection = new object();
		private readonly object _syncModels = new object();
		private bool _abort;

		public class State
		{
			[JsonIgnore]
			public long Id;

			public long IdCounter
			{
				get => Id;
				set => Id = value;
			}
			public CollectionSnapshot Collection { get; set; }
			public List<Deck> Decks { get; set; } = new List<Deck>();
		}

		private static readonly JsonSerializer _serializer;
	}
}