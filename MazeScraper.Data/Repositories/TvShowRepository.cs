namespace MazeScraper.Data.Services;

public class TvShowRepository : ITvShowRepository
{
    private readonly IMongoCollection<TvShow> _collection;

    public TvShowRepository(MongoDbContext dbContext)
    {
        _collection = dbContext.TvShows;
    }

    public async Task<List<TvShow>> GetAllAsync(int page, int pageSize)
    {
        return await _collection.Find(Builders<TvShow>.Filter.Empty)
            .Skip(page * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task BulkUpsertAsync(List<TvShow> tvShows)
    {
        if (tvShows == null || tvShows.Count == 0) return;

        var writes = new List<WriteModel<TvShow>>();
        foreach (var show in tvShows)
        {
            var filter = Builders<TvShow>.Filter.Eq(s => s.Id, show.Id);
            var replaceOne = new ReplaceOneModel<TvShow>(filter, show)
            {
                IsUpsert = true
            };
            writes.Add(replaceOne);
        }

        await _collection.BulkWriteAsync(writes);
    }
}