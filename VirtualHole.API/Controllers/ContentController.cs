﻿using System;
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
	public class ContentController : ApiController
    {
		private CreatorClient creatorClient => dbService.Client.Creators;
		private ContentClient contentClient => dbService.Client.Contents;
		private VirtualHoleDBService dbService = null;

		public ContentController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/v1/content")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, Type = typeof(List<ContentDTO>))]
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
			} else {
				settings = new FindContentSettings() { };
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

		private async Task<List<ContentDTO>> InternalListContents<T>(APIQuery query, T request)
			where T : FindContentSettings
		{
			List<ContentDTO> results = new List<ContentDTO>();
			if(query == null) { return results; }

			FindResults<Content> contentFindResults = await contentClient.FindContentsAsync(request.SetPage(query));
			await contentFindResults.MoveNextAsync();

			FindResults<Creator> creatorFindResults = await creatorClient.FindCreatorsAsync(new FindCreatorsStrictSettings {
				Id = new List<string>(contentFindResults.Current.Select(c => c.CreatorId))
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
