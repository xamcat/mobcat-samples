using System;
using System.Collections.Generic;

namespace SampleNativeVideo.iOS.Model
{
    public class Stream
    {
        public Stream()
        {
            ContentKeyIDList = new List<string>();
        }

        public Stream(string name, string playlistUrl, bool isProtected, List<String> contentKeyIds = null)
        {
            Name = name;
            PlaylistUrl = playlistUrl;
            IsProtected = isProtected;
            ContentKeyIDList = contentKeyIds ?? new List<string>();
        }

        // The name of the stream.
        public string Name { get; set; }

        // The URL pointing to the HLS stream.
        public string PlaylistUrl { get; set; }

        // A Boolen value representing if the stream uses FPS.
        public bool IsProtected { get; set; }

        //A list of content IDs to use for loading content keys with FPS.
        public List<string> ContentKeyIDList { get; set; }
    }
}