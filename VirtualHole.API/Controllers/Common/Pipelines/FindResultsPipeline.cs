using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Tasks;
using VirtualHole.DB;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class FindResultsPipeline<TQuery, TResult>
		where TQuery : APIQuery
	{
		public TQuery Query { get; set; } = null;
		public List<FindResultsPipelineStep<TQuery>> Steps { get; set; } = new List<FindResultsPipelineStep<TQuery>>();
		public Func<FindSettings, Task<FindResults<TResult>>> FindProvider { get; set; } = null;
		public Func<TQuery, TResult, Task<object>> PostProcessFactory { get; set; } = null;

		public async Task<List<object>> ExecuteAsync()
		{
			if(Query == null || Steps == null ||
			   FindProvider == null) {
				throw new NullReferenceException();
			}

			if(Steps.Count <= 0) {
				throw new InvalidOperationException("Steps should be provided.");
			}

			FindSettings find = new FindSettings();
			foreach(FindResultsPipelineStep<TQuery> step in Steps) {
				await step.ExecuteAsync(find, Query);
			}

			List<object> results = new List<object>();

			FindResults<TResult> findResults = await FindProvider(find);
			await findResults.MoveNextAsync();

			await Concurrent.ForEachAsync(findResults.Current, async (TResult findResult) => {
				object result = findResult;
				if(PostProcessFactory != null) { result = await PostProcessFactory(Query, findResult); }
				if(result != null) { results.Add(result); }
			});

			return results;
		}
	}

	public abstract class FindResultsPipelineStep<TQuery>
		where TQuery : APIQuery
	{
		public abstract Task ExecuteAsync(FindSettings find, TQuery query);
	}
}