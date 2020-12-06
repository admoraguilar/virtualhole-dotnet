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
			JsonConvert.DefaultSettings = () => JsonUtilities.SerializerSettings.DefaultCamelCase;

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
