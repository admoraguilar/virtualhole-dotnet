using System.Threading.Tasks;

namespace VirtualHole
{
	public sealed class PipelineStepConverter<T, U> : PipelineStep<T>
	{
		public PipelineStepConverter(PipelineStep<U> step)
		{
			Step = step;
		}

		public readonly PipelineStep<U> Step;

		public override async Task ExecuteAsync()
		{
			Step.Context = (U)(object)Context;
			if(await Step.ShouldExecuteAsync()) {
				await Step.ExecuteAsync();
			} else {
				await Task.CompletedTask;
			}
		}
	}
}
