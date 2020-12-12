using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Midnight;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
    public class ResourcesController : ApiController
    {
		// Source: https://www.geeksblood.com/ihttpactionresult-vs-httpresponsemessage/#:~:text=In%20web%20API%201%2C%20We,response%20message%20from%20action%20method.&text=In%20Web%20API%202%2C%20IHttpActionResult,of%20response%20that%20we%20create.
		// (HttpResponseMessage vs IHttpActionResult)
		[Route("api/v1/resources")]
		[HttpGet]
        public async Task<HttpResponseMessage> Get([FromUri] ResourcesQuery query)
        {
			HttpClient httpClient = HttpClientFactory.GetClient();

			if(query == null || string.IsNullOrEmpty(query.Path)) {
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Query is invalid.");
			}

			string domain = "https://virtualhole.b-cdn.net/";
			HttpResponseMessage httpResponse = await httpClient.GetAsync(
				UriUtilities.CombineUri(domain, query.Path));
			return httpResponse;
		}
    }
}
