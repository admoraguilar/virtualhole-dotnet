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
			FindSettings find = new FindSettings();
			if(query == null) {
				find.Filters.Add(new CreatorsFilter() { });
				return Ok(await ProcessQuery(query, find));
			}

			if(string.IsNullOrEmpty(query.Search)) {
				find.Filters.Add(new CreatorsFilter() {	
					IsCheckForIsGroup = false,
				});
			} else {
				find.Filters.Add(new CreatorsRegexFilter() {
					SearchQueries = new List<string>() { query.Search }
				});
			}

			return Ok(await ProcessQuery(query, find));
		}

		private async Task<List<Creator>> ProcessQuery<T>(CreatorQuery query, T find)
			where T : FindSettings
		{
			List<Creator> results = new List<Creator>();
			if(query == null) { return results; }
			else {
				find.Page = query.Page;
				find.PageSize = query.PageSize;
				find.MaxPages = query.MaxPages;
			}

			FindResults<Creator> findResults = await creatorClient.FindCreatorsAsync(find);
			await findResults.MoveNextAsync();
			results.AddRange(findResults.Current);

			return results;
		}
    }
}
