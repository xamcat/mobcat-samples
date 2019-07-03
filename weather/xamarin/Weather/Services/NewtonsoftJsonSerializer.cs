using Microsoft.MobCAT.Converters;
using Newtonsoft.Json;

namespace Weather.Services
{
	public class NewtonsoftJsonSerializer : ISerializer<string>
    {
        public string MediaType => "application/json";

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}