using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VirtualHole.API.Controllers
{
	using DB.Common;
	using DB.Contents.Videos;
	using Common;
	using Services;

	public class VideosController : ApiController
    {
		private VideoClient videoClient => dbService.Client.Contents.Videos;
		private VirtualHoleDBService dbService = null;

		public VideosController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/Videos/ListCreatorRelatedVideos")]
		[HttpPost]
		public async Task<List<Video>> ListCreatorRelatedVideosAsync([FromBody] FindCreatorRelatedVideosSettings<Video> request)
		{
			return await InternalListVideos<Video, FindCreatorRelatedVideosSettings<Video>>(request);
		}

		[Route("api/Videos/ListCreatorVideos")]
		[HttpPost]
		public async Task<List<Video>> ListCreatorVideosAsync([FromBody] FindCreatorVideosSettings<Video> request)
		{
			return await InternalListVideos<Video, FindCreatorVideosSettings<Video>>(request);
		}

		[Route("api/Broadcasts/ListCreatorRelatedBroadcasts")]
		[HttpPost]
		public async Task<List<Broadcast>> ListCreatorRelatedBroadcastsAsync([FromBody] FindCreatorRelatedVideosSettings<Broadcast> request)
		{
			return await InternalListVideos<Broadcast, FindCreatorRelatedVideosSettings<Broadcast>>(request);
		}

		[Route("api/Broadcasts/ListCreatorBroadcasts")]
		[HttpPost]
		public async Task<List<Broadcast>> ListCreatorBroadcastsAsync([FromBody] FindCreatorRelatedVideosSettings<Broadcast> request)
		{
			return await InternalListVideos<Broadcast, FindCreatorRelatedVideosSettings<Broadcast>>(request);
		}

		private async Task<List<TVideo>> InternalListVideos<TVideo, TFind>(TFind request)
			where TVideo : Video
			where TFind : FindVideosSettings<TVideo>
		{
			List<TVideo> results = new List<TVideo>();

			FindResults<TVideo> findResults = await videoClient.FindVideosAsync(request);
			await findResults.MoveNextAsync();
			results.AddRange(findResults.Current);

			return results;
		}
	}
}
