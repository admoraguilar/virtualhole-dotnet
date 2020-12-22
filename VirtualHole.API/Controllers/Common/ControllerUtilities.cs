using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using VirtualHole.DB;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	internal static class ControllerUtilities
	{
		public static async Task<List<object>> ProcessPagedQuery<TQuery, TFind, TResult>(
			TQuery query, TFind find,
			Func<Task<FindResults<TResult>>> findProvider, 
			Func<TQuery, TResult, object> resultFactory)
			where TQuery : PagedQuery
			where TFind : FindSettings
		{
			List<object> results = new List<object>();
			if(query != null) {
				find.Page = query.Page;
				find.PageSize = query.PageSize;
				find.MaxPages = query.MaxPages;
			}

			FindResults<TResult> findResults = await findProvider();
			await findResults.MoveNextAsync();

			foreach(TResult findResult in findResults.Current) {
				results.Add(resultFactory(query, findResult));
			}

			return results;
		}
	}
}