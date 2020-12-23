
namespace VirtualHole.API.Models
{
	public class PagedQuery : APIQuery
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public int MaxPages { get; set; } = 50;
	}
}