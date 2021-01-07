using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Pipeline;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class ContentFilterStep : PipelineStep<FindContext<ContentQuery, Content>>
	{
		private CreatorsClient creatorsClient;

		public ContentFilterStep(CreatorsClient creatorsClient)
		{
			this.creatorsClient = creatorsClient;
		}

		public override async Task ExecuteAsync()
		{
			if(Context.InQuery.CreatorIds != null && Context.InQuery.CreatorIds.Count > 0) {
				if(Context.InQuery.IsCreatorRelated) {
					// We load the creators here for additional information so that the client only needs 
					// to pass in the creator id. This is so the client doesn't need to include names and 
					// social urls anymore that could lengthen the Context.InQuery URL; making it easy to hit the 
					// character limit. This will mostly be used on the "followed feed tab"
					//
					// TODO: This could be optimized in the future though, as right now it always
					// pulls fresh from the creator DB.
					IEnumerable<Creator> creators = await GetCreatorsFromId(Context.InQuery.CreatorIds);
					Context.InFindSettings.Filters.Add(new CreatorRelatedContentFilter() {
						IsCreatorsInclude = Context.InQuery.IsCreatorsInclude,
						CreatorIds = Context.InQuery.CreatorIds,
						CreatorNames = creators.Select(c => c.Name).ToList(),
						CreatorSocialIds = creators.SelectMany(c => c.Socials.Select(s => s.Id)).ToList(),
					});
				} else {
					Context.InFindSettings.Filters.Add(new CreatorContentFilter() {
						IsCreatorsInclude = Context.InQuery.IsCreatorsInclude,
						CreatorIds = Context.InQuery.CreatorIds
					});
				}
			}

			ContentFilter contentsFilter = new ContentFilter();

			if(Context.InQuery.SocialType != null && Context.InQuery.SocialType.Count > 0) {
				contentsFilter.IsSocialTypeInclude = Context.InQuery.IsSocialTypeInclude;
				contentsFilter.SocialType = Context.InQuery.SocialType;
			}

			if(Context.InQuery.ContentType != null && Context.InQuery.ContentType.Count > 0) {
				contentsFilter.IsContentTypeInclude = Context.InQuery.IsContentTypeInclude;
				contentsFilter.ContentType = Context.InQuery.ContentType;
			}

			contentsFilter.IsCheckForAffiliations = Context.InQuery.IsCheckCreatorAffiliations;
			contentsFilter.IsAffiliationsAll = Context.InQuery.IsAffiliationsAll;
			contentsFilter.IsAffiliationsInclude = Context.InQuery.IsAffiliationsInclude;
			contentsFilter.Affiliations = Context.InQuery.CreatorAffiliations;

			Context.InFindSettings.Filters.Add(contentsFilter);
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