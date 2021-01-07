using System;
using System.Threading.Tasks;
using Midnight.Logs;

namespace VirtualHole.Scraper
{
	public class ContentWriteToDBStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			// TODO: Compare with previously scraped contents
			// so that only new and old ones are written to MongoDB
			// hence reducing load
			Task write = Context.InDB.Contents.UpsertManyContentsAsync(Context.OutResults);

			using(StopwatchScope s = new StopwatchScope(
				nameof(ContentWriteToDBStep),
				"Writing to content collection...",
				"Finished writing to content collection!")) {
				await TaskExtV.Timeout(write, TimeSpan.FromMinutes(15));
			}

			MLog.Log(nameof(DBClient), $"Wrote a total of {Context.OutResults.Count} content to database, during this iteration!");

			//Task WriteAsync()
			//{
			//	if(isIncremental) { return dbClient.Contents.UpsertManyContentsAsync(contents, cancellationToken); } 
			//	else { return dbClient.Contents.UpsertManyContentsAndDeleteDanglingAsync(contents, cancellationToken); }
			//}
		}
	}
}
