namespace MazeScraper.Client;

public interface ITvMazeClient
{
    Task<List<dynamic>> FetchShowsPageAsync(int page);
    Task<List<dynamic>> FetchShowCastAsync(int showId);
}