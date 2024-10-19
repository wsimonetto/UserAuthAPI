using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UserAuthAPI.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; } // Agora é anulável

        [BsonElement("passwordHash")]
        public string? PasswordHash { get; set; } // Agora é anulável

        [BsonElement("recoveryEmail")]
        public string? RecoveryEmail { get; set; } // Agora é anulável
    }
}
