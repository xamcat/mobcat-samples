using System.Threading.Tasks;

namespace CustomVisionInferencing
{
    public interface ICustomVisionImageProcessor
    {
        Task<CustomVisionModelInput> CreateResizeCenterCroppedRGBAImageAsync(byte[] image, int width, int height);
    }
}