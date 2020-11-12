using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VirtualHole.Common
{
	public static class JsonUtilities
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
