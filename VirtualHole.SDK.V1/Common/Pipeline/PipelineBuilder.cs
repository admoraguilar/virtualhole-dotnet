using System.Threading.Tasks;
using System;

namespace VirtualHole
{
	public sealed class PipelineBuilder<T> : PipelineStep<T>
	{
		public PipelineBuilder(T payload) {
			pipeline = new Pipeline<T>(payload);
			Context = pipeline.Context;
		}

		private Pipeline<T> pipeline = null;

		public Pipeline<T> Add(PipelineStep<T> step)
		{
			return pipeline.AddStep(step);
		}

		public Pipeline<T> AddConverted<U>(PipelineStep<U> step)
		{
			return pipeline.AddStep(new PipelineStepConverter<T, U>(step));
		}

		public Pipeline<T> AddAction(Func<T, Task<bool>> shouldExecuteAsync, Func<T, Task> executeAsync)
		{
			return pipeline.AddStep(new PipelineStepAction<T>(shouldExecuteAsync, executeAsync));
		}

		public Pipeline<T> AddAction(Func<T, Task> executeAsync)
		{
			return pipeline.AddStep(new PipelineStepAction<T>(executeAsync));
		}

		public override async Task ExecuteAsync()
		{
			await pipeline.ExecuteAsync();
		}
	}
}
