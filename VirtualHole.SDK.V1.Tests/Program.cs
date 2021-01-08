using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.SDK.V1.Tests
{
	class Program
	{
		private static async Task Main(string[] args)
		{
			VirtualHoleClient.Initialize();

			VirtualHoleDBClient dbClient = new VirtualHoleDBClient(
				"virtualhole-prod",
				"mongodb+srv://<username>:<password>@virtualhole-1.41hlb.mongodb.net/test",
				"editor", ".VirtualHole12321"
			);

			MLog.Log("Running tests...");

			using(new StopwatchScope("Test", "Start", "End")) {
				List<Creator> creators = await dbClient.Creators.FindAllAsync();
				MLog.Log(MLogLevel.Info, "Test", $"Creator Count: {creators.Count}");
				await dbClient.Creators.UpsertManyAsync(creators);
			}

			using(new StopwatchScope("Test", "Start", "End")) {
				List<Content> contents = await dbClient.Contents.FindAllAsync();
				MLog.Log(MLogLevel.Info, "Test", $"Content Count: {contents.Count}");
				await dbClient.Contents.UpsertManyAsync(contents);
			}

			Console.ReadLine();
		}
	}
}
