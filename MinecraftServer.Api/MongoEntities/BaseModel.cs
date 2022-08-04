using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MinecraftServer.Api.MongoEntities
{
    public abstract class BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        protected BaseModel()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is string id)
            {
                return this.Id.Equals(id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
