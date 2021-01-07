using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Tasks;
using VirtualHole.DB;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
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
				if(Context.InPostProcess != null) { result = await Context.InPostProcess(Context.InQuery, findResult); }
				if(result != null) { results.Add(result); }
			});

			Context.OutResults = results;
		}
	}
}