using System.Collections.Generic;
using Newtonsoft.Json;

namespace PushDemo.Models
{
    public class DeviceInstallation
    {
        public DeviceInstallation()
        {
            Tags = new List<string>();
            Templates = new Dictionary<string, PushTemplate>();
        }

        [JsonProperty("installationId")]
        public string InstallationId { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("pushChannel")]
        public string PushChannel { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("templates")]
        public Dictionary<string, PushTemplate> Templates { get; set; }
    }
}