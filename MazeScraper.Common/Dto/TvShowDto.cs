namespace MazeScraper.Common.Dto;

public class TvShowDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ActorDto> Cast { get; set; } = new();
}