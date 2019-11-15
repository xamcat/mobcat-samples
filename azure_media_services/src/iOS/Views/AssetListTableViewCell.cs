using System;
using CoreFoundation;
using Foundation;
using SampleNativeVideo.iOS.Managers;
using SampleNativeVideo.iOS.Model;
using UIKit;

namespace SampleNativeVideo.iOS
{
    public partial class AssetListTableViewCell : UITableViewCell
    {
        private Asset asset;
        public const string CellIdentifier = "AssetListTableViewCellIdentifier";

        private WeakReference<IAssetListTableViewCellDelegate> weakDelegate;

        public IAssetListTableViewCellDelegate WeakDelegate
        {
            get
            {
                if (weakDelegate != null && weakDelegate.TryGetTarget(out var target))
                    return target;

                return null;
            }

            set
            {
                if (value == null)
                {
                    weakDelegate = null;
                    return;
                }

                weakDelegate = new WeakReference<IAssetListTableViewCellDelegate>(value);
            }
        }

        public Asset Asset
        {
            get
            {
                return asset;
            }

            set
            {
                asset = value;
                UpdateCell();
            }
        }

        public AssetListTableViewCell () : base(NSObjectFlag.Empty)
        {
        }

        public AssetListTableViewCell (IntPtr handle) : base (handle)
        {

        }

        public const string AssetListManagerDidLoad = "AssetListManagerDidLoadNotification";

        internal void UpdateCell()
        {
            if (asset == null)
            {
                AssetNameLabel.Text = null;
                DownloadStateLabel.Text = null;
            }
            else
            {
                var downloadState = AssetPersistenceManager.Current.GetDownloadState(asset);
                AssetNameLabel.Text = asset.Stream.Name;
                DownloadStateLabel.Text = downloadState.ToString();

                switch (downloadState)
                {
                    case AssetDownloadState.Downloaded:
                        DownloadProgressView.Hidden = true;
                        break;
                    case AssetDownloadState.Downloading:
                        DownloadProgressView.Hidden = false;
                        break;
                    default:
                        break;
                }

                var notificationCenter = NSNotificationCenter.DefaultCenter;
                notificationCenter.AddObserver(new NSString(AssetPersistenceManager.AssetDownloadStateChanged), HandleAssetDownloadStateChanged);
                notificationCenter.AddObserver(new NSString(AssetPersistenceManager.AssetDownloadProgress), HandleAssetDownloadProgress);
            }
        }

        private void HandleAssetDownloadProgress(NSNotification notification)
        {
            NSObject assetStreamName = null;
            NSObject progress = null;

            notification.UserInfo?.TryGetValue(new NSString(Asset.Keys.Name), out assetStreamName);

            if (assetStreamName == null || assetStreamName?.ToString() != asset.Stream.Name)
                return;

            notification.UserInfo?.TryGetValue(new NSString(Asset.Keys.PercentDownloaded), out progress);

            NSNumber number = progress as NSNumber;

            if (number == null)
                throw new Exception("Unable to parse progress value.");

            DownloadProgressView.SetProgress(number.FloatValue, true);
        }

        private void HandleAssetDownloadStateChanged(NSNotification notification)
        {
            NSObject assetStreamName;
            NSObject downloadStateRawValue;
            AssetDownloadState downloadState;
            NSObject downloadSelection;

            notification.UserInfo.TryGetValue(new NSString(Asset.Keys.Name), out assetStreamName);
            notification.UserInfo.TryGetValue(new NSString(Asset.Keys.DownloadState), out downloadStateRawValue);
            notification.UserInfo.TryGetValue(new NSString(Asset.Keys.DownloadSelectionDisplayName), out downloadSelection);

            if (assetStreamName == null || 
                downloadStateRawValue == null ||
                downloadSelection == null ||
                asset?.Stream.Name != assetStreamName.ToString())
                return;

            if (!Enum.TryParse(downloadStateRawValue.ToString(), out downloadState))
                throw new Exception("Unable to determine download state.");

            DispatchQueue.MainQueue.DispatchAsync(() => 
            {
                switch(downloadState)
                {
                    case AssetDownloadState.NotDownloaded:
                        DownloadProgressView.Hidden = true;
                        break;
                    default:
                        DownloadProgressView.Hidden = false;
                        DownloadStateLabel.Text = $"{downloadState}: {downloadSelection}";
                        break;
                }

                WeakDelegate?.DownloadStateDidChange(this, downloadState);
            });
        }
    }
}