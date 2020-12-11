using System.Collections.Generic;

namespace VirtualHole.API.Models
{
	public class ContentQuery : PaginatedQuery
	{
		public (bool, List<string>) SocialType { get; set; } = (false, new List<string>());
		public (bool, List<string>) ContentType { get; set; } = (false, new List<string>());
	}
}