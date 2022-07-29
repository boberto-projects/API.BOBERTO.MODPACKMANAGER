using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinecraftServer.Api.MongoModels
{
    public abstract class BaseModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is string id)
            {
                return this.Id == ObjectId.Parse(id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        //protected BaseModel()
        //{
        //    Id = ObjectId.GenerateNewId();
        //}
    }
}
