using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.DB.Contents;

namespace VirtualHole.SDK.V1.Tests
{
	class Program
	{
		static void CreatorTest()
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

		static void ContentTest()
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

		static void Main(string[] args)
		{
			//CreatorTest();
			ContentTest();

			Console.ReadLine();
		}
	}
}
