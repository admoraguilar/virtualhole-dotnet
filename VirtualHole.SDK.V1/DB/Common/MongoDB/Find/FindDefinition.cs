using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace VirtualHole.DB
{
	public abstract class FindDefinition
	{
		internal abstract BsonDocument Document { get; }
		internal virtual IEnumerable<Type> ConflictingTypes => new Type[0];
	}
}
