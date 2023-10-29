using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace what_a_place_is_this.api.Models
{
    public class Picture
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public bool Active { get; set; } = false;
        public bool Validated { get; set; } = false;
        public string PostedBy { get; set; }
        public string Path { get; set; }
    }
}
