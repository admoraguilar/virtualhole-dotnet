using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class BroadcastLiveFilterFindStep : FindResultsPipelineStep<ContentsQuery>
	{
		public bool IsLive { get; set; } = true;

		public override async Task ExecuteAsync(FindSettings find, ContentsQuery query)
		{
			await Task.CompletedTask;

			find.Filters.Add(new BroadcastContentFilter() {
				IsLive = IsLive
			});
		}
	}
}