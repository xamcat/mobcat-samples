using SampleNativeVideo.iOS.Model;

namespace SampleNativeVideo.iOS
{
    public interface IAssetListTableViewCellDelegate
    {
        void DownloadStateDidChange(AssetListTableViewCell cell, AssetDownloadState state);
    }
}