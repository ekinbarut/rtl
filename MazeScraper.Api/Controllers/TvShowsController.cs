using Microsoft.AspNetCore.Mvc;

namespace MazeScraper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TvShowsController : ControllerBase
{
    private readonly ITvShowService _tvShowService;

    public TvShowsController(ITvShowService tvShowService)
    {
        _tvShowService = tvShowService;
    }

    /// <summary>
    /// GET /api/tvshows?page=0&pageSize=10
    /// Returns a paginated list of shows, each with a cast array sorted by birthday desc.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetShowsWithCast(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _tvShowService.GetShowsWithCastAsync(page, pageSize);
        return Ok(response);
    }
}