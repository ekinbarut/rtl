namespace MazeScraper.Data;

public interface ITvShowRepository
{
    Task<List<TvShow>> GetAllAsync(int page, int pageSize);
    Task BulkUpsertAsync(List<TvShow> tvShows);
}