using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class ContentFilterFindStep : FindResultsPipelineStep<ContentsQuery>
	{
		public override async Task ExecuteAsync(FindSettings find, ContentsQuery query)
		{
			await Task.CompletedTask;

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
		}
	}
}