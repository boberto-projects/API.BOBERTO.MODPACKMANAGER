using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinecraftServer.Api.MongoEntities
{
    public class ConfigModel : BaseModel
    {
        [BsonElement("maintenance")]
        [BsonRepresentation(BsonType.String)]
        public bool Maintenance { get; set; }

        [BsonElement("maintenance_message")]
        [BsonRepresentation(BsonType.String)]
        public string MaintenanceMessage { get; set; }

        [BsonElement("offline")]
        [BsonRepresentation(BsonType.String)]
        public bool Offline { get; set; }

        [BsonElement("client_id")]
        [BsonRepresentation(BsonType.String)]
        public string ClientId { get; set; }

        [BsonElement("java")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Java { get; set; }

        [BsonElement("ignored")]
        public List<string> Ignored { get; set; }
    }
}
