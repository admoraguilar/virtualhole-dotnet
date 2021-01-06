using System.Threading.Tasks;
using System.Collections.Generic;

namespace VirtualHole
{
	public sealed class Pipeline<T> : PipelineStep<T>
	{
		public Pipeline(T payload)
		{
			Context = payload;
		}

		public override async Task<bool> ShouldExecuteAsync() => await Task.FromResult(true);

		public IReadOnlyList<PipelineStep<T>> Steps => steps;
		private List<PipelineStep<T>> steps = new List<PipelineStep<T>>();

		public Pipeline<T> AddStep(PipelineStep<T> step)
		{
			steps.Add(step);
			return this;
		}

		public Pipeline<T> RemoveStep(PipelineStep<T> step)
		{
			steps.Remove(step);
			return this;
		}

		public override async Task ExecuteAsync()
		{
			foreach(PipelineStep<T> step in Steps) {
				step.Context = Context;

				if(await step.ShouldExecuteAsync()) {
					await step.ExecuteAsync();
				}
			}
		}
	}
}
