using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Contents.Creators;

namespace VirtualHole.Scraper.Contents.Creators
{
	public class CreatorClient
	{
		private ScraperClient scraperClient = null;
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
				FindResults<Creator> findResults = await dbClient.Contents.Creators.FindCreatorsAsync(findSettings, cancellationToken);
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
				await dbClient.Contents.Creators.UpsertManyCreatorsAndDeleteDanglingAsync(creators, cancellationToken);
			}
		}
	}
}
