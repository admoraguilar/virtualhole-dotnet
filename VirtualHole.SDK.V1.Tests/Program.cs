using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.SDK.V1.Tests
{
	class Program
	{
		private static void CreatorTest()
		{
			Creator creator = new Creator() {
				Socials = new List<CreatorSocial> {
					new YouTubeSocial {
						SubscribersCount = 1203,
						TotalViewsCount = 2320410
					},
					new TwitterSocial {
						FollowersCount = 302030
					}
				}
			};

			string json = JsonConvert.SerializeObject(creator, new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				Formatting = Formatting.Indented
			});
			Console.WriteLine(json);

			Creator deserializedJson = JsonConvert.DeserializeObject<Creator>(json, new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				Converters = new JsonConverter[] {
					new CreatorSocialConverter()
				}
			});
			Console.WriteLine("Done");
		}

		private static void ContentTest()
		{
			List<Content> contents = new List<Content> {
				new YouTubeVideo() {
					Title = "My first video.",
					ViewsCount = 219123,
					LikesCount = 123138,
					DislikesCount = 390
				},
				new TwitterTweet() {
					Title = "My first tweet.",
					HeartsCount = 9293,
					RetweetsCount = 1923
				}
			};

			string json = JsonConvert.SerializeObject(contents, new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				Formatting = Formatting.Indented
			});
			Console.WriteLine(json);

			List<Content> deserializedJson = JsonConvert.DeserializeObject<List<Content>>(json, new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				Converters = new JsonConverter[] {
					new ContentConverter()
				}
			});
			Console.WriteLine("Done");
		}

		private static async Task DBUpsertTest()
		{
			List<Creator> creators = new List<Creator> {
				new Creator() {
					Id = "0",
					Name = "Test 1",
					Affiliations = new List<string> {
						"Test"
					},
					Socials = new List<CreatorSocial> {
						new YouTubeSocial {
							Id = "0",
							Name = "TestYouTube"
						}
					}
				},
				new Creator() {
					Id = "1",
					Name = "Test 2",
					Affiliations = new List<string> {
						"Test"
					},
					Socials = new List<CreatorSocial> {
						new YouTubeSocial {
							Id = "0",
							Name = "TestYouTube"
						}
					}
				}
			};

			List<Content> contents = new List<Content> {
				new YouTubeVideo() {
					Id = "0",
					Title = "Test Video 1",
					CreationDate = DateTimeOffset.UtcNow,
					ViewsCount = 281233,
					Description = "This is a test video."
				},
				new YouTubeBroadcast() {
					Id = "1",
					Title = "Test Broadcast 1",
					CreationDate = DateTimeOffset.MinValue,
					ViewsCount = 423123,
					Description = "This is a test broadcast.",
					IsLive = true,
					ScheduleDate = DateTimeOffset.MinValue
				},
				new TwitterTweet() {
					Id = "2",
					Text = "This is a test tweet",
					HeartsCount = 9239,
					RetweetsCount = 2233
				},
				new YouTubeBroadcast() {
					Id = "3",
					Title = "Test Broadcast 2",
					CreationDate = DateTimeOffset.MinValue,
					ViewsCount = 423123,
					Description = "This is a test broadcast 2.",
					IsLive = true,
					ScheduleDate = DateTimeOffset.UtcNow
				},
				new YouTubeBroadcast() {
					Id = "4",
					Title = "Test Broadcast 3",
					CreationDate = DateTimeOffset.MinValue,
					ViewsCount = 423123,
					Description = "This is a test broadcast 3.",
					IsLive = true,
					ScheduleDate = DateTimeOffset.UtcNow.AddMinutes(-10)
				}
			};

			VirtualHoleDBClient dbClient = new VirtualHoleDBClient(
				"mongodb+srv://<username>:<password>@us-east-1-free.41hlb.mongodb.net/test",
				"holoverse-editor",
				"RBqYN3ugVTb2stqD");
			await dbClient.Creators.UpsertManyCreatorsAndDeleteDanglingAsync(creators);
			await dbClient.Contents.UpsertManyContentsAndDeleteDanglingAsync(contents);

			Console.WriteLine("Done");
		}

		private static async Task DBFindTest()
		{
			VirtualHoleDBClient dbClient = new VirtualHoleDBClient(
				"mongodb+srv://<username>:<password>@us-east-1-free.41hlb.mongodb.net/test",
				"holoverse-editor",
				"RBqYN3ugVTb2stqD");

			FindResults<Creator> creatorResults = await dbClient.Creators.FindCreatorsAsync(new FindCreatorsStrictSettings { IsAll = true });
			await creatorResults.MoveNextAsync();

			FindSettings<Content> findContentSettings = new FindBroadcastContentSettings { };
			FindResults<Content> contentResults = await dbClient.Contents.FindContentsAsync(findContentSettings);
			await contentResults.MoveNextAsync();

			List<Creator> creators = new List<Creator>(creatorResults.Current);
			List<Content> contents = new List<Content>(contentResults.Current);

			Console.WriteLine("Done");
		}

		async static Task Main(string[] args)
		{
			//CreatorTest();
			//ContentTest();

			Console.WriteLine("Start Program");
			//await DBUpsertTest();
			await DBFindTest();
			Console.WriteLine("End Program");

			Console.ReadLine();
		}
	}
}
