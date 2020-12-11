namespace VirtualHole.API.Models
{
	public class CreatorQuery : PaginatedQuery
	{
		public string Search { get; set; } = string.Empty;
	}
}