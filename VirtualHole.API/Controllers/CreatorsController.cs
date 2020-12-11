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

		[Route("api/v1/creators")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(List<Creator>))]
		[HttpGet]
		public async Task<IHttpActionResult> GetCreators([FromUri] CreatorQuery query)
		{
			FindCreatorsSettings creatorSettings = null;

			if(string.IsNullOrEmpty(query.Search)) {
				creatorSettings = new FindCreatorsStrictSettings() {
					IsAll = true,
					IsCheckForIsGroup = false,
				};
			} else {
				creatorSettings = new FindCreatorsRegexSettings() {
					SearchQueries = new List<string>() { query.Search }
				};
			}

			return Ok(await InternalListCreators(query, creatorSettings));
		}

		private async Task<List<Creator>> InternalListCreators<T>(PaginatedQuery query, T request)
			where T : FindCreatorsSettings
		{
			List<Creator> results = new List<Creator>();
			
			FindResults<Creator> findResults = await creatorClient.FindCreatorsAsync(request.SetPage(query));
			await findResults.MoveNextAsync();
			results.AddRange(findResults.Current);

			return results;
		}
    }
}
