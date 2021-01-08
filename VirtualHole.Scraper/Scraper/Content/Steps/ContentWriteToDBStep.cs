using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using Midnight.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper
{
	public class ContentWriteToDBStep : PipelineStep<ContentScraperContext>
	{
		public override async Task ExecuteAsync()
		{
			// TODO: Compare with previously scraped contents
			// so that only new and old ones are written to MongoDB
			// hence reducing load

			List<Content> prevContents = ContentScraperUtilities.LoadFromDisk();

			List<Content> toDeleteContents = prevContents.Except(Context.OutResults).ToList();
			List<Content> toUpdateContents = Context.OutResults.Except(prevContents).ToList();
			
			ContentScraperUtilities.SaveToDisk(Context.OutResults);

			using(StopwatchScope s = new StopwatchScope(
				nameof(ContentWriteToDBStep),
				"Running operations to content collection...",
				"Finished operations to content collection!")) {
				MLog.Log(nameof(ContentWriteToDBStep), $"Will delete: {toDeleteContents.Count}");
				MLog.Log(nameof(ContentWriteToDBStep), $"Will update: {toUpdateContents.Count}");

				await TaskExt.Timeout(DeleteAsync(), TimeSpan.FromMinutes(15));
				await TaskExt.Timeout(UpdateAsync(), TimeSpan.FromMinutes(15));
			}

			async Task DeleteAsync()
			{
				using(new StopwatchScope(nameof(ContentWriteToDBStep), $"Start deleting...", $"Finished deleting!")) {
					LogContentDetails(toDeleteContents);
					await Context.InDB.Contents.DeleteManyAsync(toDeleteContents);
				}
			}

			async Task UpdateAsync()
			{
				using(new StopwatchScope(nameof(ContentWriteToDBStep), $"Start updating...", $"Finished updating!")) {
					LogContentDetails(toUpdateContents);
					await Context.InDB.Contents.UpsertManyAsync(toUpdateContents);
				}
			}

			void LogContentDetails(IEnumerable<Content> contents)
			{
				foreach(Content content in contents) {
					MLog.Log(nameof(ContentWriteToDBStep), $"{content.SocialType}-{content.ContentType} | Creator: {content.Creator.Name} | Title: {content.Title}");
				}
			}
		}
	}
}
