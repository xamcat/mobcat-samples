using System;
using AVFoundation;
using SampleNativeVideo.iOS.Managers;

namespace SampleNativeVideo.iOS.Model
{
    public class Asset
    {
        public Stream Stream { get; set; }
        public AVUrlAsset UrlAsset { get; set; }
        public static AssetKeys Keys = new AssetKeys();

        public Asset()
        {
        }

        public Asset(Stream stream, AVUrlAsset urlAsset)
        {
            this.Stream = stream;
            this.UrlAsset = urlAsset;

            if (Stream.IsProtected)
                ContentKeyManager.Current.ContentKeySession?.Add(UrlAsset);
        }
    }
}