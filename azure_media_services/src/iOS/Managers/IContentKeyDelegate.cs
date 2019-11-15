using AVFoundation;
using SampleNativeVideo.iOS.Model;

namespace SampleNativeVideo.iOS.Managers
{
    public interface IContentKeyDelegate : IAVContentKeySessionDelegate
    {
        void RequestPersistableContentKeys(Asset asset);
        void DeleteAllPeristableContentKeys(Asset asset);
    }
}