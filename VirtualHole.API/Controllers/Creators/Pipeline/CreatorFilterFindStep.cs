using System.Threading.Tasks;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class CreatorFilterStep : PipelineStep<FindContext<CreatorQuery, Creator>>
	{
		public override Task ExecuteAsync()
		{
			CreatorRegexFilter creatorRegexFilter = new CreatorRegexFilter();
			creatorRegexFilter.SearchQueries.Add(Context.InQuery.Search);

			Context.InFindSettings.Filters.Add(creatorRegexFilter);

			CreatorFilter creatorFilter = new CreatorFilter();

			creatorFilter.IsHidden = Context.InQuery.IsHidden;
			creatorFilter.IsCheckForIsGroup = Context.InQuery.IsCheckForIsGroup;
			creatorFilter.IsGroup = Context.InQuery.IsGroup;

			creatorFilter.IsCheckForDepth = Context.InQuery.IsCheckForDepth;
			creatorFilter.Depth = Context.InQuery.Depth;

			creatorFilter.IsCheckForAffiliations = Context.InQuery.IsCheckForAffiliations;
			creatorFilter.IsAffiliationsAll = Context.InQuery.IsAffiliationsAll;
			creatorFilter.IsAffiliationsInclude = Context.InQuery.IsAffiliationsInclude;
			creatorFilter.Affiliations = Context.InQuery.Affiliations;

			Context.InFindSettings.Filters.Add(creatorFilter);

			return Task.CompletedTask;
		}
	}
}