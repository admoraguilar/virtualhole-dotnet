using System;
using Newtonsoft.Json.Linq;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class CreatorSocialConverter : PolymorphicObjectConverter<CreatorSocial>
	{
		public override CreatorSocial ProcessJObject(JObject jObj)
		{
			CreatorSocial result = null;

			string socialTypeKey = nameof(CreatorSocial.SocialType).ToCamelCase();
			string socialType = jObj[socialTypeKey].ToObject<string>();

			if(socialType == SocialTypes.YouTube) { result = new YouTubeSocial(); } 
			else if(socialType == SocialTypes.Twitter) { result = new TwitterSocial(); }
			else { throw new NotSupportedException(); }

			return result;
		}
	}
}
