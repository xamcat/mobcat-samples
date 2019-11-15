using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using SampleNativeVideo.iOS.Model;

namespace SampleNativeVideo.iOS.Managers
{
    public class ContentKeyDelegate : AVContentKeySessionDelegate, IContentKeyDelegate
    {
        // Certificate Url: https://openidconnectweb.azurewebsites.net/Content/FPSAC.cer
        private string CertUrl { get; set; }
        public const string DidSaveAllPersistableContentKey = "ContentKeyDelegateDidSaveAllPersistableContentKey";

        private List<string> pendingPersistableContentKeyIdentifiers = new List<string>();
        private Dictionary<string, string> contentKeyToStreamNameMap = new Dictionary<string, string>();

        public ContentKeyDelegate()
        {
            CertUrl = Path.Combine(NSBundle.MainBundle.BundlePath, "FPSAC.cer");
        }

        private NSData RequestApplicationCertificate()
        {
            NSData applicationCertificate = null;

            try
            {
                applicationCertificate = NSData.FromUrl(new NSUrl(CertUrl, false));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading FairPlay application certificate: {ex.Message}");
            }

            return applicationCertificate;
        }

        private void HandleStreamingContentKeyRequest(AVContentKeyRequest keyRequest)
        {
            var contentKeyIdentifierString = keyRequest.Identifier.ToString();

            if (string.IsNullOrEmpty(contentKeyIdentifierString))
            {
                Debug.WriteLine("Failed to retrieve the assetID from the keyRequest!");
                return;
            }

            var contentKeyIdentifierUrl = new NSUrl(contentKeyIdentifierString);
            var assetIdString = contentKeyIdentifierUrl.Host;
            var assetIdData = NSData.FromString(assetIdString, NSStringEncoding.UTF8);

            Action provideOnlineKey = () => 
            {
                var applicationCertificate = RequestApplicationCertificate();

                try
                {
                    var keys = new[] { new NSString(AVContentKeyRequest.ProtocolVersions) };
                    var numbers = new NSMutableArray<NSNumber>();
                    numbers.Add(new NSNumber(1));
                    var objects = new NSObject[] { numbers };
                    var options = new NSDictionary<NSString, NSObject>(keys, objects);
                    keyRequest.MakeStreamingContentKeyRequestData(applicationCertificate, assetIdData, options, async (data, error) => {

                        var ckcData = await this.RequestContentKeyFromKeySecurityModule(data, assetIdString);

                        if (ckcData == null)
                            return;

                        var keyResponse = AVContentKeyResponse.Create(ckcData);

                        keyRequest.Process(keyResponse);

                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to make streaming content key request data: {ex.Message}");
                }
            };

            /*
             When you receive an AVContentKeyRequest via -contentKeySession:didProvideContentKeyRequest:
             and you want the resulting key response to produce a key that can persist across multiple
             playback sessions, you must invoke -respondByRequestingPersistableContentKeyRequest on that
             AVContentKeyRequest in order to signal that you want to process an AVPersistableContentKeyRequest
             instead. If the underlying protocol supports persistable content keys, in response your
             delegate will receive an AVPersistableContentKeyRequest via -contentKeySession:didProvidePersistableContentKeyRequest:.
             */
            if (ShouldRequestPersistableContentKey(assetIdString) || PersistableContentKeyExistsOnDisk(assetIdString))
            {
                try
                {
                    NSError error;
                    keyRequest.RespondByRequestingPersistableContentKeyRequest(out error);

                    if (error != null)
                        throw new Exception($"Error requesting persistable content key: {error.ToString()}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    /*
                    This case will occur when the client gets a key loading request from an AirPlay Session.
                    You should answer the key request using an online key from your key server.
                    */
                    provideOnlineKey();
                }

                return;
            }

            try
            {
                provideOnlineKey();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        static HttpClient client = new HttpClient();

        async Task<NSData> RequestContentKeyFromKeySecurityModule(NSData spcData, String assetID)
        {
            NSData ckcData = null;

            if (spcData == null)
                return null;

            try
            {
                var str = spcData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                var postString = $"spc={str}&assetId={assetID}";

                var postData = NSData.FromString(postString);

                var content = new ByteArrayContent(postData.ToArray());

                // TODO: You can replace this hard-coded endpoint with your own if you have one set up
                var response = await client.PostAsync("https://willzhanmswest.keydelivery.westus.media.azure.net/FairPlay/?kid=bf05ec87-4fff-4488-aa45-828fe8d7f840", content);

                var responseString = (await response.Content.ReadAsStringAsync()).Replace("<ckc>", "").Replace("</ckc>", "");

                ckcData = new NSData(responseString, NSDataBase64DecodingOptions.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return ckcData;
        }

        public void RequestPersistableContentKeys(Asset asset)
        {
            foreach (var identifier in asset.Stream.ContentKeyIDList)
            {
                var contentKeyIdentifierUrl = new NSUrl(new NSString(identifier));

                if (contentKeyIdentifierUrl == null)
                    return;

                var assetIdString = contentKeyIdentifierUrl.Host;

                pendingPersistableContentKeyIdentifiers.Add(assetIdString);
                contentKeyToStreamNameMap[assetIdString] = asset.Stream.Name;
                
                ContentKeyManager.Current.ContentKeySession.ProcessContentKeyRequest(new NSString(identifier), null, null);
            }
        }

        private bool ShouldRequestPersistableContentKey(string identifier)
        {
            return pendingPersistableContentKeyIdentifiers.Contains(identifier);
        }

        public override void DidProvideContentKeyRequest(AVContentKeySession session, AVContentKeyRequest keyRequest)
        {
            HandleStreamingContentKeyRequest(keyRequest);
        }

        public override void DidProvidePersistableContentKeyRequest(AVContentKeySession session, AVPersistableContentKeyRequest keyRequest)
        {
            HandlePersistableContentKeyRequest(keyRequest);
        }

        private void WritePersistableContentKey(NSData contentKey, NSString contentKeyIdentifier)
        {
            var fileUrl = CreateUrlForPersistableContentKey(contentKeyIdentifier);

            try
            {
                contentKey.Save(fileUrl, true);           
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void HandlePersistableContentKeyRequest(AVPersistableContentKeyRequest keyRequest)
        {
            /*
            The key ID is the URI from the EXT-X-KEY tag in the playlist (e.g. "skd://key65") and the
            asset ID in this case is "key65".
            */

            var contentKeyIdentifierString = keyRequest.Identifier as NSString;

            if (contentKeyIdentifierString == null)
            {
                Debug.WriteLine("Failed to retrieve the assetID from the keyRequest!");
                return;
            }

            var contentKeyIdentifierUrl = new NSUrl(contentKeyIdentifierString);
            var assetIdString = contentKeyIdentifierUrl.Host;
            var assetIdData = NSData.FromString(assetIdString, NSStringEncoding.UTF8);

            Action<NSData, NSError> completionHandler = async (data, error) =>
            {

                if (error != null)
                {
                    keyRequest.Process(error);
                    pendingPersistableContentKeyIdentifiers.Remove(assetIdString);
                    return;
                }

                if (data == null)
                    return;

                try
                {
                    var ckcData = await RequestContentKeyFromKeySecurityModule(data, assetIdString);
                    NSData persistentKey = keyRequest.GetPersistableContentKey(ckcData, null, out error);

                    WritePersistableContentKey(persistentKey, new NSString(assetIdString));

                    /*
                     AVContentKeyResponse is used to represent the data returned from the key server when requesting a key for
                     decrypting content.
                     */
                    var keyResponse = AVContentKeyResponse.Create(persistentKey);

                    /*
                     Provide the content key response to make protected content available for processing.
                     */
                    keyRequest.Process(keyResponse);

                    string assetName = string.Empty;
                    bool assetRemoved = false;

                    if (contentKeyToStreamNameMap.TryGetValue(assetIdString, out assetName))
                        assetRemoved = contentKeyToStreamNameMap.Remove(assetIdString);
                        
                    if (!string.IsNullOrWhiteSpace(assetName) && assetRemoved && !contentKeyToStreamNameMap.ContainsKey(assetIdString))
                    {
                        var userInfo = new Dictionary<string, object>();
                        userInfo["name"] = assetName;

                        var userInfoDictionary = NSDictionary.FromObjectsAndKeys(userInfo.Values.ToArray(), userInfo.Keys.ToArray());
                        NSNotificationCenter.DefaultCenter.PostNotificationName(ContentKeyDelegate.DidSaveAllPersistableContentKey, null, userInfoDictionary);
                    }

                    pendingPersistableContentKeyIdentifiers.Remove(assetIdString);
                }
                catch (Exception ex)
                {
                    pendingPersistableContentKeyIdentifiers.Remove(assetIdString);
                    Debug.WriteLine(ex.Message);
                }
            };

            try
            {
                var applicationCertificate = RequestApplicationCertificate();

                var keys = new[] { new NSString(AVContentKeyRequest.ProtocolVersions) };
                var numbers = new NSMutableArray<NSNumber>();
                numbers.Add(new NSNumber(1));
                var objects = new NSObject[] { numbers };
                var options = new NSDictionary<NSString, NSObject>(keys, objects);

                if (PersistableContentKeyExistsOnDisk(assetIdString))
                {
                    var urlToPersistableKey = CreateUrlForPersistableContentKey(assetIdString);
                    var contentKey = NSFileManager.DefaultManager.Contents(urlToPersistableKey.Path);

                    if (contentKey == null)
                    {
                        pendingPersistableContentKeyIdentifiers.Remove(assetIdString);

                        /*
                         Key requests should never be left dangling.
                         Attempt to create a new persistable key.
                         */
                        keyRequest.MakeStreamingContentKeyRequestData(applicationCertificate, assetIdData, options, completionHandler);

                        return;
                    }

                    /*
                     Create an AVContentKeyResponse from the persistent key data to use for requesting a key for
                     decrypting content.
                     */

                    var keyResponse = AVContentKeyResponse.Create(contentKey);
                    keyRequest.Process(keyResponse);

                    return;
                }

                keyRequest.MakeStreamingContentKeyRequestData(applicationCertificate, assetIdData, options, completionHandler);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failure responding to an AVPersistableContentKeyRequest when attemping to determine if key is already available for use on disk. {ex.Message}");
            }
        }

        public void DeleteAllPeristableContentKeys(Asset asset)
        {
            foreach(var contentKeyIdentifier in asset.Stream.ContentKeyIDList)
            {
                DeletePersistableContentKey(contentKeyIdentifier);
            }
        }

        private void DeletePersistableContentKey(string identifier)
        {
            if (!PersistableContentKeyExistsOnDisk(identifier))
                return;

            var contentKeyUrl = CreateUrlForPersistableContentKey(identifier);

            try
            {
                NSError error;
                NSFileManager.DefaultManager.Remove(contentKeyUrl, out error);

                if (error != null)
                    throw new Exception(error.DebugDescription);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured removing the persisted content key: {ex.Message}", ex);
            }
        }

        private NSUrl CreateUrlForPersistableContentKey(string identifier)
        {
            return GetContentKeyDirectory().Append($"{identifier}-Key", false);

        }

        private bool PersistableContentKeyExistsOnDisk(string identifier)
        {
            var contentKeyUrl = CreateUrlForPersistableContentKey(identifier);

            return NSFileManager.DefaultManager.FileExists(contentKeyUrl.Path);
        }

        private NSUrl GetContentKeyDirectory()
        {
            var documentPath = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, true).FirstOrDefault();

            if (documentPath == null)
                throw new Exception("Unable to determine library URL");

            var documentUrl = new NSUrl(documentPath, true);

            var contentKeyDirectory = documentUrl.Append(".keys", true);

            if (!NSFileManager.DefaultManager.FileExists(contentKeyDirectory.Path))
            {
                try
                {
                    NSFileManager.DefaultManager.CreateDirectory(contentKeyDirectory.AbsoluteString, false, null);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to create directory for content keys at path: {contentKeyDirectory.Path}", ex);
                }
            }

            return contentKeyDirectory;
        }
    }
}