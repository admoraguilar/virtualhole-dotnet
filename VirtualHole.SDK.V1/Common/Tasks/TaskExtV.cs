using System;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualHole
{
	public static class TaskExtV
	{
		public static async Task Timeout(Task task, TimeSpan timeout, CancellationToken cancellationToken = default)
		{
			Task timeoutTask = Task.Delay(timeout, cancellationToken);
			try {
				Task done = await Task.WhenAny(task, timeoutTask);
				cancellationToken.ThrowIfCancellationRequested();
				if(done == timeoutTask) {
					throw new TimeoutException();
				}
			} catch (OperationCanceledException) {
				throw;
			}
		}
	}
}
