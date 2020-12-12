
using System;

namespace VirtualHole.API.Models
{
	public class APIQuery
	{
		public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.MinValue;
		public string Locale { get; set; } = string.Empty;

		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public int MaxPages { get; set; } = 50;
	}
}