using Newtonsoft.Json;

namespace PushDemoApi.Models
{
    public class NotifcationRequest
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("silent")]
        public bool Silent { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}