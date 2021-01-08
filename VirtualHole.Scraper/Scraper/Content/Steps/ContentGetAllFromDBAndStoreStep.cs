using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper
{
	public class ContentGetAllFromDBAndStoreStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(ContentGetAllFromDBAndStoreStep),
				"Start getting all contents from DB.",
				"Finished getting all contents from DB.")) {
				List<Content> contents = await Context.InDB.Contents.FindAllAsync();
				MLog.Log(nameof(ContentGetAllFromDBAndStoreStep), $"Content Count: {contents.Count}");
				ContentScraperUtilities.SaveToDisk(contents);
			}
		}
	}
}
