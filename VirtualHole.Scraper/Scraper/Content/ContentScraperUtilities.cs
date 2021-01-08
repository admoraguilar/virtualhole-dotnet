using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Midnight;
using Midnight.Logs;
using VirtualHole.DB.Contents;

namespace VirtualHole.Scraper
{
	public static class ContentScraperUtilities
	{
		public static string ContentsPath => Path.Combine(PathUtilities.GetApplicationPath(), "data/results/contents.json");

		public static List<Content> LoadFromDisk()
		{
			if(File.Exists(ContentsPath)) {
				string prevContentsJson = File.ReadAllText(ContentsPath);
				return JsonConvert.DeserializeObject<List<Content>>(prevContentsJson);
			} else {
				MLog.Log(nameof(ContentScraperUtilities), $"No contents.json found at {ContentsPath}.");
				return new List<Content>();
			}
		}

		public static void SaveToDisk(IEnumerable<Content> contents)
		{
			string contentsDirectory = Path.GetDirectoryName(ContentsPath);
			Directory.CreateDirectory(contentsDirectory);

			if(File.Exists(ContentsPath)) {
				File.Delete(ContentsPath);
			}

			File.WriteAllText(ContentsPath, JsonConvert.SerializeObject(contents));
			MLog.Log(nameof(ContentScraperUtilities), $"Saved contents.json to {ContentsPath}.");

		}
	}
}
