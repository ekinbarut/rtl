namespace MazeScraper.Domain.Models;

[BsonIgnoreExtraElements] 
public class TvShow
{
    [BsonElement("id")]
    public int Id { get; set; }

    [BsonElement("name")] public string Name { get; set; }

    [BsonElement("type")] public string Type { get; set; }

    [BsonElement("status")] public string Status { get; set; }

    [BsonElement("image")] public Image? Image { get; set; }

    [BsonElement("summary")] public string Summary { get; set; }

    [BsonElement("updated")] public int Updated { get; set; }
    
}

public class Image
{
    [BsonElement("medium")] public string Medium { get; set; }

    [BsonElement("original")] public string Original { get; set; }
}
