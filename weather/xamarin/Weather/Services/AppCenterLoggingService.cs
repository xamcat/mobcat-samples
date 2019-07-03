using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using Microsoft.MobCAT.Services;

namespace Weather.Services
{
    public class AppCenterLoggingService : ILoggingService
    {
        public void Log(LogType logType, string message)
        {
            WriteToConsole(message);
        }

        public void Log(LogType logType, string message, Exception exception)
        {
            switch (logType)
            {
                case LogType.ERROR:
                case LogType.FATAL:
                    Crashes.TrackError(exception, properties: new Dictionary<string, string> { { nameof(message), message } });
                    break;
                default:
                    WriteToConsole(message);
                    break;
            }
        }

        private void WriteToConsole(string message)
        {
            Console.WriteLine($"Exception occurred: {message}");
        }
    }
}
