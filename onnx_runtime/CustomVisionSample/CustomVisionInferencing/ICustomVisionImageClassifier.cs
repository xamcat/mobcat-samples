using System.Threading.Tasks;

namespace CustomVisionInferencing
{
    public interface ICustomVisionImageClassifier
    {
        Task<CustomVisionClassificationModelOutput> RunAsync(byte[] image);
    }
}