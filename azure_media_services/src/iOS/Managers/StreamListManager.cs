using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using SampleNativeVideo.iOS.Model;

namespace SampleNativeVideo.iOS.Managers
{
    public class StreamListManager
    {
        public static StreamListManager Current = new StreamListManager();
        public List<Stream> Streams = new List<Stream>();

        public StreamListManager()
        {
            var streamsFilepath = NSBundle.MainBundle.GetUrlForResource("Streams", "plist");

            if (streamsFilepath == null)
                return;

            NSError error;
            var plistValues = NSArray.FromUrl(streamsFilepath, out error);

            if (error != null)
                throw new Exception("Error parsing streams.plist");
                
            for (nuint i = 0; i < plistValues.Count; i++)
            {
                var value = plistValues.ValueAt(i);
                var item = plistValues.GetItem<NSDictionary>(i);

                if (item == null)
                    break;

                var name = item["name"].ToString();
                var playlistUrl = item["playlist_url"].ToString();
                var isProtected = (NSNumber)item.ValueForKey(new NSString("is_protected"));

                var contentKeys = item["content_key_id_list"] as NSArray;
                var contentKeyIds = NSArray.StringArrayFromHandle(contentKeys.Handle)?.ToList();

                Streams.Add(new Stream(name, playlistUrl, isProtected.BoolValue, contentKeyIds));
            }
        }
    }
}