// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SampleNativeVideo.iOS
{
    [Register ("AssetListTableViewCell")]
    partial class AssetListTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AssetNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView DownloadProgressView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DownloadStateLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AssetNameLabel != null) {
                AssetNameLabel.Dispose ();
                AssetNameLabel = null;
            }

            if (DownloadProgressView != null) {
                DownloadProgressView.Dispose ();
                DownloadProgressView = null;
            }

            if (DownloadStateLabel != null) {
                DownloadStateLabel.Dispose ();
                DownloadStateLabel = null;
            }
        }
    }
}