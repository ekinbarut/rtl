namespace MazeScraper.Domain.Dtos;

public class TvShowDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<CastDto> Cast { get; set; }
}