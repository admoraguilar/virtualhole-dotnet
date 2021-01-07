using System.Threading.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class BroadcastLiveFilterStep : PipelineStep<FindContext<ContentQuery, Content>>
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