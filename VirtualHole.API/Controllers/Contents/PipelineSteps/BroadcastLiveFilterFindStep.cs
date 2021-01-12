using System;
using System.Threading.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class BroadcastFilterStep : PipelineStep<FindContext<ContentQuery, Content>>
	{
		public bool IsLive { get; set; } = true;
		public DateTimeOffset UntilDate { get; set; } = DateTimeOffset.UtcNow;

		public override Task ExecuteAsync()
		{
			Context.InFindSettings.Filters.Add(new BroadcastContentFilter() {
				IsLive = IsLive,
				UntilDate = UntilDate
			});

			return Task.CompletedTask;
		}
	}
}