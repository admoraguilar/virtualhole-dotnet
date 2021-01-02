using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace VirtualHole.API.Controllers
{
	public class ContentFilterFindStep : FindResultsPipelineStep<ContentsQuery>
	{
		private CreatorsClient creatorsClient;

		public ContentFilterFindStep(CreatorsClient creatorsClient)
		{
			this.creatorsClient = creatorsClient;
		}

		public override async Task ExecuteAsync(FindSettings find, ContentsQuery query)
		{
			await Task.CompletedTask;

			if(query.CreatorIds != null && query.CreatorIds.Count > 0) {
				if(query.IsCreatorRelated) {
					// We load the creators here for additional information so that the client only needs 
					// to pass in the creator id. This is so the client doesn't need to include names and 
					// social urls anymore that could lengthen the query URL; making it easy to hit the 
					// character limit. This will mostly be used on the "followed feed tab"
					//
					// TODO: This could be optimized in the future though, as right now it always
					// pulls fresh from the creator DB.

					IEnumerable<Creator> creators = await GetCreatorsFromId(query.CreatorIds);
					find.Filters.Add(new CreatorRelatedContentFilter() {
						IsCreatorsInclude = query.IsCreatorsInclude,
						CreatorIds = query.CreatorIds,
						CreatorNames = creators.Select(c => c.Name).ToList(),
						CreatorSocialIds = creators.SelectMany(c => c.Socials.Select(s => s.Id)).ToList(),
						CreatorSocialUrls = creators.SelectMany(c => c.Socials.Select(s => s.Url)).ToList()
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
		}

		private async Task<IEnumerable<Creator>> GetCreatorsFromId(List<string> creatorIds)
		{
			FindSettings creatorFind = new FindSettings();
			creatorFind.Page = 1;
			creatorFind.PageSize = 500;
			creatorFind.MaxPages = 1;

			creatorFind.Filters.Add(new CreatorStrictFilter() {
				Id = new List<string>(creatorIds),
			});
			FindResults<Creator> creatorFindResults = await creatorsClient.FindCreatorsAsync(creatorFind);
			await creatorFindResults.MoveNextAsync();

			return creatorFindResults.Current;
		}
	}
}