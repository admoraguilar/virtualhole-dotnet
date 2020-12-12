using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using shortid;
using shortid.Configuration;
using Midnight;
using Midnight.Logs;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.SDK.V1.Tests
{
	public class CreatorV1
	{
		public string UniversalName = string.Empty;
		public string UniversalId = string.Empty;
		public string WikiUrl = string.Empty;
		public string AvatarUrl = string.Empty;

		public bool IsHidden = false;

		public string[] Affiliations = new string[0];
		public bool IsGroup = false;
		public int Depth = 0;

		public SocialV1[] Socials = new SocialV1[0];
		public string[] CustomKeywords = new string[0];
	}

	public class SocialV1
	{
		public string Name;
		public Platform Platform;
		public string Id;
		public string Url;
		public string AvatarUrl;

		public string[] CustomKeywords = new string[0];
	}

	public enum Platform
	{
		None,
		YouTube,
		Twitter,
		Reddit,
		Twitch,
		Niconico
	}

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

			FindResults<Creator> creatorResults = await dbClient.Creators.FindCreatorsAsync(new FindCreatorsStrictSettings { });
			await creatorResults.MoveNextAsync();

			FindSettings<Content> findContentSettings = new FindContentSettings {
				ContentType = new List<string>() { ContentTypes.Broadcast } 
			};
			FindResults<Content> contentResults = await dbClient.Contents.FindContentsAsync(findContentSettings);
			await contentResults.MoveNextAsync();

			List<Creator> creators = new List<Creator>(creatorResults.Current);
			List<Content> contents = new List<Content>(contentResults.Current);

			Console.WriteLine("Done");
		}

		private static async Task DBConvertCreatorsV1()
		{
			await Task.CompletedTask;

			string jsonRaw = File.ReadAllText($"F:/Desktop/content/creators.json");
			List<CreatorV1> creatorV1s = JsonConvert.DeserializeObject<List<CreatorV1>>(jsonRaw);
			
			List<Creator> creators = new List<Creator>();
			creatorV1s.ForEach(v1 => {
				Creator creator = new Creator {
					Id = ShortId.Generate(new GenerationOptions {
						Length = 8,
						UseNumbers = true,
						UseSpecialCharacters = true
					}),
					Name = v1.UniversalName,
					AvatarUrl = v1.AvatarUrl,
					IsHidden = v1.IsHidden,
					IsGroup = v1.IsGroup,
					Depth = v1.Depth,
					Affiliations = new List<string>(v1.Affiliations)
				};

				v1.Socials.ForEach(v1Soc => {
					if(v1Soc.Platform == Platform.YouTube) {
						creator.Socials.Add(new YouTubeSocial {
							Id = v1Soc.Id,
							Name = v1Soc.Name,
							Url = v1Soc.Url,
							AvatarUrl = v1Soc.AvatarUrl
						});
					}
				});

				creators.Add(creator);
			});

			VirtualHoleDBClient dbClient = new VirtualHoleDBClient(
				"mongodb+srv://<username>:<password>@us-east-1-free.41hlb.mongodb.net/test",
				"holoverse-editor",
				"RBqYN3ugVTb2stqD");

			await dbClient.Creators.UpsertManyCreatorsAndDeleteDanglingAsync(creators);

			MLog.Log(creatorV1s.Count);
			MLog.Log(creators.Count);
		}

		private static async Task FindJsonTest()
		{
			await Task.CompletedTask;
			string json = JsonConvert.SerializeObject(new FindCreatorContentSettings(), Formatting.Indented);
			Console.WriteLine(json);
		}

		async static Task Main(string[] args)
		{
			//CreatorTest();
			//ContentTest();

			MLog.Log(MLogLevel.Warning, "Start Program");
			//await DBUpsertTest();
			//await DBFindTest();
			//await DBConvertCreatorsV1();
			await FindJsonTest();
			MLog.Log(MLogLevel.Warning, "End Program");

			Console.ReadLine();
		}
	}
}
