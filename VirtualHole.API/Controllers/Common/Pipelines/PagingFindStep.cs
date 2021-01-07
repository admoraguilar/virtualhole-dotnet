using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Tasks;
using VirtualHole.DB;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class PagingFindStep<TQuery> : FindResultsPipelineStep<TQuery>
		where TQuery : PagedQuery
	{
		public override async Task ExecuteAsync(FindSettings find, TQuery query)
		{
			await Task.CompletedTask;

			find.Page = query.Page;
			find.PageSize = query.PageSize;
			find.MaxPages = query.MaxPages;
		}
	}

	public class FindContext<TQuery, TResult>
		where TQuery : PagedQuery
	{

		public Func<FindSettings, Task<FindResults<TResult>>> InProvider { get; set; } = null;
		public Func<TResult, Task<object>> InPostProcess { get; set; } = null;

		public TQuery InQuery { get; set; } = null;
		public FindSettings InFindSettings { get; set; } = new FindSettings();

		public List<object> OutResults { get; set; } = new List<object>();
	}

	public class GetPagedResultsStep<TQuery, TResult> : PipelineStep<FindContext<TQuery, TResult>>
		where TQuery : PagedQuery
	{
		public override async Task ExecuteAsync()
		{
			Context.InFindSettings.Page = Context.InQuery.Page;
			Context.InFindSettings.PageSize = Context.InQuery.PageSize;
			Context.InFindSettings.MaxPages = Context.InQuery.MaxPages;

			List<object> results = new List<object>();

			FindResults<TResult> findResults = await Context.InProvider(Context.InFindSettings);
			await findResults.MoveNextAsync();

			await Concurrent.ForEachAsync(findResults.Current, async (TResult findResult) => {
				object result = findResult;
				if(Context.InPostProcess != null) { result = await Context.InPostProcess(findResult); }
				if(result != null) { results.Add(result); }
			});

			Context.OutResults = results;
		}
	}
}