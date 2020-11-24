using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Contents.Videos;
using VirtualHole.DB.Contents.Creators;

using ScraperVideoClient = VirtualHole.Scraper.Contents.Videos.VideoClient;
using ScraperCreatorClient = VirtualHole.Scraper.Contents.Creators.CreatorClient;

namespace VirtualHole.Scraper
{
	public class ContentClient
	{
		public ScraperVideoClient Videos { get; private set; } = null;
		public ScraperCreatorClient Creators { get; private set; } = null;

		public ContentClient(ScraperClient scraperClient, VirtualHoleDBClient dbClient)
		{
			Videos = new ScraperVideoClient(scraperClient, dbClient);
			Creators = new ScraperCreatorClient(scraperClient, dbClient);
		}

		public async Task WriteToVideosDBUsingCreatorsDBAsync(
			bool incremental = false, CancellationToken cancellationToken = default)
		{
			IEnumerable<Creator> creators = await Creators.GetAllFromDBAsync(cancellationToken);

			MLog.Log(nameof(ContentClient), $"Found {creators.Count()} creators on the database...");

			List<Video> videos = new List<Video>();
			videos.AddRange(await Videos.ScrapeAsync(creators, incremental, cancellationToken));
			if(videos.Count > 0) { await Videos.WriteToDBAsync(videos, incremental, cancellationToken); }

			MLog.Log(nameof(ContentClient), $"Wrote a total of {videos.Count} videos to database, during this iteration!");
		}
	}
}
