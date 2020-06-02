using Newtonsoft.Json;

namespace PushDemo.Models
{
    public class PushTemplate
    {
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}