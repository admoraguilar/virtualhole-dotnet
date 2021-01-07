using System;
using System.Threading.Tasks;

namespace VirtualHole
{
	public sealed class PipelineStepAction<T> : PipelineStep<T>
	{
		public PipelineStepAction(Func<T, Task> executeAsync, Func<T, Task<bool>> shouldExecuteAsync = null)
		{
			this.shouldExecuteAsyncFunc = shouldExecuteAsync;
			this.executeAsyncFunc = executeAsync;
		}

		private Func<T, Task<bool>> shouldExecuteAsyncFunc;
		private Func<T, Task> executeAsyncFunc;

		public override async Task<bool> ShouldExecuteAsync()
		{
			if(shouldExecuteAsyncFunc != null) {
				return await shouldExecuteAsyncFunc(Context);
			} else {
				return true;
			}
		}

		public override async Task ExecuteAsync()
		{
			await executeAsyncFunc(Context);
		}
	}
}
