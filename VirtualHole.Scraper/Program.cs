using System;
using Newtonsoft.Json;
using Midnight;
using Midnight.Logs;

namespace VirtualHole.Scraper
{

	internal class Program
	{
		private static void Main(string[] args)
		{
			// We do this as we're dealing with camel case convention JSON files
			// which is not aligned with C#'s naming convention
			JsonConvert.DefaultSettings = () => JsonUtilities.SerializerSettings.DefaultCamelCase;

			MLog.Log("Running scraper...");

			VirtualHoleScraperRunner runner = new VirtualHoleScraperRunner();
			runner.Run();

			Console.ReadLine();
		}
	}
}
