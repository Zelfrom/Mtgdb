using System.Threading.Tasks;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class GuiLoader
	{
		[UsedImplicitly] // by ninject
		public GuiLoader(
			Loader loader,
			CardRepository repo,
			NewsService newsService,
			DownloaderSubsystem downloaderSubsystem,
			DeckListModel deckListModel,
			DeckSearcher deckSearcher,
			DeckIndexUpdateSubsystem deckIndexUpdateSubsystem)
		{
			_loader = loader;
			_repo = repo;

			_loader.AddAction(newsService.FetchNews);
			_loader.AddAction(downloaderSubsystem.CalculateProgress);
			_loader.AddTask(async () =>
			{
				deckListModel.Load();

				if (deckSearcher.IsIndexSaved)
					deckSearcher.LoadIndexes();
				else
				{
					while (!_repo.IsPriceLoadingComplete)
						await TaskEx.Delay(100);

					deckSearcher.LoadIndexes();
				}
			});
		}

		public void Run() =>
			_loader.Run();

		private readonly Loader _loader;
		private readonly CardRepository _repo;
	}
}