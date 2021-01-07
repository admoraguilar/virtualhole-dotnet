using System;
using System.Threading.Tasks;

namespace VirtualHole
{
	public sealed class PipelineStepConverter<T, U> : PipelineStep<T>
	{
		public PipelineStepConverter(Func<T, U> contextConverter, PipelineStep<U> step)
		{
			ContextConverter = contextConverter;
			Step = step;
		}

		public readonly Func<T, U> ContextConverter;
		public readonly PipelineStep<U> Step;

		public override async Task ExecuteAsync()
		{
			Step.Context = ContextConverter(Context);
			if(await Step.ShouldExecuteAsync()) {
				await Step.ExecuteAsync();
			} else {
				await Task.CompletedTask;
			}
		}
	}
}
