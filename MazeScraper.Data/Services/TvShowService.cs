namespace MazeScraper.Data.Services;

public class TvShowService : ITvShowService
{
    private readonly ITvShowRepository _showRepository;
    private readonly ICastRepository _castRepository;

    public TvShowService(ITvShowRepository showRepository, ICastRepository castRepository)
    {
        _showRepository = showRepository;
        _castRepository = castRepository;
    }

    public async Task<IEnumerable<TvShowDto>> GetShowsWithCastAsync(int page, int pageSize)
    {
        var shows = await _showRepository.GetAllAsync(page, pageSize);
        if (shows == null || shows.Count == 0)
            new List<object>(); 

        var tasks = shows.Select(async show =>
        {
            var cast = await _castRepository.GetByShowIdAsync(
                show.Id,
                sortByBirthdayDesc: true);

            return new TvShowDto
            {
                Id = show.Id,
                Name = show.Name,
                Type = show.Type,
                Cast = cast.Select(c => new CastDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Birthday = c.Birthday?.ToString("yyyy-MM-dd"),
                    })
                    .ToList()
            };
        });
        
        return await Task.WhenAll(tasks);
    }
}