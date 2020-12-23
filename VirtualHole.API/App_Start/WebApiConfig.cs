using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VirtualHole.API
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				DateParseHandling = DateParseHandling.None,
			};

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

			// Source: https://weblog.west-wind.com/posts/2012/mar/09/removing-the-xml-formatter-from-aspnet-web-api-applications
			// Remove default Xml handler
			List<MediaTypeFormatter> matches = config.Formatters
				.Where(f => f.SupportedMediaTypes
				.Where(m => 
					m.MediaType.ToString() == "application/xml" 
					|| m.MediaType.ToString() == "text/xml")
				.Count() > 0)
				.ToList();
			foreach(var match in matches)
				config.Formatters.Remove(match);

			//// Source: https://stackoverflow.com/a/26068063
			//config.Formatters.JsonFormatter.MediaTypeMappings.Add(
			//	new RequestHeaderMapping("Accept",
			//		"text/html", StringComparison.InvariantCultureIgnoreCase,
			//		true, "application/json"));

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
