using System.IO;
using System.Threading.Tasks;
using Midnight;
using Midnight.Logs;
using Newtonsoft.Json;

namespace VirtualHole.Scraper
{
	public class ContentSaveAsJsonToDiskStep : PipelineStep<ContentScraperContext>
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
			File.WriteAllText(contentsPath, JsonConvert.SerializeObject(Context.OutAllResults));
			MLog.Log(nameof(ContentSaveAsJsonToDiskStep), $"Saved contents.json to {contentsPath}.");

			return Task.CompletedTask;
		}
	}
}
