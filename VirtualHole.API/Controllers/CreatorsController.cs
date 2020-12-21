using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Swashbuckle.Swagger.Annotations;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;
using VirtualHole.API.Services;

namespace VirtualHole.API.Controllers
{
	public class CreatorsController : ApiController
    {
		private CreatorClient creatorClient => dbService.Client.Creators;
		private VirtualHoleDBService dbService = null;

		public CreatorsController()
		{
			dbService = new VirtualHoleDBService();
		}

		// Source: https://stackoverflow.com/a/49076281
		[Route("api/v1/creators")]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Creator>))]
		[HttpGet]
		public async Task<IHttpActionResult> GetCreators([FromUri] CreatorQuery query)
		{
			CreatorsFilter settings = null;
			if(query == null) {
				settings = new CreatorsFilter() { };
				return Ok(await InternalListCreators(query, settings));
			}

			if(string.IsNullOrEmpty(query.Search)) {
				settings = new CreatorsStrictFilter() {	
					IsCheckForIsGroup = false,
				};
			} else {
				settings = new CreatorsRegexFilter() {
					SearchQueries = new List<string>() { query.Search }
				};
			}

			return Ok(await InternalListCreators(query, settings));
		}

		private async Task<List<Creator>> InternalListCreators<T>(PagedQuery query, T request)
			where T : CreatorsFilter
		{
			List<Creator> results = new List<Creator>();
			if(query != null) { request.SetPage(query); }

			FindResults<Creator> findResults = await creatorClient.FindCreatorsAsync(request);
			await findResults.MoveNextAsync();
			results.AddRange(findResults.Current);

			return results;
		}
    }
}
