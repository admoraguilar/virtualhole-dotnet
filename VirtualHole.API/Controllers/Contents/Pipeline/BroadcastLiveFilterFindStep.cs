using System.Threading.Tasks;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class BroadcastLiveFilterStep : PipelineStep<FindContext<ContentsQuery, Content>>
	{
		public bool IsLive { get; set; } = true;

		public override Task ExecuteAsync()
		{
			Context.InFindSettings.Filters.Add(new BroadcastContentFilter() {
				IsLive = IsLive
			});

			return Task.CompletedTask;
		}
	}
}