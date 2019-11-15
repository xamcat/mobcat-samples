using System;
using AVFoundation;
using CoreFoundation;

namespace SampleNativeVideo.iOS.Managers
{
    public class ContentKeyManager
    {
        private DispatchQueue contentKeyDelegateQueue = new DispatchQueue("com.mobcat.SampleNativeVideo.ContentKeyDelegateQueue");
        public static ContentKeyManager Current = new ContentKeyManager();

        public AVContentKeySession ContentKeySession { get; private set; }
        public IContentKeyDelegate ContentKeyDelegate { get; set; }

        public ContentKeyManager()
        {
            var storagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var constantVal = AVContentKeySystem.FairPlayStreaming.GetConstant().ToString();

            // TODO: Handle error when running in the simulator!
            ContentKeySession = AVContentKeySession.Create(constantVal);
            ContentKeyDelegate = new ContentKeyDelegate(); 
            ContentKeySession?.SetDelegate(ContentKeyDelegate, contentKeyDelegateQueue);
        }
    }
}