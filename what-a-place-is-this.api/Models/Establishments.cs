using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace what_a_place_is_this.api.Models
{
    public class Establishments : PlaceModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}