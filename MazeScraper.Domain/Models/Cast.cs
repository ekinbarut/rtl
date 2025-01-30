namespace MazeScraper.Domain.Models;

[BsonIgnoreExtraElements]
public class Cast
{
    [BsonElement("id")] public int Id { get; set; }

    [BsonElement("name")] public string Name { get; set; }

    [BsonElement("birthday")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? Birthday { get; set; }

    [BsonElement("gender")] public string Gender { get; set; }

    [BsonElement("image")] public Image? Image { get; set; }

    [BsonElement("updated")] public int Updated { get; set; }

    [BsonElement("showId")] public int ShowId { get; set; }
}