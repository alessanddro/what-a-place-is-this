using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace what_a_place_is_this.api.Models;

public class Place
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Coordinates { get; set; }
    public string Address { get; set; }
    public List<Picture> Pictures { get; set; } = new List<Picture>();
    public List<Comments> Comment { get; set; } = new List<Comments>();
    public int Evaluation { get; set; } = 0;
}
