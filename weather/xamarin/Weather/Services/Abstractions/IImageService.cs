using System.Threading;
using System.Threading.Tasks;

namespace Weather.Services.Abstractions
{
    public interface IImageService
    {
        Task<string> GetImageAsync(string city, string weather, CancellationToken cancellationToken = default(CancellationToken));
    }
}