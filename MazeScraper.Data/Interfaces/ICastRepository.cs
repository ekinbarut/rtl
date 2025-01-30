namespace MazeScraper.Data;

public interface ICastRepository
{
    Task BulkUpsertAsync(List<Cast> castList);
    Task<List<Cast>> GetByShowIdAsync(int showId, bool sortByBirthdayDesc = false);
}