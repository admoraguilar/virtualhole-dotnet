using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Midnight;

namespace VirtualHole.DB.Creators
{
	public class CreatorsClient
	{
		internal const string CreatorsCollectionName = "creators";

		private IMongoDatabase database = null;
		private IMongoCollection<Creator> creatorsCollection = null;

		internal CreatorsClient(IMongoDatabase database)
		{
			this.database = database;
			creatorsCollection = this.database.GetCollection<Creator>(CreatorsCollectionName);
		}

		public async Task<List<Creator>> FindAllAsync(CancellationToken cancellationToken = default)
		{
			return await MongoDBUtilities.FindAllAsync(creatorsCollection, cancellationToken);
		}

		public async Task<FindResults<Creator>> FindAsync(
			FindSettings settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Creator>(
				await MongoDBUtilities.FindAsync(creatorsCollection, settings, cancellationToken));
		}

		public async Task UpsertManyAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				creatorsCollection,
				(Creator creator) => new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creators, cancellationToken);
		}

		public async Task DeleteManyAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.DeleteManyAsync(
				creatorsCollection,
				(Creator creator) => new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creators, cancellationToken);
		}
	}
}
