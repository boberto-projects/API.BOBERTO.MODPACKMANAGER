using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinecraftServer.Api.MongoEntities
{
    public class ModPackModel : BaseModel
    {
        [BsonElement("name")]
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; } = null!;

        [BsonElement("author")]
        [BsonRepresentation(BsonType.String)]
        public string Author { get; set; } = null!;

        [BsonElement("dateTimeCreatAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateTimeCreatAt { get; set; }

        [BsonElement("dateTimeUpdatAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateTimeUpdatAt { get; set; }

        [BsonElement("isVerifyMods")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsVerifyMods { get; set; }

        [BsonElement("isDefault")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsDefault { get; set; }

        [BsonElement("isPremium")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsPremium { get; set; }

        [BsonElement("description")]
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; } = null!;

        [BsonElement("directory")]
        [BsonRepresentation(BsonType.String)]
        public string Directory { get; set; } = null!;

        [BsonElement("forgeVersion")]
        [BsonRepresentation(BsonType.String)]
        public string ForgeVersion { get; set; } = null!;

        [BsonElement("fabricVersion")]
        [BsonRepresentation(BsonType.String)]
        public string FabricVersion { get; set; } = null!;

        [BsonElement("gameVersion")]
        [BsonRepresentation(BsonType.String)]
        public string GameVersion { get; set; } = null!;

        [BsonElement("img")]
        [BsonRepresentation(BsonType.String)]
        public string Img { get; set; } = null!;

        [BsonElement("serverIp")]
        [BsonRepresentation(BsonType.String)]
        public string ServerIp { get; set; } = null!;

        [BsonElement("serverPort")]
        [BsonRepresentation(BsonType.String)]
        public int ServerPort { get; set; }
    }
}
