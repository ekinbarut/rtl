namespace MazeScraper.Data;

public interface ITvShowService
{
    Task<IEnumerable<TvShowDto>> GetShowsWithCastAsync(int page, int pageSize);
}