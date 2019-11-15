using System;
using System.Diagnostics;
using AVFoundation;
using Foundation;
using SampleNativeVideo.iOS.Model;

namespace SampleNativeVideo.iOS.Managers
{
    public class AssetPlaybackManager : NSObject
    {
        public static AssetPlaybackManager Current = new AssetPlaybackManager();
        public IAssetPlaybackDelegate Delegate { get; set; }

        private bool readyForPlayback = false;
        private IDisposable urlAssetObserver;
        private IDisposable playerObserver;
        private IDisposable playerItemObserver;
        private AVPlayerItem playerItem;
        private AVPlayer player = new AVPlayer();

        public AssetPlaybackManager()
        {
            playerObserver = player.AddObserver(new NSString(@"currentItem"), NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, PlayerCurrentItemDidChange);
            player.UsesExternalPlaybackWhileExternalScreenIsActive = true;
        }

        public Asset Asset { get; private set; }

        private AVPlayerItem PlayerItem
        {
            get
            {
                return playerItem;
            }

            set
            {
                // Will set
                if (playerItemObserver != null)
                {
                    playerItemObserver.Dispose();
                    playerItemObserver = null;
                }

                // Set
                playerItem = value;

                // Did set
                playerItemObserver = playerItem.AddObserver(new NSString("status"), NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, PlayerItemStatusChanged);
            }
        }

        /*
        Replaces the currently playing `Asset`, if any, with a new `Asset`. If nil
        is passed, `AssetPlaybackManager` will handle unloading the existing `Asset`
        and handle KVO cleanup
        */
        public void SetAssetForPlayback(Asset asset)
        {
            // Remove any existing observers here
            if (urlAssetObserver != null)
            {
                urlAssetObserver.Dispose();
                urlAssetObserver = null;
            }

            Asset = asset;

            if (asset == null)
            {
                playerItem = null;
                player?.ReplaceCurrentItemWithPlayerItem(null);
                readyForPlayback = false;
                return;
            }
                
            urlAssetObserver = asset.UrlAsset.AddObserver(new NSString("isPlayable"), NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, IsPlayableChanged);
        }

        private void PlayerCurrentItemDidChange(NSObservedChange obj)
        {
            Delegate?.PlayerCurrentItemDidChange(this, player);
        }

        private void IsPlayableChanged(NSObservedChange obj)
        {
            if (!Asset.UrlAsset.Playable)
                return;

            playerItem = new AVPlayerItem(Asset.UrlAsset);
            player.ReplaceCurrentItemWithPlayerItem(playerItem);
        }

        private void PlayerItemStatusChanged(NSObservedChange obj)
        {
            if (playerItem.Status == AVPlayerItemStatus.ReadyToPlay)
            {
                if (!readyForPlayback)
                {
                    readyForPlayback = true;
                    Delegate.PlayerReadyToPlay(this, player);
                }
                else if (playerItem.Status == AVPlayerItemStatus.Failed)
                {
                    var error = playerItem.Error;
                    Debug.WriteLine($"Error: {error?.LocalizedDescription}");
                }
            }
        }
    }
}