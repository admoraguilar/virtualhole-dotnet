using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using Humanizer;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public static class ContentDTOPostProcess
	{
		public static async Task<object> ContentDTOFactory(ContentQuery query, Content content)
		{
			HttpClient httpClient = HttpClientFactory.HttpClient;

			string creationDateDisplay = string.Empty;
			string scheduleDateDisplay = string.Empty;

			bool isAvailable = true;
			bool isBroadcast = content.ContentType == ContentTypes.Broadcast;

			if(HasTimestampAndLocale()) {
				creationDateDisplay = content.CreationDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
			}

			if(content.SocialType == SocialTypes.YouTube) {
				// Include a flag that tells if this video is not accessible for some reason.
				// privated, deleted, etc...
				if(content is YouTubeVideo youTubeVideo) {
					// HACK: 
					// * Check for high quality thumbnail
					// * If there's none, switch to MQ.
					// * If there's none, then it's either this video is privated or deleted. Hence mark
					// as unavailable.
					//
					// SOLUTION FUTURE: Should find a better way to do this for better maintainance of the
					// API.
					string defaultThumbnailUrl = youTubeVideo.ThumbnailUrl;
					string hiresThumbnailUrl = defaultThumbnailUrl.Replace("mqdefault", "hq720");

					HttpResponseMessage res = await httpClient.GetAsync(hiresThumbnailUrl);
					youTubeVideo.ThumbnailUrl = hiresThumbnailUrl;

					if(!res.IsSuccessStatusCode) {
						res = await httpClient.GetAsync(defaultThumbnailUrl);
						youTubeVideo.ThumbnailUrl = defaultThumbnailUrl;
					}

					isAvailable = res.IsSuccessStatusCode;
				}

				if(content is YouTubeBroadcast youTubeBroadcast) {
					if(HasTimestampAndLocale()) {
						scheduleDateDisplay = youTubeBroadcast.ScheduleDate.Humanize(query.Timestamp, new CultureInfo(query.Locale));
					}
				}
			}

			if(isBroadcast) {
				return new {
					content,
					isAvailable,
					creationDateDisplay,
					scheduleDateDisplay
				};
			} else {
				return new {
					content,
					isAvailable,
					creationDateDisplay,
				};
			}

			bool HasTimestampAndLocale() => query.Timestamp != DateTimeOffset.MinValue && !string.IsNullOrEmpty(query.Locale);
		}
	}
}