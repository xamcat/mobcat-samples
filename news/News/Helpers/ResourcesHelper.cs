using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace News.Helpers
{
    public static class ResourcesHelper
    {
        public async static Task<string> ReadResourceContent(this string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return null;

            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream == null)
                return null;

            using (var reader = new StreamReader(resourceStream))
            {
                var result = await reader.ReadToEndAsync();
                return result;
            }
        }
    }
}
