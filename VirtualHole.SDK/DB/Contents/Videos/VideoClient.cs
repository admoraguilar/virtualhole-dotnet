using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using VirtualHole.Common;

namespace VirtualHole.DB.Contents.Videos
{
	using Common;
	
	public class VideoClient
	{
		internal const string VideosCollectionName = "videos";

		private IMongoDatabase database = null;

		private IMongoCollection<Video> videoCollection = null;
		private IMongoCollection<Broadcast> broadcastCollection = null;
		private IMongoCollection<BsonDocument> videoBsonCollection = null;

		private Dictionary<Type, object> collectionsCache = new Dictionary<Type, object>();

		internal VideoClient(IMongoDatabase database)
		{
			this.database = database;
			videoCollection = this.database.GetCollection<Video>(VideosCollectionName);
			broadcastCollection = this.database.GetCollection<Broadcast>(VideosCollectionName);
			videoBsonCollection = this.database.GetCollection<BsonDocument>(VideosCollectionName);

			collectionsCache = new Dictionary<Type, object> {
				{ typeof(Video), videoCollection },
				{ typeof(Broadcast), broadcastCollection }
			};
		}

		public async Task<FindResults<T>> FindVideosAsync<T>(
			FindSettings<T> settings, CancellationToken cancellationToken = default) where T : Video
		{
			return new FindResults<T>(await DBUtilities.FindAsync(
				(IMongoCollection<T>)collectionsCache[typeof(T)],
				settings, cancellationToken));
		}

		public async Task UpsertVideoAsync<T>(
			T video, CancellationToken cancellationToken = default) where T : Video
		{
			await DBUtilities.UpsertAsync(
				(IMongoCollection<T>)collectionsCache[typeof(T)],
				new BsonDocument
				{
					{ nameof(Content.Id).ToCamelCase(), video.Id },
					{ nameof(Content.Platform).ToCamelCase(), (int)video.Platform }
				},
				video, cancellationToken);
		}

		public async Task UpsertManyVideosAndDeleteDanglingAsync<T>(
			IEnumerable<T> videos, CancellationToken cancellationToken = default) where T : Video
		{
			await DBUtilities.UpsertManyAndDeleteDanglingAsync(
				videoBsonCollection,
				(T video) => new BsonDocument(nameof(Content.Id).ToCamelCase(), video.Id),
				videos, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}

		public async Task UpsertManyVideosAsync<T>(
			IEnumerable<T> videos, CancellationToken cancellationToken = default) where T : Video
		{
			await DBUtilities.UpsertManyAsync(
				videoBsonCollection,
				(T video) => new BsonDocument(nameof(Content.Id).ToCamelCase(), video.Id),
				videos, DateTimeOffset.UtcNow.DateTime, cancellationToken);
		}
	}
}
