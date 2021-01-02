using System.Linq;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Midnight;
using VirtualHole.DB.Creators;

namespace VirtualHole.DB.Contents
{
	public class CreatorRelatedContentFilter : FindFilter
	{
		public bool IsCreatorsInclude { get; set; } = true;
		public List<string> CreatorIds { get; set; } = new List<string>();
		public List<string> CreatorNames { get; set; } = new List<string>();
		public List<string> CreatorSocialIds { get; set; } = new List<string>();

		internal override IEnumerable<Type> ConflictingTypes => new Type[] {
			typeof(CreatorContentFilter)
		};

		internal override BsonDocument Document
		{
			get {
				BsonDocument bson = new BsonDocument();

				BsonArray andExpressions = new BsonArray();
				andExpressions.Add(
					new BsonDocument(
						$"{nameof(Content.Creator).ToCamelCase()}.{nameof(CreatorSimple.Id).ToCamelCase()}",
						new BsonDocument("$not", new BsonDocument(IsCreatorsInclude ? "$in" : "$nin", new BsonArray(CreatorIds)))));
				andExpressions.Add(
					new BsonDocument("$text",
						new BsonDocument("$search", Join(Concat(CreatorNames, CreatorSocialIds)))));

				bson.AddRange(new BsonDocument("$and", andExpressions));

				return bson;

				string Join(IEnumerable<string> values) =>
					string.Join(" ", values);

				IEnumerable<string> Concat(params IEnumerable<string>[] enumerables) =>
					enumerables.SelectMany(e => e);
			}
		}
	}
}
