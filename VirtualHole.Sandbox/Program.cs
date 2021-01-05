using Midnight.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualHole.Sandbox
{
	class Program
	{
		private static void Main(string[] args)
		{
			MLog.Log(MLogLevel.Warning, "Hello World.");
			Console.ReadLine();
		}
	}
}
