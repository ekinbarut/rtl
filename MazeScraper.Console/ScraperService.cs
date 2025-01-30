namespace MazeScraper.Console;

public class ScraperService
{
    private readonly ITvMazeClient _client;
    private readonly ITvShowRepository _showRepository;
    private readonly ICastRepository _castRepository;
    private readonly ILogger<ScraperService> _logger;

    // Rate-limit parameters
    private const int MaxParallelRequests = 5; // concurrency for cast calls
    private const int DelayBetweenPagesMs = 1000; // delay between pages to avoid 429
    private const int DelayBetweenRequestsMs = 500; // delay between cast requests
    private const int MaxShowCount = 1000; // times amount of cast to keep in memory before bulk operation

    public ScraperService(
        ITvMazeClient client,
        ITvShowRepository showRepository,
        ICastRepository castRepository,
        ILogger<ScraperService> logger)
    {
        _client = client;
        _showRepository = showRepository;
        _castRepository = castRepository;
        _logger = logger;
    }

    public async Task RunShowsAsync()
    {
        _logger.LogInformation("Starting incremental scraping for shows...");
        var stopwatch = Stopwatch.StartNew();

        int totalShows = 0;
        try
        {
            int page = 0;
            while (true)
            {
                var rawShows = await _client.FetchShowsPageAsync(page);
                if (rawShows == null || rawShows.Count == 0)
                {
                    _logger.LogInformation($"No shows found at page {page}, stopping.");
                    break;
                }

                _logger.LogInformation($"Fetched {rawShows.Count} shows on page {page}.");
                var tvShows = MapShows(rawShows);

                await _showRepository.BulkUpsertAsync(tvShows);

                totalShows += tvShows.Count;
                _logger.LogInformation($"Upserted {tvShows.Count} shows for page {page} Total Shows: {totalShows}.");

                page++;
                _logger.LogInformation($"Waiting {DelayBetweenPagesMs}ms before the next page...");
                await Task.Delay(DelayBetweenPagesMs);
            }
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                $"SHOW scraping finished in {stopwatch.Elapsed.TotalSeconds:F2} seconds. TotalShows: {totalShows}");
        }
    }

    /// <summary>
    /// Fetch Cast data in parallel and bulk insert
    /// </summary>
    public async Task RunCastAsync()
    {
        _logger.LogInformation("Starting incremental scraping for CAST...");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            int page = 0;
            while (true)
            {
                var rawShows = await _showRepository.GetAllAsync(page, MaxShowCount);
                var rawShowIds = rawShows.Select(x => x.Id).ToList();
                if (rawShows == null || rawShows.Count == 0)
                {
                    _logger.LogInformation($"No shows found at page {page}, stopping.");
                    break;
                }

                _logger.LogInformation($"Fetched {rawShows.Count} shows on page {page} for cast scraping.");
                await ProcessCastForShowsAsync(rawShowIds);

                page++;
            }
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation($"CAST scraping finished in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
        }
    }

    private async Task ProcessCastForShowsAsync(List<int> showIds)
    {
        var allCasts = new List<Cast>();
        var semaphore = new SemaphoreSlim(MaxParallelRequests);

        var tasks = showIds.Select(async showId =>
        {
            await semaphore.WaitAsync();
            try
            {
                var castJson = await _client.FetchShowCastAsync(showId);
                if (castJson != null && castJson.Count > 0)
                {
                    var mappedCast = MapCast(castJson, showId);

                    lock (allCasts)
                    {
                        allCasts.AddRange(mappedCast);
                    }
                }

                // Quick delay to avoid hitting 429
                await Task.Delay(DelayBetweenRequestsMs);
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(tasks);

        if (allCasts.Any())
        {
            await _castRepository.BulkUpsertAsync(allCasts);
            _logger.LogInformation($"Upserted {allCasts.Count} cast entries.");
        }
    }

    #region Mappers

    private List<TvShow> MapShows(List<dynamic> rawShows)
    {
        var tvShows = new List<TvShow>();
        foreach (var showJson in rawShows)
        {
            var tvShow = new TvShow
            {
                Id = (int)showJson.id,
                Name = (string)showJson.name,
                Type = (string)showJson.type,
                Status = (string)showJson.status,
                Summary = (string)showJson.summary,
                Updated = (int)showJson.updated,
                Image = showJson.image != null
                    ? new Image
                    {
                        Medium = (string)showJson.image.medium,
                        Original = (string)showJson.image.original
                    }
                    : null
            };
            tvShows.Add(tvShow);
        }

        return tvShows;
    }

    private List<Cast> MapCast(List<dynamic> castJson, int showId)
    {
        var castList = new List<Cast>();
        foreach (var castObj in castJson)
        {
            var person = castObj.person;
            if (person == null) continue;

            var mapped = new Cast
            {
                Id = (int)person.id,
                Name = (string)person.name,
                Birthday = ParseDate((string)person.birthday),
                Gender = (string)person.gender,
                Updated = (int)person.updated,
                ShowId = showId,
                Image = person.image != null
                    ? new Image
                    {
                        Medium = (string)person.image.medium,
                        Original = (string)person.image.original
                    }
                    : null
            };
            castList.Add(mapped);
        }

        return castList;
    }

    private DateTime? ParseDate(string date)
    {
        DateTime? parsedBirthday = null;
        if (!string.IsNullOrEmpty(date))
        {
            if (DateTime.TryParse(date, out var dt))
            {
                parsedBirthday = dt;
            }
        }

        return parsedBirthday;
    }

    #endregion
}