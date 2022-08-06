using MinecraftServer.Api.MongoEntities;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.ResponseModels
{
    public class ConfigResponse
    {
        [JsonPropertyName("maintenance")]
        public bool Maintenance { get; set; }

        [JsonPropertyName("maintenance_message")]
        public string MaintenanceMessage { get; set; }

        [JsonPropertyName("offline")]
        public bool Offline { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("java")]
        public bool Java { get; set; }

        [JsonPropertyName("ignored")]
        public List<string> Ignored { get; set; }

        public ConfigModel ToMap()
        {
            return new ConfigModel
            {
              ClientId = this.ClientId,
              Maintenance = this.Maintenance,
              MaintenanceMessage = this.MaintenanceMessage,
              Offline = this.Offline,
              Java = this.Java,
              Ignored = this.Ignored,
            };
        }
    }
}
