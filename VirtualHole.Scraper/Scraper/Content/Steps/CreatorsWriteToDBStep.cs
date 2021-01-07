using System.Threading.Tasks;
using Midnight.Logs;
using Midnight.Pipeline;

namespace VirtualHole.Scraper
{
	public class CreatorsWriteToDBStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			using(StopwatchScope stopwatchScope = new StopwatchScope(
				nameof(CreatorsWriteToDBStep),
				"Start writing creators to DB.",
				"Finished writing creators to DB.")) {
				await Context.InDB.Creators.UpsertManyCreatorsAndDeleteDanglingAsync(Context.InCreators);
			}
		}
	}
}
