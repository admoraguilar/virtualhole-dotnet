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

	public class CreatorFilterStep : PipelineStep<FindContext<CreatorsQuery, Creator>>
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