using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Contents.Creators
{
	public class FindCreatorsStrictSettings : FindCreatorsSettings
	{
		public bool IsAll = false;

		public string UniversalName = string.Empty;
		public string UniversalId = string.Empty;

		protected override BsonDocument CreateFilterDocument()
		{
			BsonDocument filter = new BsonDocument();
			
			if(IsAll) { return filter; }

			if(!string.IsNullOrEmpty(UniversalName)) {
				filter.Add(nameof(Creator.UniversalName).ToCamelCase(), UniversalName);
			}

			if(!string.IsNullOrEmpty(UniversalId)) {
				filter.Add(nameof(Creator.UniversalId).ToCamelCase(), UniversalId);
			}

			filter.AddRange(base.CreateFilterDocument());

			return filter;
		}
	}
}
