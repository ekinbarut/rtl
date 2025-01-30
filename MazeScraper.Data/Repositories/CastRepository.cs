namespace MazeScraper.Data.Services;

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
            // Assuming "Id" from the API is globally unique
            var filter = Builders<Cast>.Filter.Eq(c => c.Id, cast.Id);

            var replaceOne = new ReplaceOneModel<Cast>(filter, cast)
            {
                IsUpsert = true
            };
            writes.Add(replaceOne);
        }

        await _collection.BulkWriteAsync(writes);
    }
    
    public async Task<List<Cast>> GetByShowIdAsync(int showId, bool sortByBirthdayDesc = false)
    {
        var filter = Builders<Cast>.Filter.Eq(c => c.ShowId, showId);

        var query = _collection.Find(filter);

        if (sortByBirthdayDesc)
        {
            // Sort by cast.Birthday descending
            query = query.Sort(Builders<Cast>.Sort.Descending(c => c.Birthday));
        }

        return await query.ToListAsync();
    }
}