using System;
using System.IO;
using System.Diagnostics;

namespace VirtualHole.Scraper
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			Console.WriteLine($"Scraper Start: {path}");
			Console.ReadLine();
		}
	}
}
