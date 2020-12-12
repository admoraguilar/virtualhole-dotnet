using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VirtualHole.Scraper
{
	internal static class JsonConfig
	{
		public static void Initialize()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				DateParseHandling = DateParseHandling.None,
			};
		}
	}
}
