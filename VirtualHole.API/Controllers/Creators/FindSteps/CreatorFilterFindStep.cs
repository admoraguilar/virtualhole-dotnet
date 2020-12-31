using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class CreatorFilterFindStep : FindResultsPipelineStep<CreatorsQuery>
	{
		public override async Task ExecuteAsync(FindSettings find, CreatorsQuery query)
		{
			await Task.CompletedTask;

			CreatorRegexFilter creatorRegexFilter = new CreatorRegexFilter();
			creatorRegexFilter.SearchQueries.Add(query.Search);

			find.Filters.Add(creatorRegexFilter);

			CreatorFilter creatorFilter = new CreatorFilter();

			creatorFilter.IsHidden = query.IsHidden;
			creatorFilter.IsCheckForIsGroup = query.IsCheckForIsGroup;
			creatorFilter.IsGroup = query.IsGroup;

			creatorFilter.IsCheckForDepth = query.IsCheckForDepth;
			creatorFilter.Depth = query.Depth;

			creatorFilter.IsCheckForAffiliations = query.IsCheckForAffiliations;
			creatorFilter.IsAffiliationsAll = query.IsAffiliationsAll;
			creatorFilter.IsAffiliationsInclude = query.IsAffiliationsInclude;
			creatorFilter.Affiliations = query.Affiliations;

			find.Filters.Add(creatorFilter);
		}
	}
}