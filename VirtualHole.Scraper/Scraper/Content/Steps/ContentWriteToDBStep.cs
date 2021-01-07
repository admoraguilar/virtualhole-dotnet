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

			using(StopwatchScope s = new StopwatchScope(
				nameof(ContentWriteToDBStep),
				"Writing to content collection...",
				"Finished writing to content collection!")) {
				await TaskExtV.Timeout(WriteAsync(), TimeSpan.FromMinutes(15));
			}

			MLog.Log(nameof(ContentWriteToDBStep), $"Wrote a total of {Context.OutNewResults.Count} content to database, during this iteration!");

			async Task WriteAsync()
			{
				MLog.Log(nameof(ContentWriteToDBStep), $"Will write new content: {Context.OutNewResults.Count}...");
				MLog.Log(nameof(ContentWriteToDBStep), $"Will deleting non-existent content: {Context.OutDeletedResults.Count}...");

				MLog.Log(nameof(ContentWriteToDBStep), "Writing...");
				await Context.InDB.Contents.UpsertManyContentsAsync(Context.OutNewResults);

				MLog.Log(nameof(ContentWriteToDBStep), "Deleting...");
				await Context.InDB.Contents.DeleteManyAsync(Context.OutDeletedResults);
			}
		}
	}
}
