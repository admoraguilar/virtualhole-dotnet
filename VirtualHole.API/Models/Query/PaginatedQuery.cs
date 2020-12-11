
namespace VirtualHole.API.Models
{
	public class PaginatedQuery
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public int MaxPages { get; set; } = 50;
	}

	public class CreatorQuery
	{
		public string Search { get; set; } = string.Empty;
	}
}