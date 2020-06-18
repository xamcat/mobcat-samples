namespace PushDemoApi.Models
{
    public class PushTemplates
    {
        public class Generic
        {
            public const string Android = "{ \"notification\": { \"title\" : \"PushDemo\", \"body\" : \"$(alertMessage)\"}, \"data\" : { \"action\" : \"$(alertAction)\" } }";
            public const string iOS = "{ \"aps\" : {\"alert\" : \"$(alertMessage)\"}, \"action\" : \"$(alertAction)\" }";
        }

        public class Silent
        {
            public const string Android = "{ \"data\" : {\"message\" : \"$(alertMessage)\", \"action\" : \"$(alertAction)\"} }";
            public const string iOS = "{ \"aps\" : {\"content-available\" : 1, \"apns-priority\": 5, \"sound\" : \"\", \"badge\" : 0}, \"message\" : \"$(alertMessage)\", \"action\" : \"$(alertAction)\" }";
        }
    }
}