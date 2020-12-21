using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using System;

namespace VirtualHole.Scraper.Creators
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
				"Start getting all creators from DB.",
				"Finished getting all creators from DB.")) {
				CreatorsStrictFilter findSettings = new CreatorsStrictFilter { };
				FindResults<Creator> findResults = await dbClient.Creators.FindCreatorsAsync(findSettings, cancellationToken);
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
				"Start writing creators to DB.",
				"Finished writing creators to DB.")) {
				await dbClient.Creators.UpsertManyCreatorsAndDeleteDanglingAsync(creators, cancellationToken);
			}
		}
	}
}
