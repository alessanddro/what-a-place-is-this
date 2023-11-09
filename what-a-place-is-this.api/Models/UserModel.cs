using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace what_a_place_is_this.api.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
