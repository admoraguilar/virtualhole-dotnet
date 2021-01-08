using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using Midnight.Pipeline;
using VirtualHole.DB;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	public class CreatorGetFromDBStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			using(StopwatchScope stopwatch = new StopwatchScope(
				nameof(CreatorGetFromDBStep),
				"Start getting all creators from DB.",
				"Finished getting all creators from DB.")) {
				FindSettings findSettings = new FindSettings() {
					Filters = new List<FindFilter>() { 
						new CreatorFilter() {
							IsCheckForIsGroup = true,
							IsGroup = false,
							IsHidden = false,
						},
						new CreatorStrictFilter() 
					}
				};
				FindResults<Creator> findResults = await Context.InDB.Creators.FindAsync(findSettings);
				Context.InCreators.AddRange(await findResults.ToListAsync());
			}
		}
	}
}
