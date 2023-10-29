using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace what_a_place_is_this.api.Models
{
    public class Comments
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Comment { get; set; }
        public string UserId { get; set; }
    }
}
