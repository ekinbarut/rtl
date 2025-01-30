namespace MazeScraper.Data;

public interface ICastRepository
{
    Task BulkUpsertAsync(List<Cast> castList);
}