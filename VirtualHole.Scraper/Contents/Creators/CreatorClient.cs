using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;

namespace VirtualHole.Scraper.Contents.Creators
{
	using DB;
	using DB.Contents.Creators;

	using DBCreatorClient = DB.Contents.Creators.CreatorClient;

	public class CreatorClient
	{
		private YouTubeScraperFactory youtubeScraperFactory => scraperClient.youtube;
		private ScraperClient scraperClient = null;

		private DBCreatorClient dbCreatorClient => dbClient.Contents.Creators;
		private VirtualHoleDBClient dbClient = null;

		public CreatorClient(ScraperClient scraperClient, VirtualHoleDBClient dbClient)
		{
			this.scraperClient = scraperClient;
			this.dbClient = dbClient;
		}

		public async Task<IEnumerable<Creator>> GetAllFromDBAsync(CancellationToken cancellationToken = default)
		{
			List<Creator> results = new List<Creator>();

			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(CreatorClient),
				"Start getting all creators from DB",
				"Finished getting all creators from DB")) {
				FindCreatorsStrictSettings findSettings = new FindCreatorsStrictSettings { IsAll = true };
				FindResults<Creator> findResults = await dbCreatorClient.FindCreatorsAsync(findSettings, cancellationToken);
				while(await findResults.MoveNextAsync(cancellationToken)) {
					results.AddRange(findResults.Current);
				}
			}

			return results;
		}

		public async Task WriteToDBAsync(IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			using(StopwatchScope stopwatchScope = new StopwatchScope(
				nameof(CreatorClient),
				"Start writing creators to DB",
				"Finished writing creators to DB")) {
				await dbCreatorClient.UpsertManyCreatorsAndDeleteDanglingAsync(creators, cancellationToken);
			}
		}
	}
}
