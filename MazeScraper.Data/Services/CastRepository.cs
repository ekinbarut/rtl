namespace MazeScraper.Data;

public class CastRepository : ICastRepository
{
    private readonly IMongoCollection<Cast> _collection;

    public CastRepository(MongoDbContext dbContext)
    {
        _collection = dbContext.Cast;
    }

    public async Task BulkUpsertAsync(List<Cast> castList)
    {
        if (castList == null || castList.Count == 0) return;

        var writes = new List<WriteModel<Cast>>();
        foreach (var cast in castList)
        {
            // Let's assume "Id" from TVMaze is globally unique
            var filter = Builders<Cast>.Filter.Eq(c => c.Id, cast.Id);

            var replaceOne = new ReplaceOneModel<Cast>(filter, cast)
            {
                IsUpsert = true
            };
            writes.Add(replaceOne);
        }

        await _collection.BulkWriteAsync(writes);
    }
}