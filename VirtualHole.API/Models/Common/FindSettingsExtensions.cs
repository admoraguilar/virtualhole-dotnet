using VirtualHole.DB;

namespace VirtualHole.API.Models
{
	public static class FindSettingsExtensions 
	{
		public static T SetPage<T>(this T settings, APIQuery query)
			where T : FindSettings
		{
			settings.Page = query.Page;
			settings.PageSize = query.PageSize;
			settings.MaxPages = query.MaxPages;
			return settings;
		}
	}
}