﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.MongoEntities
{
    public class LauncherVersionModel : BaseModel
    {
        public class Linux64Entity
        {
            [BsonElement("url")]
            [BsonRepresentation(BsonType.String)]
            public string Url { get; set; }
        }

        public class Mac64Entity
        {
            [BsonElement("url")]
            [BsonRepresentation(BsonType.String)]
            public string Url { get; set; }
        }

        public class Win64Entity
        {
            [BsonElement("url")]
            [BsonRepresentation(BsonType.String)]
            public string Url { get; set; }
        }

        public class PackagesEntity
        {
            [BsonElement("win64")]
            public Win64Entity Win64 { get; set; }

            [BsonElement("mac64")]
            public Mac64Entity Mac64 { get; set; }

            [BsonElement("linux64")]
            public Linux64Entity Linux64 { get; set; }
        }

        [BsonElement("version")]
        [BsonRepresentation(BsonType.String)]
        public string Version { get; set; }

        [BsonElement("packages")]
        public PackagesEntity Packages { get; set; }

    }
}