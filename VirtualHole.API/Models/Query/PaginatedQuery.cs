using VirtualHole.DB;

namespace VirtualHole.API.Models
{
	public static class FindSettingsExtensions 
	{
		public static T SetPage<T>(this T settings, PaginatedQuery query)
			where T : FindSettings
		{
			settings.Page = query.Page;
			settings.PageSize = query.PageSize;
			settings.MaxPages = query.MaxPages;
			return settings;
		}
	}

	public class PaginatedQuery
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public int MaxPages { get; set; } = 50;
	}

	public class CreatorQuery : PaginatedQuery
	{
		public string Search { get; set; } = string.Empty;
	}
}