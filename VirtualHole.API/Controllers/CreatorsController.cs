using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using VirtualHole.DB;
using VirtualHole.DB.Contents.Creators;
using VirtualHole.API.Services;

namespace VirtualHole.API.Controllers
{
	public class CreatorsController : ApiController
    {
		private CreatorClient creatorClient => dbService.Client.Contents.Creators;
		private VirtualHoleDBService dbService = null;

		public CreatorsController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/Creators/ListCreatorsRegex")]
		[HttpPost]
		public async Task<List<Creator>> ListCreatorsRegexAsync([FromBody] FindCreatorsRegexSettings request)
		{
			return await InternalListCreators(request);
		}

		[Route("api/Creators/ListCreatorsStrict")]
		[HttpPost]
		public async Task<List<Creator>> ListCreatorsStrictAsync([FromBody] FindCreatorsStrictSettings request)
		{
			return await InternalListCreators(request);
		}

		[Route("api/Creators/ListCreators")]
		[HttpPost]
		public async Task<List<Creator>> ListCreatorsAsync([FromBody] FindCreatorsSettings request)
		{
			return await InternalListCreators(request);
		}

		private async Task<List<Creator>> InternalListCreators<T>(T request)
			where T : FindCreatorsSettings
		{
			List<Creator> results = new List<Creator>();
			
			FindResults<Creator> findResults = await creatorClient.FindCreatorsAsync(request);
			await findResults.MoveNextAsync();
			results.AddRange(findResults.Current);
			
			return results;
		}
    }
}
