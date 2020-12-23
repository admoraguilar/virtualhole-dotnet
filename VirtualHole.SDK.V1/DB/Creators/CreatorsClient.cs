using System;
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
		private IMongoCollection<BsonDocument> creatorsBsonCollection = null;

		internal CreatorsClient(IMongoDatabase database)
		{
			this.database = database;
			creatorsCollection = this.database.GetCollection<Creator>(CreatorsCollectionName);
			creatorsBsonCollection = this.database.GetCollection<BsonDocument>(CreatorsCollectionName);
		}

		public async Task<FindResults<Creator>> FindCreatorsAsync(
			FindSettings settings = default, CancellationToken cancellationToken = default)
		{
			return new FindResults<Creator>(
				await MongoDBUtilities.FindAsync(creatorsCollection, settings, cancellationToken));
		}

		public async Task UpsertCreatorAsync(
			Creator creator, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertAsync(
				creatorsCollection,
				new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creator, cancellationToken);
		}

		public async Task UpsertManyCreatorsAndDeleteDanglingAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAndDeleteDanglingAsync(
				creatorsBsonCollection,
				(Creator creator) => new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creators, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}

		public async Task UpsertManyCreatorsAsync(
			IEnumerable<Creator> creators, CancellationToken cancellationToken = default)
		{
			await MongoDBUtilities.UpsertManyAsync(
				creatorsBsonCollection,
				(Creator creator) => new BsonDocument(nameof(Creator.Id).ToCamelCase(), creator.Id),
				creators, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}	
	}
}
