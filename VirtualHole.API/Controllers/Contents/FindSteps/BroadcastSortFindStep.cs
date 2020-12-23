using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class BroadcastSortFindStep : FindResultsPipelineStep<ContentsQuery>
	{
		public override async Task ExecuteAsync(FindSettings find, ContentsQuery query)
		{
			await Task.CompletedTask;

			find.Sorts.Add(new ContentSort {
				SortMode = SortMode.BySchedule,
				IsSortAscending = query.IsSortAscending
			});
		}
	}
}