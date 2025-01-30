namespace MazeScraper.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var scraperService = serviceProvider.GetRequiredService<ScraperService>();

        await scraperService.RunShowsAsync();

        await scraperService.RunCastAsync();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddLogging(config => config.AddConsole());

        services.AddHttpClient<TvMazeClient>();

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<ITvShowRepository, TvShowRepository>();
        services.AddScoped<ICastRepository, CastRepository>();
        services.AddScoped<ScraperService>();
    }
}