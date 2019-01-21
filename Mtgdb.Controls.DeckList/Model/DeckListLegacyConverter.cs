﻿using System.IO;
using System.Linq;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckListLegacyConverter
	{
		public DeckListLegacyConverter(DeckListModel model, DeckConverter deckConverter)
		{
			_model = model;
			_deckConverter = deckConverter;

			if (File.Exists(_model.FileName))
				return;

			if (File.Exists(_v3FileName))
				IsV3ConversionRequired = true;
			else if (File.Exists(_v2FileName))
				IsV2ConversionRequired = true;
			else if (File.Exists(_legacyFileName))
				IsLegacyConversionRequired = true;
		}

		public void ConvertLegacyList()
		{
			string legacyFileContent = File.ReadAllText(_legacyFileName);
			var deserialized = _model.Deserialize(legacyFileContent);

			deserialized.Decks = deserialized.Decks
				.Select(_deckConverter.ConvertLegacyDeck)
				.ToList();

			deserialized.Collection = deserialized.Collection?.Invoke0(convertLegacyCollection);

			var serialized = _model.Serialize(deserialized);
			File.WriteAllText(_model.FileName, serialized);

			IsConversionCompleted = true;
		}

		private CollectionSnapshot convertLegacyCollection(CollectionSnapshot collection)
		{
			var deck = Deck.Create(collection.CountById, collection.CountById.Keys.ToList(), null, null);
			var converted = _deckConverter.ConvertLegacyDeck(deck);

			return new CollectionSnapshot
			{
				CountById = converted.MainDeck.Count
			};
		}

		public void ConvertV2List()
		{
			string v2FileContent = File.ReadAllText(_v2FileName);
			var deserialized = _model.Deserialize(v2FileContent);

			deserialized.Decks = deserialized.Decks
				.Select(_deckConverter.ConvertV2Deck)
				.ToList();

			deserialized.Collection = deserialized.Collection?.Invoke0(convertV2Collection);

			var serialized = _model.Serialize(deserialized);
			File.WriteAllText(_model.FileName, serialized);

			IsConversionCompleted = true;
		}

		private CollectionSnapshot convertV2Collection(CollectionSnapshot collection)
		{
			var deck = Deck.Create(collection.CountById, collection.CountById.Keys.ToList(), null, null);
			var converted = _deckConverter.ConvertV2Deck(deck);

			return new CollectionSnapshot
			{
				CountById = converted.MainDeck.Count
			};
		}

		public void ConvertV3List()
		{
			string v3FileContent = File.ReadAllText(_v3FileName);
			var deserialized = _model.Deserialize(v3FileContent);

			deserialized.Decks = deserialized.Decks
				.Select(_deckConverter.ConvertV3Deck)
				.ToList();

			deserialized.Collection = deserialized.Collection?.Invoke0(convertV3Collection);

			var serialized = _model.Serialize(deserialized);
			File.WriteAllText(_model.FileName, serialized);

			IsConversionCompleted = true;
		}

		private CollectionSnapshot convertV3Collection(CollectionSnapshot collection)
		{
			var deck = Deck.Create(collection.CountById, collection.CountById.Keys.ToList(), null, null);
			var converted = _deckConverter.ConvertV3Deck(deck);

			return new CollectionSnapshot
			{
				CountById = converted.MainDeck.Count
			};
		}

		public bool IsLegacyConversionRequired { get; }
		public bool IsV2ConversionRequired { get; }
		public bool IsV3ConversionRequired { get; }

		public bool IsConversionCompleted { get; private set; }



		private static readonly string _legacyFileName = AppDir.History.AddPath("decks.json");
		private static readonly string _v2FileName = AppDir.History.AddPath("decks.v2.json");
		private static readonly string _v3FileName = AppDir.History.AddPath("decks.v3.json");

		private readonly DeckListModel _model;
		private readonly DeckConverter _deckConverter;
	}
}