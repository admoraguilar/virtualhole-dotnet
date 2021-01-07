using System;
using Newtonsoft.Json.Linq;
using Midnight;

namespace VirtualHole.DB.Contents
{
	public class ContentConverter : PolymorphicObjectConverter<Content>
	{
		public override Content ProcessJObject(JObject jObj)
		{
			Content result = null;

			string socialTypeKey = nameof(Content.SocialType).ToCamelCase();
			string socialType = jObj[socialTypeKey].ToObject<string>();

			string contentTypeKey = nameof(Content.ContentType).ToCamelCase();
			string contentType = jObj[contentTypeKey].ToObject<string>();

			if(socialType == SocialTypes.YouTube && contentType == ContentTypes.Video) { result = new YouTubeVideo(); }
			else if(socialType == SocialTypes.YouTube && contentType == ContentTypes.Broadcast) { result = new YouTubeBroadcast(); }
			else if(socialType == SocialTypes.Twitter && contentType == ContentTypes.Blog) { result = new TwitterTweet(); } 
			else { throw new NotSupportedException(); }

			return result;
		}
	}
}