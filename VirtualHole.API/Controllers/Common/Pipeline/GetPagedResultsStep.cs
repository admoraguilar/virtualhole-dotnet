using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Tasks;
using Midnight.Pipeline;
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

			FindResults<TResult> findResults = await Context.InProvider(Context.InFindSettings);
			await findResults.MoveNextAsync();

			List<TResult> findResultsList = findResults.Current.ToList();
			List<object> results = new List<object>(new object[findResultsList.Count]);

			await Concurrent.ForAsync(0, results.Count, async (int i) => {
				object result = null;

				if(Context.InPostProcess != null) { result = await Context.InPostProcess(Context.InQuery, findResultsList[i]); }
				else { result = findResultsList[i]; }

				if(result != null) { results[i] = result; }
			});

			results.RemoveAll(r => r == null);

			Context.OutResults = results;
		}
	}
}