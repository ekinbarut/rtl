namespace MazeScraper.Client;

public class TvMazeClient : ITvMazeClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TvMazeClient> _logger;

    public TvMazeClient(HttpClient httpClient, ILogger<TvMazeClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Fetch paginated TV shows from API.
    /// </summary>
    public async Task<List<dynamic>> FetchShowsPageAsync(int page)
    {
        var url = $"https://api.tvmaze.com/shows?page={page}";
        _logger.LogInformation($"Fetching TV shows - Page {page}");

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<dynamic>>(content);
    }

    /// <summary>
    /// Fetch cast for a specific show.
    /// </summary>
    public async Task<List<dynamic>> FetchShowCastAsync(int showId)
    {
        var url = $"https://api.tvmaze.com/shows/{showId}/cast";
        _logger.LogInformation($"Fetching cast for Show ID {showId}");

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return new List<dynamic>();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<dynamic>>(content);
    }
}