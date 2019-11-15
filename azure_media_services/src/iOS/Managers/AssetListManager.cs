using System;
using System.Collections.Generic;
using AVFoundation;
using Foundation;
using SampleNativeVideo.iOS.Model;

namespace SampleNativeVideo.iOS.Managers
{
    public class AssetListManager : NSObject
    {
        public const string AssetListManagerDidLoad = "AssetListManagerDidLoadNotification";
        public static AssetListManager Current = new AssetListManager();
        public List<Asset> Assets { get; }

        public AssetListManager()
        {
            Assets = new List<Asset>();

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(AssetPersistenceManager.AssetPersistenceManagerDidRestoreState), HandleAssetPersistenceManagerDidRestoreState);
        }

        private void HandleAssetPersistenceManagerDidRestoreState(NSNotification obj)
        {
            foreach (var stream in StreamListManager.Current.Streams)
            {
                var asset = AssetPersistenceManager.Current.AssetForStream(stream.Name);

                if (asset != null)
                {
                    Assets.Add(asset);
                }
                else
                {
                    /*
                     If an existing `AVURLAsset` is not available for an active
                     download we then see if there is a file URL available to
                     create an asset from.
                     */

                    asset = AssetPersistenceManager.Current.LocalAssetForStream(stream.Name);

                    if (asset != null)
                    {
                        Assets.Add(asset);
                    }
                    else
                    {
                        var urlAsset = new AVUrlAsset(new NSUrl(stream.PlaylistUrl));
                        asset = new Asset(stream, urlAsset);
                        Assets.Add(asset);
                    }
                }
            }

            NSNotificationCenter.DefaultCenter.PostNotificationName(AssetListManager.AssetListManagerDidLoad, this);
        }
    }
}