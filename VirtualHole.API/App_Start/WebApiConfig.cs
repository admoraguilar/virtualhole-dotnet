using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Midnight;

namespace VirtualHole.API
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				DateParseHandling = DateParseHandling.None,
			};

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
