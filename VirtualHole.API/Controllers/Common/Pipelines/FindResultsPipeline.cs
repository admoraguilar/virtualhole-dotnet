using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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
		public Func<TQuery, TResult, object> ResultFactory { get; set; } = null;

		public async Task<List<object>> ExecuteAsync()
		{
			if(Query == null || Steps == null ||
			   FindProvider == null || ResultFactory == null) {
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

			foreach(TResult findResult in findResults.Current) {
				results.Add(ResultFactory(Query, findResult));
			}

			return results;
		}
	}

	public abstract class FindResultsPipelineStep<TQuery>
		where TQuery : APIQuery
	{
		public abstract Task ExecuteAsync(FindSettings find, TQuery query);
	}
}