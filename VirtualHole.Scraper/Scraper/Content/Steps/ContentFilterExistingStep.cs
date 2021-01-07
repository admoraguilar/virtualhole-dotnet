using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Midnight;
using Midnight.Logs;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper
{
	public class ContentFilterExistingStep : PipelineStep<ContentScraperContext>
	{
		public override Task ExecuteAsync()
		{
			// TODO:
			// * Separate two contents.json: contents.json, contents-prev.json
			// * Do a comparison and divide it by: OutNewResults, OutDeleteResults
			// * Upsert the OutNewResults to DB
			// * Delete the OutDeleteResults on DB
			string contentsPath = Path.Combine(PathUtilities.GetApplicationPath(), "data/results/contents.json");

			string contentsDirectory = Path.GetDirectoryName(contentsPath);
			Directory.CreateDirectory(contentsDirectory);

			if(File.Exists(contentsPath)) {
				MLog.Log("Found previous scraped content...");

				string prevContentsJson = File.ReadAllText(contentsPath);
				List<Content> prevContents = JsonConvert.DeserializeObject<List<Content>>(prevContentsJson);

				Context.OutNewResults = Context.OutAllResults.Except(prevContents).ToList();
				Context.OutDeletedResults = prevContents.Except(Context.OutAllResults).ToList();

				File.Delete(contentsPath);
			} else {
				MLog.Log("No previous scraped content found...");

				Context.OutNewResults = Context.OutAllResults;
			}

			File.WriteAllText(contentsPath, JsonConvert.SerializeObject(Context.OutAllResults));
			MLog.Log(nameof(ContentFilterExistingStep), $"Saved contents.json to {contentsPath}.");

			return Task.CompletedTask;
		}
	}
}
