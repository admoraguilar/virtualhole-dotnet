﻿using MongoDB.Bson;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class FindCreatorsStrictSettings : FindCreatorsSettings
	{
		public bool IsAll { get; set; } = false;

		public string Name { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;

		internal override BsonDocument FilterDocument
		{
			get {
				BsonDocument bson = base.FilterDocument;

				if(IsAll) { return bson; }

				if(!string.IsNullOrEmpty(Name)) {
					bson.Add(nameof(Creator.Name).ToCamelCase(), Name);
				}

				if(!string.IsNullOrEmpty(Id)) {
					bson.Add(nameof(Creator.Id).ToCamelCase(), Id);
				}

				return bson;
			}
		}
	}
}
