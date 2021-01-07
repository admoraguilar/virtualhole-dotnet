using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VirtualHole
{
	public sealed class Pipeline<TContext> : PipelineStep<TContext>
	{
		public Pipeline(TContext context)
		{
			SetContext(context);
		}

		public IReadOnlyList<PipelineStep<TContext>> Steps => steps;
		private List<PipelineStep<TContext>> steps = new List<PipelineStep<TContext>>();

		public override async Task<bool> ShouldExecuteAsync() => await Task.FromResult(true);

		public void SetContext(TContext context)
		{
			Context = context;
		}

		public Pipeline<TContext> Add(PipelineStep<TContext> step)
		{
			steps.Add(step);
			return this;
		}

		public Pipeline<TContext> Add<T>(Func<TContext, T> inConverter, PipelineStep<T> step)
		{
			steps.Add(new PipelineStepConverter<TContext, T>(inConverter, step));
			return this;
		}

		public Pipeline<TContext> Add(Func<TContext, Task> executeAsync, Func<TContext, Task<bool>> shouldExecuteAsync = null)
		{
			steps.Add(new PipelineStepAction<TContext>(executeAsync, shouldExecuteAsync));
			return this;
		}

		public Pipeline<TContext> Remove(PipelineStep<TContext> step)
		{
			steps.Remove(step);
			return this;
		}

		public override async Task ExecuteAsync()
		{
			foreach(PipelineStep<TContext> step in Steps) {
				step.Context = Context;

				if(await step.ShouldExecuteAsync()) {
					await step.ExecuteAsync();
				}
			}
		}
	}
}
