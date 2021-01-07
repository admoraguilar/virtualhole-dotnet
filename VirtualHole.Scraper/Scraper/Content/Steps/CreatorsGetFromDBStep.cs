using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	public class CreatorsGetFromDBStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(CreatorsGetFromDBStep),
				"Start getting all creators from DB.",
				"Finished getting all creators from DB.")) {
				FindSettings findSettings = new FindSettings() {
					Filters = new List<FindFilter>() { new CreatorStrictFilter() }
				};
				FindResults<Creator> findResults = await Context.InDB.Creators.FindCreatorsAsync(findSettings);
				while(await findResults.MoveNextAsync()) {
					Context.InCreators.AddRange(findResults.Current);
				}
			}
		}
	}
}
