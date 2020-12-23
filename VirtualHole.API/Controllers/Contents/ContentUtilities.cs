using System;
using System.Globalization;
using Humanizer;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public static class ContentUtilities
	{
		public const string AffiliationCommunity = "Community";

		public static object ContentDTOFactory(ContentsQuery query, Content content)
		{
			string creationDateDisplay = string.Empty;
			string scheduleDateDisplay = string.Empty;

			bool isBroadcast = false;

			if(query.Timestamp != DateTimeOffset.MinValue && !string.IsNullOrEmpty(query.Locale)) {
				creationDateDisplay = content.CreationDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
				if(content is YouTubeBroadcast youTubeBroadcast) {
					isBroadcast = true;
					scheduleDateDisplay = youTubeBroadcast.ScheduleDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
				}
			}

			if(isBroadcast) {
				return new {
					content,
					creationDateDisplay,
					scheduleDateDisplay
				};
			} else {
				return new {
					content,
					creationDateDisplay,
				};
			}
		}
	}
}