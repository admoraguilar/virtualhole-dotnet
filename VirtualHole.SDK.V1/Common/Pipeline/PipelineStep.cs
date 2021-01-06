using System.Threading.Tasks;

namespace VirtualHole
{
	public abstract class PipelineStep<T>
	{
		public T Result => Context;
		protected internal T Context { get; internal set; } = default;

		public virtual Task<bool> ShouldExecuteAsync() => Task.FromResult(true);
		public abstract Task ExecuteAsync();
	}
}
