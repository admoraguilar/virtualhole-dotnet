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
	public partial class CreatorsController : ApiController
    {
		private CreatorsClient creatorClient => dbService.Client.Creators;
		private VirtualHoleDBService dbService = null;

		public CreatorsController()
		{
			dbService = new VirtualHoleDBService();
		}

		// Source: https://stackoverflow.com/a/49076281
		[Route("api/v1/creators")]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Creator>))]
		[HttpGet]
		public async Task<IHttpActionResult> GetCreators([FromUri] CreatorsQuery query)
		{
			if(query == null) { query = new CreatorsQuery(); }

			FindResultsPipeline<CreatorsQuery, Creator> pipeline = new FindResultsPipeline<CreatorsQuery, Creator>();
			pipeline.Query = query;
			pipeline.Steps.Add(new PagingFindStep<CreatorsQuery>());
			pipeline.Steps.Add(new CreatorFilterFindStep());
			pipeline.FindProvider = (FindSettings find) => creatorClient.FindCreatorsAsync(find);

			return Ok(await pipeline.ExecuteAsync());
		}
    }
}
