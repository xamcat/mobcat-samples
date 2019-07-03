using System;
using Newtonsoft.Json;

namespace Weather.Models
{
    public class Forecast
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("currentTemperature")]
        public string CurrentTemperature { get; set; }

        [JsonProperty("minTemperature")]
        public string MinTemperature { get; set; }

        [JsonProperty("maxTemperature")]
        public string MaxTemperature { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }
    }
}
