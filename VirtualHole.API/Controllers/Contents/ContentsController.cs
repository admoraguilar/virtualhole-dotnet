using System;
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
			if(query == null) {
				query = new ContentsQuery();
			}

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { ContentTypes.Video };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = false;
			query.IsAffiliationsInclude = false;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(ContentUtilities.AffiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { ContentUtilities.AffiliationCommunity };
			}

			FindResultsPipeline<ContentsQuery, Content> pipeline = new FindResultsPipeline<ContentsQuery, Content>();
			pipeline.Query = query;
			pipeline.Steps.Add(new ContentSortFindStep());
			pipeline.Steps.Add(new ContentFilterFindStep());
			pipeline.FindProvider = (FindSettings find) => contentsClient.FindContentsAsync(find);
			pipeline.ResultFactory = ContentUtilities.ContentDTOFactory;
			return Ok(await pipeline.ExecuteAsync());
		}

		[Route("api/v1/contents/community")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCommunity([FromUri] ContentsQuery query)
		{
			FindSettings find = new FindSettings();

			find.Sorts.Add(new ContentSort {
				SortMode = SortMode.ByCreationDate,
				IsSortAscending = query != null ? query.IsSortAscending : false
			});

			if(query == null) {
				query = new ContentsQuery();
			}

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { "video" };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = true;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			return await GetContent(find, query);
		}

		[Route("api/v1/contents/live")]
		[HttpGet]
		public async Task<IHttpActionResult> GetLive([FromUri] ContentsQuery query)
		{
			FindSettings find = new FindSettings();

			find.Sorts.Add(new ContentSort {
				SortMode = SortMode.ByCreationDate,
				IsSortAscending = query != null ? query.IsSortAscending : false
			});

			find.Filters.Add(new BroadcastContentFilter() {
				IsLive = true
			});

			if(query == null) {
				query = new ContentsQuery();
			}

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { "broadcast" };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = false;
			query.IsAffiliationsInclude = false;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			return await GetContent(find, query);
		}

		[Route("api/v1/contents/scheduled")]
		[HttpGet]
		public async Task<IHttpActionResult> GetScheduled([FromUri] ContentsQuery query)
		{
			FindSettings find = new FindSettings();

			find.Sorts.Add(new ContentSort {
				SortMode = SortMode.BySchedule,
				IsSortAscending = query != null ? query.IsSortAscending : false
			});

			find.Filters.Add(new BroadcastContentFilter() {
				IsLive = false
			});

			if(query == null) {
				query = new ContentsQuery();
			}

			query.IsContentTypeInclude = true;
			query.ContentType = new List<string>() { "broadcast" };

			query.IsCheckCreatorAffiliations = true;
			query.IsAffiliationsAll = false;
			query.IsAffiliationsInclude = false;

			if(query.CreatorAffiliations != null) {
				query.CreatorAffiliations.Add(affiliationCommunity);
			} else {
				query.CreatorAffiliations = new List<string>() { affiliationCommunity };
			}

			return await GetContent(find, query);
		}

		[Route("api/v1/contents")]
		[HttpGet]
		public async Task<IHttpActionResult> GetContent([FromUri] ContentsQuery query)
		{
			FindSettings find = new FindSettings();

			find.Sorts.Add(new ContentSort {
				SortMode = SortMode.ByCreationDate,
				IsSortAscending = query != null ? query.IsSortAscending : false
			});

			if(query == null) {
				query = new ContentsQuery();
			}

			return await GetContent(find, query);
		}

		private async Task<IHttpActionResult> GetContent(FindSettings find, ContentsQuery query)
		{
			if(query.CreatorIds != null && query.CreatorIds.Count > 0) {
				if(query.IsCreatorRelated) {
					find.Filters.Add(new CreatorRelatedContentFilter() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds,
						CreatorNames = query.CreatorNames,
						CreatorSocialIds = query.CreatorSocialIds,
						CreatorSocialUrls = query.CreatorSocialUrls
					});
				} else {
					find.Filters.Add(new CreatorContentFilter() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds
					});
				}
			}

			ContentFilter contentsFilter = new ContentFilter();

			if(query.SocialType != null && query.SocialType.Count > 0) {
				contentsFilter.IsSocialTypeInclude = query.IsSocialTypeInclude;
				contentsFilter.SocialType = query.SocialType;
			}

			if(query.ContentType != null && query.ContentType.Count > 0) {
				contentsFilter.IsContentTypeInclude = query.IsContentTypeInclude;
				contentsFilter.ContentType = query.ContentType;
			}

			contentsFilter.IsCheckForAffiliations = query.IsCheckCreatorAffiliations;
			contentsFilter.IsAffiliationsAll = query.IsAffiliationsAll;
			contentsFilter.IsAffiliationsInclude = query.IsAffiliationsInclude;
			contentsFilter.Affiliations = query.CreatorAffiliations;

			find.Filters.Add(contentsFilter);

			return Ok(await ControllerUtilities.ProcessPagedQuery(
				query, find, 
				() => contentsClient.FindContentsAsync(find),
				ContentDTOFactory
			));
		}
	}

	public partial class ContentsController
	{
		private static object ContentDTOFactory(ContentsQuery query, Content content)
		{
			string creationDateDisplay = string.Empty;
			string scheduleDateDisplay = string.Empty;

			bool isBroadcast = false;

			if(query.Timestamp != DateTimeOffset.MinValue && !string.IsNullOrEmpty(query.Locale)) {
				creationDateDisplay = content.CreationDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
				if(content is YouTubeBroadcast youTubeBroadcast) {
					isBroadcast = true;
					scheduleDateDisplay = youTubeBroadcast.ScheduleDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
				}
			}

			if(isBroadcast) {
				return new {
					content,
					creationDateDisplay,
					scheduleDateDisplay
				};
			} else {
				return new {
					content,
					creationDateDisplay,
				};
			}
		}
	}
}
