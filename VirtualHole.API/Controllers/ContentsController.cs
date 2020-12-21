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
			return Ok(await ProcessRequest(query, new FindSettings(), DefaultContentDTOFactory));
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
			FindSettings find = new FindSettings();

			find.Sorts.Add(new ContentsSort {
				SortMode = SortMode.ByCreationDate,
				IsSortAscending = query.IsSortAscending
			});

			if(query == null) {
				find.Filters.Add(new ContentsFilter() { });
				return Ok(await ProcessRequest(
					query, find,
					DefaultContentDTOFactory));
			}

			if(query.CreatorIds != null && query.CreatorIds.Count > 0) {
				if(query.IsCreatorRelated) {
					find.Filters.Add(new CreatorRelatedContentsFilter() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds,
						CreatorNames = query.CreatorNames,
						CreatorSocialIds = query.CreatorSocialIds,
						CreatorSocialUrls = query.CreatorSocialUrls
					});
				} else {
					find.Filters.Add(new CreatorContentsFilter() {
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

			contentsFilter.IsCheckForAffiliations = query.IsCheckCreatorAffiliations;
			contentsFilter.Affiliations = query.CreatorAffiliations;

			find.Filters.Add(contentsFilter);

			return Ok(await ProcessRequest(
				query, find,
				DefaultContentDTOFactory));
		}

		private object DefaultContentDTOFactory(ContentQuery query, Content content)
		{
			string creationDateDisplay = string.Empty;
			string scheduleDateDisplay = string.Empty;

			if(query.Timestamp != DateTimeOffset.MinValue && !string.IsNullOrEmpty(query.Locale)) {
				creationDateDisplay = content.CreationDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
			}

			return new {
				content,
				creationDateDisplay,
				scheduleDateDisplay
			};
		}

		private async Task<List<object>> ProcessRequest<T>(
			ContentQuery query, T find,
			Func<ContentQuery, Content, object> contentFactory)
			where T : FindSettings
		{
			if(contentFactory != null) { throw new NullReferenceException(); }

			List<object> results = new List<object>();
			if(query == null) { return results; }
			else {
				find.Page = query.Page;
				find.PageSize = query.PageSize;
				find.MaxPages = query.MaxPages;
			}

			FindResults<Content> contentFindResults = await contentClient.FindContentsAsync(find);
			await contentFindResults.MoveNextAsync();

			foreach(Content content in contentFindResults.Current) {
				results.Add(contentFactory(query, content));
			}

			return results;
		}
	}
}
