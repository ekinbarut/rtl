namespace MazeScraper.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var settings = configuration.GetSection("MongoDb").Get<MongoDbSettings>();
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<TvShow> TvShows => _database.GetCollection<TvShow>("tv_shows");
    public IMongoCollection<Cast> Cast => _database.GetCollection<Cast>("cast");

}