using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

using ScraperContentClient = VirtualHole.Scraper.Contents.ContentClient;
using ScraperCreatorClient = VirtualHole.Scraper.Creators.CreatorClient;

namespace VirtualHole.Scraper
{
	public class ScraperRootClient
	{
		public ScraperContentClient Contents { get; private set; } = null;
		public ScraperCreatorClient Creators { get; private set; } = null;

		public ScraperRootClient(ScraperClient scraperClient, VirtualHoleDBClient dbClient)
		{
			Contents = new ScraperContentClient(scraperClient, dbClient);
			Creators = new ScraperCreatorClient(scraperClient, dbClient);
		}

		public async Task WriteToVideosDBUsingCreatorsDBAsync(
			bool incremental = false, CancellationToken cancellationToken = default)
		{
			IEnumerable<Creator> creators = await Creators.GetAllFromDBAsync(cancellationToken);

			MLog.Log(nameof(ScraperRootClient), $"Found {creators.Count()} creators on the database...");

			List<Content> contents = new List<Content>();
			contents.AddRange(await Contents.ScrapeAsync(creators, incremental, cancellationToken));
			if(contents.Count > 0) { await Contents.WriteToDBAsync(contents, incremental, cancellationToken); }

			MLog.Log(nameof(ScraperRootClient), $"Wrote a total of {contents.Count} content to database, during this iteration!");
		}
	}
}
