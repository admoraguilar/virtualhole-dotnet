using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using Swashbuckle.Swagger.Annotations;
using Humanizer;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;
using VirtualHole.API.Services;

namespace VirtualHole.API.Controllers
{
	public class ContentController : ApiController
    {
		private ContentClient contentClient => dbService.Client.Contents;
		private VirtualHoleDBService dbService = null;

		public ContentController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/v1/content")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(List<YouTubeVideo>))]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(List<YouTubeBroadcast>))]
		[HttpGet]
		public async Task<IHttpActionResult> GetContent([FromUri] ContentQuery query)
		{
			FindContentSettings settings = null;
			if(query == null) {
				settings = new FindContentSettings() { };
				return Ok(await InternalListContents(query, settings));
			}

			if(query.CreatorIds != null && query.CreatorIds.Count > 0) {
				if(query.IsCreatorRelated) {
					settings = new FindCreatorRelatedContentSettings() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds,
						CreatorNames = query.CreatorNames,
						CreatorSocialIds = query.CreatorSocialIds,
						CreatorSocialUrls = query.CreatorSocialUrls
					};
				} else {
					settings = new FindCreatorContentSettings() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds
					};
				}
			}

			if(query.SocialType != null && query.SocialType.Count > 0) {
				settings.IsSocialTypeInclude = query.IsSocialTypeInclude;
				settings.SocialType = query.SocialType;
			}

			if(query.ContentType != null && query.ContentType.Count > 0) {
				settings.IsContentTypeInclude = query.IsContentTypeInclude;
				settings.ContentType = query.ContentType;
			}

			return Ok(await InternalListContents(query, settings));
		}

		private async Task<List<Content>> InternalListContents<T>(PaginatedQuery query, T request)
			where T : FindContentSettings
		{
			List<Content> results = new List<Content>();

			FindResults<Content> findResults = await contentClient.FindContentsAsync(request.SetPage(query));
			await findResults.MoveNextAsync();
			results.AddRange(findResults.Current);

			return results;
		}

		//[Route("api/Videos/ListCreatorRelatedVideos")]
		//[HttpPost]
		//public async Task<List<Video>> ListCreatorRelatedVideosAsync([FromBody] FindCreatorRelatedVideosSettings<Video> request)
		//{
		//	return await InternalListVideos<Video, FindCreatorRelatedVideosSettings<Video>>(request);
		//}

		//[Route("api/Videos/ListCreatorVideos")]
		//[HttpPost]
		//public async Task<List<Video>> ListCreatorVideosAsync([FromBody] FindCreatorVideosSettings<Video> request)
		//{
		//	return await InternalListVideos<Video, FindCreatorVideosSettings<Video>>(request);
		//}

		//[Route("api/Broadcasts/ListCreatorRelatedBroadcasts")]
		//[HttpPost]
		//public async Task<List<Broadcast>> ListCreatorRelatedBroadcastsAsync([FromBody] FindCreatorRelatedVideosSettings<Broadcast> request)
		//{
		//	return await InternalListVideos<Broadcast, FindCreatorRelatedVideosSettings<Broadcast>>(request);
		//}

		//[Route("api/Broadcasts/ListCreatorBroadcasts")]
		//[HttpPost]
		//public async Task<List<Broadcast>> ListCreatorBroadcastsAsync([FromBody] FindCreatorVideosSettings<Broadcast> request)
		//{
		//	return await InternalListVideos<Broadcast, FindCreatorVideosSettings<Broadcast>>(request);
		//}

		//private async Task<List<TVideo>> InternalListVideos<TVideo, TFind>(TFind request)
		//	where TVideo : Video
		//	where TFind : FindVideosSettings<TVideo>
		//{
		//	List<TVideo> results = new List<TVideo>();

		//	FindResults<TVideo> findResults = await videoClient.FindVideosAsync(request);
		//	await findResults.MoveNextAsync();
		//	results.AddRange(findResults.Current);

		//	foreach(TVideo result in results) {
		//		if(result is Video video) {
		//			video.CreationDateDisplay = video.CreationDate.Humanize(request.Timestamp, new CultureInfo(request.Locale));
		//		}

		//		if(result is Broadcast broadcast) {
		//			broadcast.ScheduleDateDisplay = broadcast.ScheduleDate.Humanize(request.Timestamp, new CultureInfo(request.Locale));
		//		}
		//	}

		//	return results;
		//}
	}
}
