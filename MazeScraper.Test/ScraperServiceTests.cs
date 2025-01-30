namespace MazeScraper.Test;

public class ScraperServiceTests
{
    private readonly Mock<ITvMazeClient> _clientMock;
    private readonly Mock<ITvShowRepository> _showRepoMock;
    private readonly Mock<ICastRepository> _castRepoMock;
    private readonly Mock<ILogger<ScraperService>> _loggerMock;
    private readonly ScraperService _scraperService;

    public ScraperServiceTests()
    {
        _clientMock = new Mock<ITvMazeClient>();
        _showRepoMock = new Mock<ITvShowRepository>();
        _castRepoMock = new Mock<ICastRepository>();
        _loggerMock = new Mock<ILogger<ScraperService>>();

        _scraperService = new ScraperService(
            _clientMock.Object,
            _showRepoMock.Object,
            _castRepoMock.Object,
            _loggerMock.Object
        );
    }


    [Fact]
    public async Task RunShowsAsync_NoShowsOnFirstPage_StopsImmediately()
    {
        // Arrange
        // First page is empty => no results
        _clientMock
            .Setup(c => c.FetchShowsPageAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<dynamic>()); // empty

        // Act
        await _scraperService.RunShowsAsync();

        // Assert
        // If no shows, we never call BulkUpsert
        _showRepoMock.Verify(r => r.BulkUpsertAsync(It.IsAny<List<TvShow>>()), Times.Never);
    }

    [Fact]
    public async Task RunCastAsync_NoShowsInRepository_StopsImmediately()
    {
        // Arrange
        _showRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<TvShow>()); // empty

        // Act
        await _scraperService.RunCastAsync();

        // Assert
        // No cast calls
        _clientMock.Verify(c => c.FetchShowCastAsync(It.IsAny<int>()), Times.Never);
        _castRepoMock.Verify(c => c.BulkUpsertAsync(It.IsAny<List<Cast>>()), Times.Never);
    }
}