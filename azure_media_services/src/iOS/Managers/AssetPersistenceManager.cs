using System;
using Foundation;
using AVFoundation;
using System.Collections.Generic;
using SampleNativeVideo.iOS.Model;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using CoreMedia;

namespace SampleNativeVideo.iOS.Managers
{
    public class AssetPersistenceManager : NSObject, IAVAssetDownloadDelegate
    {
        public const string AssetDownloadProgress = "AssetDownloadProgressNotification";
        public const string AssetDownloadStateChanged = "AssetDownloadStateChangedNotification";
        public const string AssetPersistenceManagerDidRestoreState = "AssetPersistenceManagerDidRestoreStateNotification";

        public static AssetPersistenceManager Current = new AssetPersistenceManager();

        bool didRestorePersistenceManager;
        AVAssetDownloadUrlSession assetDownloadUrlSession;
        Dictionary<AVAggregateAssetDownloadTask, Asset> activeDownloadsMap;
        Dictionary<AVAggregateAssetDownloadTask, NSUrl> willDownloadToUrlMap;

        public AssetPersistenceManager()
        {
            var backgroundConfiguration = NSUrlSessionConfiguration.BackgroundSessionConfiguration("AAPL-Identifier");
            assetDownloadUrlSession = AVAssetDownloadUrlSession.CreateSession(backgroundConfiguration, this, NSOperationQueue.MainQueue);
            activeDownloadsMap = new Dictionary<AVAggregateAssetDownloadTask, Asset>();
            willDownloadToUrlMap = new Dictionary<AVAggregateAssetDownloadTask, NSUrl>();
        }

        public Asset AssetForStream(string name)
        {
            Asset asset = null;

            var results = activeDownloadsMap.Where(i => i.Value.Stream.Name == asset.Stream.Name);

            if (results.Count() == 1)
                asset = results.FirstOrDefault().Value;

            return asset;
        }

        public Asset LocalAssetForStream(string name)
        {
            Asset asset = null;

            var userDefaults = NSUserDefaults.StandardUserDefaults;
            var localFileLocation = userDefaults.ValueForKey(new NSString(name)) as NSData;

            if (localFileLocation == null)
            {
                return null;
            }

            var bookmarkDataIsStale = false;
            NSError error;
            NSUrl url;

            try
            {
                url = new NSUrl(localFileLocation, default(NSUrlBookmarkResolutionOptions), null, out bookmarkDataIsStale, out error);

                if (url == null || bookmarkDataIsStale)
                {
                    throw new Exception("Bookmark data is stale!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create URL from bookmark!", ex);
            }

            var urlAsset = new AVUrlAsset(url);
            var stream = StreamListManager.Current.Streams.Where(i => i.Name == name).FirstOrDefault();
            asset = new Asset(stream, urlAsset);

            return asset;
        }

        public AssetDownloadState GetDownloadState(Asset asset)
        {
            var localFileLocation = LocalAssetForStream(asset.Stream.Name)?.UrlAsset.Url;

            if (localFileLocation != null && NSFileManager.DefaultManager.FileExists(localFileLocation.Path))
            {
                return AssetDownloadState.Downloaded;
            }

            var activeDownload = activeDownloadsMap.Where(i => i.Value.Stream.Name == asset.Stream.Name).Count();

            return activeDownload > 0 ? AssetDownloadState.Downloading : AssetDownloadState.NotDownloaded;
        }

        public async Task RestorePersistenceManager()
        {
            if (didRestorePersistenceManager)
                return;

            didRestorePersistenceManager = true;

            // Grab all the tasks associated with the assetDownloadURLSession
            TaskCompletionSource<NSUrlSessionTask[]> tcs = new TaskCompletionSource<NSUrlSessionTask[]>();
            assetDownloadUrlSession.GetAllTasks((tasks) => tcs.TrySetResult(tasks));

            await tcs.Task;

            if (!tcs.Task.IsCompleted)
                throw new Exception("Error restoring persistence manager state", tcs.Task?.Exception);

            foreach (var task in tcs.Task?.Result)
            {
                if (!(task is AVAggregateAssetDownloadTask))
                    break;
                    
                AVAggregateAssetDownloadTask assetDownloadTask = task as AVAggregateAssetDownloadTask;
                var assetName = assetDownloadTask.TaskDescription;
                var urlAsset = assetDownloadTask.UrlAsset;
                var stream = StreamListManager.Current.Streams.FirstOrDefault(i => i.Name == assetName);
                var asset = new Asset(stream, urlAsset);

                activeDownloadsMap[assetDownloadTask] = asset;
            }

            NSNotificationCenter.DefaultCenter.PostNotificationName(AssetPersistenceManager.AssetPersistenceManagerDidRestoreState, null);
        }

        internal void DeleteAsset(Asset asset)
        {
            var userDefaults = NSUserDefaults.StandardUserDefaults;

            var localFileLocation = LocalAssetForStream(asset.Stream.Name)?.UrlAsset.Url;

            if (localFileLocation == null)
                return;
                
            try
            {
                NSError error;
                NSFileManager.DefaultManager.Remove(localFileLocation, out error);

                if (error != null)
                    throw new Exception($"An error occured deleting the file {error.ToString()}");

                userDefaults.RemoveObject(asset.Stream.Name);

                var userInfo = new Dictionary<string, object>();
                userInfo[Asset.Keys.Name] = asset.Stream.Name;
                userInfo[Asset.Keys.DownloadState] = AssetDownloadState.NotDownloaded.ToString();

                var userInfoDictionary = NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray());

                NSNotificationCenter.DefaultCenter.PostNotificationName(AssetPersistenceManager.AssetDownloadStateChanged, null, userInfoDictionary);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured deleting the file", ex);
            }
        }

        internal void CancelAssetDownload(Asset asset)
        {
            AVAggregateAssetDownloadTask task = activeDownloadsMap.Where(i => i.Value == asset).Select(i => i.Key).FirstOrDefault();
            task?.Cancel();
        }

        internal void DownloadAssetStream(Asset asset)
        {
            // Get the default media selections for the asset's media selection groups.
            var preferredMediaSelection = asset.UrlAsset.PreferredMediaSelection;

            ///*
            // Creates and initializes an AVAggregateAssetDownloadTask to download multiple AVMediaSelections
            // on an AVURLAsset.

            // For the initial download, we ask the URLSession for an AVAssetDownloadTask with a minimum bitrate
            // corresponding with one of the lower bitrate variants in the asset.
            // */

            AVAggregateAssetDownloadTask task = null;

            try
            {
                var dictionary = new NSDictionary<NSString, NSObject>(new NSString("AVAssetDownloadTaskMinimumRequiredMediaBitrateKey"), (NSObject)(new NSNumber(265_000)));

                task = assetDownloadUrlSession.GetAssetDownloadTask(asset.UrlAsset, 
                                                                          new AVMediaSelection[] { preferredMediaSelection }, 
                                                                          asset.Stream.Name, 
                                                                          null,
                                                                          dictionary);
                task.TaskDescription = asset.Stream.Name;

                activeDownloadsMap.Add(task, asset);
                task.Resume();

                var userInfo = new Dictionary<string, object>();
                userInfo[Asset.Keys.Name] = asset.Stream.Name;
                userInfo[Asset.Keys.DownloadState] = AssetDownloadState.Downloading.ToString();
                userInfo[Asset.Keys.DownloadSelectionDisplayName] = GetDisplayNamesForSelectedMediaOptions(preferredMediaSelection);

                var userInfoDictionary = NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray());

                NSNotificationCenter.DefaultCenter.PostNotificationName(AssetPersistenceManager.AssetDownloadStateChanged, null, userInfoDictionary);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Return the display names for the media selection options that are currently selected in the specified group
        private string GetDisplayNamesForSelectedMediaOptions(AVMediaSelection mediaSelection)
        {
            string displayNames = string.Empty;
            var asset = mediaSelection.Asset;

            if (asset == null)
                return displayNames;

            foreach(var mediaCharacteristic in asset.AvailableMediaCharacteristicsWithMediaSelectionOptions)
            {
                /*
                 Obtain the AVMediaSelectionGroup object that contains one or more options with the
                 specified media characteristic, then get the media selection option that's currently
                 selected in the specified group.
                 */
                var mediaSelectionGroup = asset.MediaSelectionGroupForMediaCharacteristic(mediaCharacteristic);
                var option = mediaSelection.GetSelectedMediaOption(mediaSelectionGroup);

                // Obtain the display string for the media selection option
                displayNames += string.IsNullOrWhiteSpace(displayNames) ? " " + option.DisplayName : ", " + option.DisplayName;
            }

            return displayNames;
        }

        [Export("URLSession:aggregateAssetDownloadTask:didCompleteForMediaSelection:")]
        public void DidCompleteForMediaSelection(NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, AVMediaSelection mediaSelection)
        {
            /*
             This delegate callback provides an AVMediaSelection object which is now fully available for
             offline use. You can perform any additional processing with the object here.
             */

            Asset asset = null;
            activeDownloadsMap.TryGetValue(aggregateAssetDownloadTask, out asset);

            if (asset == null || Asset.Keys.Name != asset.Stream.Name)
                return;

            aggregateAssetDownloadTask.TaskDescription = asset.Stream.Name;
            aggregateAssetDownloadTask.Resume();

            var userInfo = new Dictionary<string, string>();
            userInfo[Asset.Keys.DownloadState] = new NSString(Asset.Keys.DownloadState.ToString());
            userInfo[Asset.Keys.DownloadSelectionDisplayName] = new NSString(GetDisplayNamesForSelectedMediaOptions(mediaSelection));

            var userInfoDictionary = NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray());

            NSNotificationCenter.DefaultCenter.PostNotificationName(AssetPersistenceManager.AssetDownloadStateChanged, null, userInfoDictionary);
        }

        [Export("URLSession:aggregateAssetDownloadTask:willDownloadToURL:")]
        public void WillDownloadToUrl(NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, NSUrl location)
        {
            /*
             This delegate callback should only be used to save the location URL
             somewhere in your application. Any additional work should be done in
             `URLSessionTaskDelegate.urlSession(_:task:didCompleteWithError:)`.
             */

            willDownloadToUrlMap[aggregateAssetDownloadTask] = location;
        }

        [Export("URLSession:task:didCompleteWithError:")]
        public void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            var userDefaults = NSUserDefaults.StandardUserDefaults;

            Asset asset = null;
            NSUrl downloadURL = null;
            AVAggregateAssetDownloadTask avTask = task as AVAggregateAssetDownloadTask;

            if (avTask == null ||
                !activeDownloadsMap.TryGetValue(avTask, out asset) ||
                !willDownloadToUrlMap.TryGetValue(avTask, out downloadURL))
                return;

            var userInfo = new Dictionary<string, string>();
            userInfo[Asset.Keys.Name] = new NSString(asset.Stream.Name);

            if (error != null)
            {
                switch (error.Code)
                {
                    case (int)NSUrlError.Cancelled:

                        /*
                        This task was canceled, you should perform cleanup using the
                        URL saved from AVAssetDownloadDelegate.urlSession(_:assetDownloadTask:didFinishDownloadingTo:).
                        */
                        var localFileLocation = LocalAssetForStream(asset.Stream.Name)?.UrlAsset.Url;

                        if (localFileLocation == null)
                            return;

                        try
                        {
                            NSError err = null;
                            NSFileManager.DefaultManager.Remove(localFileLocation, out err);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"An error occured trying to delete the contents on disk for {asset.Stream.Name}: {ex.Message}");
                        }

                        userInfo[Asset.Keys.DownloadState] = new NSString(AssetDownloadState.NotDownloaded.ToString());

                        break;
                    case (int)NSUrlError.Unknown:
                        throw new Exception("Downloading HLS streams is not supported in the simulator.");
                    default:
                        throw new Exception($"An unexpected error occured {error.Domain}");
                }
            }
            else
            {
                try
                {
                    NSError err;

                    var bookmark = downloadURL.CreateBookmarkData(default(NSUrlBookmarkCreationOptions), new string[] { asset.Stream.Name }, downloadURL.AbsoluteUrl, out err);
                    userDefaults.SetValueForKey(bookmark, new NSString(asset.Stream.Name));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to create bookmarkData for download URL: {ex.Message}");
                }

                userInfo[Asset.Keys.DownloadState] = AssetDownloadState.Downloaded.ToString();
                userInfo[Asset.Keys.DownloadSelectionDisplayName] = string.Empty;
            }

            var userInfoDictionary = NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray());

            NSNotificationCenter.DefaultCenter.PostNotificationName(AssetPersistenceManager.AssetDownloadStateChanged, null, userInfoDictionary);
        }

        [Export("URLSession:aggregateAssetDownloadTask:didLoadTimeRange:totalTimeRangesLoaded:timeRangeExpectedToLoad:forMediaSelection:")]
        public void DidLoadTimeRange(NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, CoreMedia.CMTimeRange timeRange, NSValue[] loadedTimeRanges, CoreMedia.CMTimeRange timeRangeExpectedToLoad, AVMediaSelection mediaSelection)
        {
            // This delegate callback should be used to provide download progress for your AVAssetDownloadTask.
            if (!activeDownloadsMap.ContainsKey(aggregateAssetDownloadTask))
                return;

            var asset = activeDownloadsMap[aggregateAssetDownloadTask];

            var percentComplete = 0.0;

            foreach (var value in loadedTimeRanges)
            {
                var loadedTimeRange = value.CMTimeRangeValue;
                percentComplete += loadedTimeRange.Duration.Seconds / timeRangeExpectedToLoad.Duration.Seconds;
            }

            var userInfo = new Dictionary<string, object>();
            userInfo[Asset.Keys.Name] = new NSString(asset.Stream.Name);
            userInfo[Asset.Keys.PercentDownloaded] = new NSNumber(percentComplete);

            var userInfoDictionary = NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray());

            NSNotificationCenter.DefaultCenter.PostNotificationName(AssetPersistenceManager.AssetDownloadProgress, null, userInfoDictionary);
        }
    }
}