using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Swashbuckle.Swagger.Annotations;
using Midnight.Pipeline;
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
		public async Task<IHttpActionResult> GetCreators([FromUri] CreatorQuery query)
		{
			Pipeline<FindContext<CreatorQuery, Creator>> pipeline =
				new Pipeline<FindContext<CreatorQuery, Creator>>(
					new FindContext<CreatorQuery, Creator>() {
						InQuery = query ?? new CreatorQuery(),
						InProvider = (FindSettings find) => creatorClient.FindAsync(find),
					});

			pipeline.Add(new CreatorFilterStep());
			pipeline.Add(new GetPagedResultsStep<CreatorQuery, Creator>());
			await pipeline.ExecuteAsync();

			return Ok(pipeline.Context.OutResults);
		}
    }
}
