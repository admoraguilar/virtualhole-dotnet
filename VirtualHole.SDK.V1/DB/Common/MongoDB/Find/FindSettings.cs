using System;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;

namespace VirtualHole.DB
{
	public class FindSettings
	{
		public List<FindFilter> Filters { get; set; } = new List<FindFilter>();
		public List<FindSort> Sorts { get; set; } = new List<FindSort>();

		public int PageSize 
		{
			get => pageSize;
			set { pageSize = Math.Max(0, value); }
		}
		private int pageSize = 20;

		public int MaxPages 
		{
			get => maxPages;
			set { maxPages = Math.Max(0, value); }
		}
		private int maxPages = 30;

		public int Page 
		{
			get => page;
			set { page = Math.Max(1, value); }
		}
		private int page = 1;

		internal BsonDocument FilterDocument => MergeFindDefinitions(Filters);

		internal BsonDocument SortDocument => MergeFindDefinitions(Sorts);

		public int GetBatchSize() => PageSize;
		public int GetResultsLimit() => PageSize * MaxPages;
		public int GetSkip() => Math.Min(GetResultsLimit(), (Page - 1) * PageSize);

		private BsonDocument MergeFindDefinitions(IEnumerable<FindDefinition> definitions)
		{
			BsonDocument bson = new BsonDocument();
			if(definitions != null) {
				foreach(FindDefinition definition in definitions) {
					IEnumerable<Type> allConflicting = definitions.SelectMany(d => { 
						if(d != definition){ return d.ConflictingTypes; }
						else { return new Type[0]; }
					});

					IEnumerable<Type> intersectConflicting = definition
						.ConflictingTypes.Intersect(allConflicting);

					if(intersectConflicting.Count() > 0) {
						string errorMessage = $"[FindSettings] [{definition.GetType().Name}] definition has conflicts with [.";
						foreach(Type type in intersectConflicting) {
							errorMessage += type.Name;
						}
						errorMessage += "].";
						throw new InvalidOperationException(errorMessage);
					}

					bson.Merge(definition.Document);
				}
			}
			return bson;
		}
	}
}