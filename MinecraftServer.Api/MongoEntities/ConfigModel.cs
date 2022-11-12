using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinecraftServer.Api.MongoEntities
{
    public class ConfigModel : BaseModel
    {
        [BsonElement("maintenance")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Maintenance { get; set; }

        [BsonElement("maintenance_message")]
        [BsonRepresentation(BsonType.String)]
        public string MaintenanceMessage { get; set; }

        [BsonElement("offline")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Offline { get; set; }

        [BsonElement("client_id")]
        [BsonRepresentation(BsonType.String)]
        public string ClientId { get; set; }

    }
}
