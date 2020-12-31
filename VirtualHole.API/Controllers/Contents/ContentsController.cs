using System;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using Humanizer;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;
using VirtualHole.API.Services;

namespace VirtualHole.API.Controllers
{
	public partial class ContentsController : ApiController
	{
		private string affiliationCommunity => "Community";

		private ContentsClient contentsClient => dbService.Client.Contents;
		private VirtualHoleDBService dbService = null;

		public ContentsController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/v1/contents/discover")]
		[HttpGet]
		public async Task<IHttpActionResult> GetDiscover([FromUri] ContentsQuery query)
		{
			if(query == null) { query = new ContentsQuery(); }

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { ContentTypes.Video };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = false;
			query.IsAffiliationsInclude = false;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			FindResultsPipeline<ContentsQuery, Content> pipeline = new FindResultsPipeline<ContentsQuery, Content>();
			pipeline.Query = query;
			pipeline.Steps.Add(new PagingFindStep<ContentsQuery>());
			pipeline.Steps.Add(new ContentFilterFindStep());
			pipeline.Steps.Add(new ContentSortFindStep());
			pipeline.FindProvider = (FindSettings find) => contentsClient.FindContentsAsync(find);
			pipeline.PostProcessFactory = ContentDTOFactory;

			return Ok(await pipeline.ExecuteAsync());
		}

		[Route("api/v1/contents/community")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCommunity([FromUri] ContentsQuery query)
		{
			if(query == null) { query = new ContentsQuery(); }

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { ContentTypes.Video };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = true;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			FindResultsPipeline<ContentsQuery, Content> pipeline = new FindResultsPipeline<ContentsQuery, Content>();
			pipeline.Query = query;
			pipeline.Steps.Add(new PagingFindStep<ContentsQuery>());
			pipeline.Steps.Add(new ContentFilterFindStep());
			pipeline.Steps.Add(new ContentSortFindStep());
			pipeline.FindProvider = (FindSettings find) => contentsClient.FindContentsAsync(find);
			pipeline.PostProcessFactory = ContentDTOFactory;

			return Ok(await pipeline.ExecuteAsync());
		}

		[Route("api/v1/contents/live")]
		[HttpGet]
		public async Task<IHttpActionResult> GetLive([FromUri] ContentsQuery query)
		{
			if(query == null) { query = new ContentsQuery(); }

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { ContentTypes.Broadcast };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = false;
			query.IsAffiliationsInclude = false;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			FindResultsPipeline<ContentsQuery, Content> pipeline = new FindResultsPipeline<ContentsQuery, Content>();
			pipeline.Query = query;
			pipeline.Steps.Add(new PagingFindStep<ContentsQuery>());
			pipeline.Steps.Add(new ContentFilterFindStep());
			pipeline.Steps.Add(new BroadcastLiveFilterFindStep() { IsLive = true });
			pipeline.Steps.Add(new ContentSortFindStep());
			pipeline.FindProvider = (FindSettings find) => contentsClient.FindContentsAsync(find);
			pipeline.PostProcessFactory = ContentDTOFactory;

			return Ok(await pipeline.ExecuteAsync());
		}

		[Route("api/v1/contents/scheduled")]
		[HttpGet]
		public async Task<IHttpActionResult> GetScheduled([FromUri] ContentsQuery query)
		{
			if(query == null) { query = new ContentsQuery(); }

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { ContentTypes.Broadcast };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = false;
			query.IsAffiliationsInclude = false;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			FindResultsPipeline<ContentsQuery, Content> pipeline = new FindResultsPipeline<ContentsQuery, Content>();
			pipeline.Query = query;
			pipeline.Steps.Add(new PagingFindStep<ContentsQuery>());
			pipeline.Steps.Add(new ContentFilterFindStep());
			pipeline.Steps.Add(new BroadcastLiveFilterFindStep() { IsLive = false });
			pipeline.Steps.Add(new BroadcastSortFindStep());
			pipeline.FindProvider = (FindSettings find) => contentsClient.FindContentsAsync(find);
			pipeline.PostProcessFactory = ContentDTOFactory;

			return Ok(await pipeline.ExecuteAsync());
		}

		private async Task<object> ContentDTOFactory(ContentsQuery query, Content content)
		{
			HttpClient httpClient = HttpClientFactory.HttpClient;

			string creationDateDisplay = string.Empty;
			string scheduleDateDisplay = string.Empty;

			bool isAvailable = true;
			bool isBroadcast = content.ContentType == ContentTypes.Broadcast;

			if(HasTimestampAndLocale()) {
				creationDateDisplay = content.CreationDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
			}

			if(content.SocialType == SocialTypes.YouTube) {
				// Include a flag that tells if this video is not accessible for some reason.
				// privated, deleted, etc...
				if(content is YouTubeVideo youTubeVideo) {
					// HACK: 
					// * Check for high quality thumbnail
					// * If there's none, switch to MQ.
					// * If there's none, then it's either this video is privated or deleted. Hence mark
					// as unavailable.
					//
					// SOLUTION FUTURE: Should find a better way to do this for better maintainance of the
					// API.
					string defaultThumbnailUrl = youTubeVideo.ThumbnailUrl;
					string hiresThumbnailUrl = defaultThumbnailUrl.Replace("mqdefault", "hq720");

					HttpResponseMessage res = await httpClient.GetAsync(hiresThumbnailUrl);
					youTubeVideo.ThumbnailUrl = hiresThumbnailUrl;

					if(!res.IsSuccessStatusCode) { 
						res = await httpClient.GetAsync(defaultThumbnailUrl);
						youTubeVideo.ThumbnailUrl = defaultThumbnailUrl;
					}

					isAvailable = res.IsSuccessStatusCode;
				}

				if(content is YouTubeBroadcast youTubeBroadcast) {
					if(HasTimestampAndLocale()) {
						scheduleDateDisplay = youTubeBroadcast.ScheduleDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
					}
				}
			}

			if(isBroadcast) {
				return new {
					content,
					isAvailable,
					creationDateDisplay,
					scheduleDateDisplay
				};
			} else {
				return new {
					content,
					isAvailable,
					creationDateDisplay,
				};
			}

			bool HasTimestampAndLocale() => query.Timestamp != DateTimeOffset.MinValue && !string.IsNullOrEmpty(query.Locale);
		}
	}
}