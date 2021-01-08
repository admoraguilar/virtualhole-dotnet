using System.Threading.Tasks;
using Midnight.Logs;
using Midnight.Pipeline;

namespace VirtualHole.Scraper
{
	public class CreatorWriteToDBStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			using(StopwatchScope stopwatchScope = new StopwatchScope(
				nameof(CreatorWriteToDBStep),
				"Start writing creators to DB.",
				"Finished writing creators to DB.")) {
				await Context.InDB.Creators.UpsertManyAsync(Context.InCreators);
			}
		}
	}
}
