using System;
using System.Threading.Tasks;
using Midnight.Logs;

namespace VirtualHole.Sandbox
{
	class Program
	{
		private static async Task Main(string[] args)
		{
			PipelineBuilder<CounterPayload> pipelineBuilder = new PipelineBuilder<CounterPayload>(new CounterPayload());
			pipelineBuilder.Add(new IncrementCounter());
			pipelineBuilder.Add(new CreateCounterMessage());
			pipelineBuilder.AddConverted(new LogOutput());
			pipelineBuilder.AddAction((CounterPayload context) => {
				MLog.Log($"Additional action: {context.Counter}");
				return Task.CompletedTask;
			});
			await pipelineBuilder.ExecuteAsync();

			//Pipeline<CounterPayload> pipeline = new Pipeline<CounterPayload>(new CounterPayload());
			//pipeline.AddStep(new IncrementCounter())
			//	.AddStep(new CreateCounterMessage())
			//	.AddStep(new PipelineStepConverter<CounterPayload, IOutputLog>(
			//		new LogOutput()
			//		))
			//	.AddStep(new PipelineStepAction<CounterPayload>((CounterPayload context) => {
			//		MLog.Log("Additional action");
			//		return Task.CompletedTask;
			//	}));
			//await pipeline.ExecuteAsync();

			MLog.Log(MLogLevel.Warning, $"Hello World. {pipelineBuilder.Result.Counter}");
			Console.ReadLine();
		}
	}

	class CounterPayload : IOutputLog
	{
		public int Counter { get; set; }
		public string Message { get; set; }
	}

	class IncrementCounter : PipelineStep<CounterPayload>
	{
		public override Task ExecuteAsync()
		{
			Context.Counter += 3;
			return Task.CompletedTask;
		}
	}

	class CreateCounterMessage : PipelineStep<CounterPayload>
	{
		public override Task ExecuteAsync()
		{
			Context.Message = $"Counter result: {Context.Counter}";
			return Task.CompletedTask;
		}
	}

	public interface IOutputLog
	{
		string Message { get; set; }
	}

	class LogOutput : PipelineStep<IOutputLog>
	{
		public override Task ExecuteAsync()
		{
			MLog.Log(Context.Message);
			return Task.CompletedTask;
		}
	}
}

