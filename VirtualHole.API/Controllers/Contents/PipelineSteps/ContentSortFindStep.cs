using System.Threading.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class ContentSortStep : PipelineStep<FindContext<ContentQuery, Content>>
	{
		public override Task ExecuteAsync()
		{
			Context.InFindSettings.Sorts.Add(new ContentSort() {
				SortMode = SortMode.ByCreationDate,
				IsSortAscending = Context.InQuery.IsSortAscending
			});

			return Task.CompletedTask;
		}
	}
}