
namespace VirtualHole.API.Models
{
	public class CreatorQuery : PagedQuery
	{
		public string Search { get; set; } = string.Empty;
	}
}