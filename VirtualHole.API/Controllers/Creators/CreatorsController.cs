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
			Pipeline<FindContext<CreatorsQuery, Creator>> pipeline =
				new Pipeline<FindContext<CreatorsQuery, Creator>>(
					new FindContext<CreatorsQuery, Creator>() {
						InQuery = query ?? new CreatorsQuery(),
						InProvider = (FindSettings find) => creatorClient.FindCreatorsAsync(find),
					});

			pipeline.Add(new CreatorFilterStep());
			pipeline.Add(new GetPagedResultsStep<CreatorsQuery, Creator>());
			await pipeline.ExecuteAsync();

			return Ok(pipeline.Context.OutResults);
		}
    }
}
