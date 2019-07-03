using Newtonsoft.Json;

namespace Weather.Models
{
    public class CityWeatherImage
    {

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }
}