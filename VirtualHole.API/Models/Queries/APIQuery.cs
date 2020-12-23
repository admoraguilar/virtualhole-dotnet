
using System;

namespace VirtualHole.API.Models
{
	public class APIQuery
	{
		public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.MinValue;
		public string Locale { get; set; } = string.Empty;
	}
}