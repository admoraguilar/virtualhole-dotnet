using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class CreatorStrictFilter : FindFilter
	{
		public List<string> Id { get; set; } = new List<string>();
		public List<string> Name { get; set; } = new List<string>();

		internal override IEnumerable<Type> ConflictingTypes => new Type[] {
			typeof(CreatorRegexFilter)
		};

		internal override BsonDocument Document 
		{
			get {
				BsonDocument bson = new BsonDocument();

				if(Id != null && Id.Count > 0) {
					bson.Add(
						nameof(Creator.Id).ToCamelCase(),
						new BsonDocument("$in", new BsonArray(Id)));
				}

				if(Name != null && Name.Count > 0) {
					bson.Add(
						nameof(Creator.Name).ToCamelCase(), 
						new BsonDocument("$in", new BsonArray(Name)));
				}

				return bson;
			}
		}
	}
}
