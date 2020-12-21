using System;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using Swashbuckle.Swagger.Annotations;
using Humanizer;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;
using VirtualHole.API.Services;

namespace VirtualHole.API.Controllers
{
	public class ContentsController : ApiController
    {
		private CreatorClient creatorClient => dbService.Client.Creators;
		private ContentClient contentClient => dbService.Client.Contents;
		private VirtualHoleDBService dbService = null;

		public ContentsController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/v1/contents/discover")]
		public async Task<IHttpActionResult> GetDiscover([FromUri] ContentQuery query)
		{

		}

		[Route("api/v1/contents/community")]
		public async Task<IHttpActionResult> GetCommunity([FromUri] ContentQuery query)
		{

		}

		[Route("api/v1/contents/live")]
		public async Task<IHttpActionResult> GetLive([FromUri] ContentQuery query)
		{

		}

		[Route("api/v1/contents/scheduled")]
		public async Task<IHttpActionResult> GetScheduled([FromUri] ContentQuery query)
		{

		}

		[Route("api/v1/contents")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(List<ContentDTO>))]
		[HttpGet]
		public async Task<IHttpActionResult> GetContent([FromUri] ContentQuery query)
		{
			//return this.Json(new {
			//	string Test = "",
			//});

			FindSettings settings = new FindSettings();

			settings.Sorts.Add(new ContentsSort {
				SortMode = SortMode.ByCreationDate,
				IsSortAscending = query.IsSortAscending
			});

			if(query == null) {
				settings.Filters.Add(new ContentsFilter() { });
				return Ok(await ProcessRequest(query, settings));
			}

			if(query.CreatorIds != null && query.CreatorIds.Count > 0) {
				if(query.IsCreatorRelated) {
					settings.Filters.Add(new CreatorRelatedContentsFilter() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds,
						CreatorNames = query.CreatorNames,
						CreatorSocialIds = query.CreatorSocialIds,
						CreatorSocialUrls = query.CreatorSocialUrls
					});
				} else {
					settings.Filters.Add(new CreatorContentsFilter() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds
					});
				}
			}

			ContentsFilter contentsFilter = new ContentsFilter();

			if(query.SocialType != null && query.SocialType.Count > 0) {
				contentsFilter.IsSocialTypeInclude = query.IsSocialTypeInclude;
				contentsFilter.SocialType = query.SocialType;
			}

			if(query.ContentType != null && query.ContentType.Count > 0) {
				contentsFilter.IsContentTypeInclude = query.IsContentTypeInclude;
				contentsFilter.ContentType = query.ContentType;
			}

			settings.Filters.Add(contentsFilter);

			return Ok(await ProcessRequest(query, settings));
		}

		private async Task<CreatorContentsFilter> CreateCreatorContentsFilter(ContentQuery query)
		{
			CreatorContentsFilter filter = new CreatorContentsFilter();
			if(query.IsCheckCreatorAffiliations) {
				FindResults<Creator> creatorFindResults = await creatorClient.FindCreatorsAsync(new FindSettings {
					Filters = new List<FindFilter>() {
						new CreatorsFilter {
							IsCheckForAffiliations = true,
							Affiliations = new List<string>(query.CreatorAffiliations)
						}
					}
				});
				await creatorFindResults.MoveNextAsync();
			} else {
				filter.IsCreatorsInclude = query.IsCreatorsInclude;
				filter.CreatorIds = query.CreatorIds;
			}

			return filter;
		}

		private async Task<List<ContentDTO>> ProcessRequest<T>(ContentQuery query, T request)
			where T : FindSettings
		{
			List<ContentDTO> results = new List<ContentDTO>();
			if(query == null) { return results; }
			else {
				request.Page = query.Page;
				request.PageSize = query.PageSize;
				request.MaxPages = query.MaxPages;
			}

			FindResults<Content> contentFindResults = await contentClient.FindContentsAsync(request);
			await contentFindResults.MoveNextAsync();

			FindResults<Creator> creatorFindResults = await creatorClient.FindCreatorsAsync(new FindSettings {
				Filters = new List<FindFilter>() {
					new CreatorsStrictFilter {
						Id = new List<string>(contentFindResults.Current.Select(c => c.CreatorId))
					}
				}
			});
			await creatorFindResults.MoveNextAsync();

			Dictionary<string, Creator> creators = new Dictionary<string, Creator>();
			foreach(Creator creatorFindResult in creatorFindResults.Current) {
				creators[creatorFindResult.Id] = creatorFindResult;
			}

			foreach(Content findResult in contentFindResults.Current) {
				Creator creator = creators[findResult.CreatorId];
				CreatorSocial creatorSocial = creator.Socials.FirstOrDefault(s => s.SocialType == findResult.SocialType);

				ContentDTO contentDTO = new ContentDTO {
					Content = findResult,
					CreatorSocialId = creatorSocial.Id,
					CreatorSocialName = creatorSocial.Name,
					CreatorAvatarUrl = creator.AvatarUrl
				};

				if(query.Timestamp != DateTimeOffset.MinValue && !string.IsNullOrEmpty(query.Locale)) {
					contentDTO.CreationDateDisplay = findResult.CreationDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));

					if(findResult is YouTubeBroadcast youTubeBroadcast) {
						contentDTO.ScheduleDateDisplay = youTubeBroadcast.ScheduleDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
					}
				}

				results.Add(contentDTO);			
			}

			return results;
		}
	}
}
