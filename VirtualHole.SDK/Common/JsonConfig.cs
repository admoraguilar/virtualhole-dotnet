using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VirtualHole
{
	public static class JsonConfig
	{
		public static JsonSerializerSettings DefaultSettings
		{
			get {
				return new JsonSerializerSettings {
					Converters = null,
					ContractResolver = new DefaultContractResolver() {
						IgnoreSerializableAttribute = false,
						NamingStrategy = new CamelCaseNamingStrategy()
					},
					ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
					DateParseHandling = DateParseHandling.None
				};
			}
		}
	}
}
