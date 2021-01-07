using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;

namespace VirtualHole.Scraper
{
	internal static class JsonConfig
	{
		public static void Initialize()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Converters = new JsonConverter[] {
					new ContentConverter(),
					new CreatorSocialConverter(),
				},
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				DateParseHandling = DateParseHandling.None,
			};
		}
	}
}
