using System.Threading.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class CreatorSortStep : PipelineStep<FindContext<CreatorQuery, Creator>>
	{
		public override Task ExecuteAsync()
		{
			Context.InFindSettings.Sorts.Add(new CreatorSort());
			return Task.CompletedTask;
		}
	}
}