using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class CreatorSort : FindSort
	{
		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();
				bson.Add(nameof(Creator.Name).ToCamelCase(), 1);
				return bson;
			}
		}
	}
}
