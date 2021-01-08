using System;
using System.Threading.Tasks;
using Midnight.Logs;
using Midnight.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;
using System.Collections.Generic;

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
				await TaskExt.Timeout(WriteAsync(), TimeSpan.FromMinutes(15));
			}

			MLog.Log(nameof(ContentWriteToDBStep), $"Wrote a total of {Context.OutNewResults.Count} content to database, during this iteration!");

			async Task WriteAsync()
			{
				MLog.Log(nameof(ContentWriteToDBStep), $"Will write new content: {Context.OutNewResults.Count}...");
				MLog.Log(nameof(ContentWriteToDBStep), $"Will deleting non-existent content: {Context.OutDeletedResults.Count}...");

				MLog.Log(nameof(ContentWriteToDBStep), "Writing...");
				LogContentsTitle(Context.OutNewResults);
				await Context.InDB.Contents.UpsertManyAsync(Context.OutNewResults);

				MLog.Log(nameof(ContentWriteToDBStep), "Deleting...");
				LogContentsTitle(Context.OutDeletedResults);
				await Context.InDB.Contents.DeleteManyAsync(Context.OutDeletedResults);

				void LogContentsTitle(IEnumerable<Content> contents)
				{
					foreach(Content content in contents) {
						MLog.Log(nameof(ContentWriteToDBStep), $"Social: {content.SocialType} | Type: {content.ContentType} | Title: {content.Title}");
					}
				}
			}
		}
	}
}
