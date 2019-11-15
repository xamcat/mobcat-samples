using AVFoundation;
using AVKit;
using Foundation;
using SampleNativeVideo.iOS.Managers;
using SampleNativeVideo.iOS.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace SampleNativeVideo.iOS
{
    public partial class AssetListTableViewController : UITableViewController, IAssetPlaybackDelegate, IAssetListTableViewCellDelegate
    {
        public const string PresentPlayerViewControllerSegueID = "PresentPlayerViewControllerSegueIdentifier";
        private AVPlayerViewController playerViewController;
        private Dictionary<string, Asset> pendingContentKeyRequests = new Dictionary<string, Asset>();

        public AssetListTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // General setup for auto sizing UITableViewCells.
            TableView.EstimatedRowHeight = new nfloat(75.0);
            TableView.RowHeight = UITableView.AutomaticDimension;

            // Set AssetListTableViewController as the delegate for AssetPlaybackManager to recieve playback information.
            AssetPlaybackManager.Current.Delegate = this;

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(AssetListManager.AssetListManagerDidLoad), HandleAssetLoadManagerDidLoad);
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(ContentKeyDelegate.DidSaveAllPersistableContentKey), HandleContentKeyDelegateDidSaveAllPersistableContentKey);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (playerViewController != null)
            {
                // The view reappeared as a results of dismissing an AVPlayerViewController.
                AssetPlaybackManager.Current.SetAssetForPlayback(null);
                playerViewController.Player = null;
                playerViewController = null;
            }
        }

        private void HandleContentKeyDelegateDidSaveAllPersistableContentKey(NSNotification notification)
        {
            var assetName = (notification.UserInfo["name"] as NSString)?.ToString();

            if (string.IsNullOrWhiteSpace(assetName))
                return; 

            Asset asset;
            pendingContentKeyRequests.TryGetValue(assetName, out asset);

            if (asset != null)
                pendingContentKeyRequests.Remove(assetName);

            AssetPersistenceManager.Current.DownloadAssetStream(asset);
        }

        private void HandleAssetLoadManagerDidLoad(NSNotification obj)
        {
            TableView.ReloadData();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return AssetListManager.Current.Assets.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = TableView.DequeueReusableCell(new NSString(AssetListTableViewCell.CellIdentifier));

            var asset = AssetListManager.Current.Assets[indexPath.Row];

            if (cell == null)
            {
                cell = new AssetListTableViewCell();
            }

            if (cell is AssetListTableViewCell)
            {
                AssetListTableViewCell assetListCell = (cell as AssetListTableViewCell);
                assetListCell.Asset = asset;
                assetListCell.WeakDelegate = this;
            }

            return cell;
        }

        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.CellAt(indexPath) as AssetListTableViewCell;
            var asset = cell?.Asset;

            if (cell == null || asset == null)
                return;

            var downloadState = AssetPersistenceManager.Current.GetDownloadState(asset);
            UIAlertAction alertAction;

            switch (downloadState)
            {
                case AssetDownloadState.Downloading:
                    alertAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (action) =>
                    {
                        AssetPersistenceManager.Current.CancelAssetDownload(asset);
                    });
                    break;
                case AssetDownloadState.Downloaded:
                    alertAction = UIAlertAction.Create("Delete", UIAlertActionStyle.Default, (action) =>
                    {
                        AssetPersistenceManager.Current.DeleteAsset(asset);

                        if (asset.Stream.IsProtected)
                        {
                            ContentKeyManager.Current.ContentKeyDelegate.DeleteAllPeristableContentKeys(asset);
                        }
                            
                    });
                    break;
                default: // NotDownloaded
                    alertAction = UIAlertAction.Create("Download", UIAlertActionStyle.Default, (action) => 
                    {
                        if (asset.Stream.IsProtected)
                        {
                            pendingContentKeyRequests[asset.Stream.Name] = asset;
                            ContentKeyManager.Current.ContentKeyDelegate.RequestPersistableContentKeys(asset);
                        }
                        else
                        {
                            AssetPersistenceManager.Current.DownloadAssetStream(asset);
                        }
                    });
                    break;
            }

            var alertController = UIAlertController.Create(asset.Stream.Name, "Select from the following options:", UIAlertControllerStyle.ActionSheet);

            alertController.AddAction(alertAction);
            alertController.AddAction(UIAlertAction.Create("Dismiss", UIAlertActionStyle.Cancel, null));

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                var popoverController = alertController.PopoverPresentationController;

                if (popoverController == null)
                    return;

                popoverController.SourceView = cell;
                popoverController.SourceRect = cell.Bounds;
            }

            PresentViewController(alertController, true, null);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == AssetListTableViewController.PresentPlayerViewControllerSegueID ||
               sender is AssetListTableViewCell)
            {
                var cell = sender as AssetListTableViewCell;
                var destinationViewController = segue.DestinationViewController as AVPlayerViewController;
                var asset = cell.Asset;

                if (destinationViewController == null || asset == null)
                    return;

                playerViewController = destinationViewController;

                if (AssetPersistenceManager.Current.GetDownloadState(asset) == AssetDownloadState.Downloaded)
                {
                    if (!asset.UrlAsset.ResourceLoader.PreloadsEligibleContentKeys)
                        asset.UrlAsset.ResourceLoader.PreloadsEligibleContentKeys = true;
                }

                AssetPlaybackManager.Current.SetAssetForPlayback(asset);
            }
        }

        public void PlayerCurrentItemDidChange(AssetPlaybackManager assetPlaybackManager, AVPlayer player)
        {
            if (playerViewController == null || player?.CurrentItem == null)
                return;

            playerViewController.Player = player;
        }

        public void PlayerReadyToPlay(AssetPlaybackManager assetPlaybackManager, AVPlayer player)
        {
            player?.Play();
        }

        public void DownloadStateDidChange(AssetListTableViewCell cell, AssetDownloadState state)
        {
            var indexPath = TableView.IndexPathForCell(cell);

            if (indexPath == null)
                return;

            TableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
        }
    }
}