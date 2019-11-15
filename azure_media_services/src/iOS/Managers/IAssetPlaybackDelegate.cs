using AVFoundation;

namespace SampleNativeVideo.iOS.Managers
{
    public interface IAssetPlaybackDelegate
    {
        // This is called when the internal AVPlayer in AssetPlaybackManager is ready to start playback.
        void PlayerReadyToPlay(AssetPlaybackManager assetPlaybackManager, AVPlayer player);

        // This is called when the internal AVPlayer's currentItem has changed.
        void PlayerCurrentItemDidChange(AssetPlaybackManager assetPlaybackManager, AVPlayer player);
    }
}