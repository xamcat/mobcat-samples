using System;
using System.Linq;
using Microsoft.MobCAT;
using Weather.Extensions;
using Weather.Services.Abstractions;

namespace Weather.Services
{
    public class TimeOfDayImageService : ITimeOfDayImageService
    {
        private string[] sunriseImages = {
            "sunrise1",
            "sunrise2",
            "sunrise3",
            "sunrise4"
        };

        private string[] middayImages = {
            "midday1",
            "midday2",
            "midday3",
            "midday4"
        };

        private string[] eveningImages = {
            "evening1",
            "evening2",
            "evening3",
            "evening4"
        };

        private string[] nighttimeImages = {
            "night1",
            "night2",
            "night3",
            "night4"
        };

        private string[] genericTimeImages = {
            "earth1",
            "earth2",
            "earth3",
            "earth4"
        };

        public string GetImageForDateTime(DateTime dateTime)
        {
            try
            {
                var random = new Random();

                var sunriseStartHour = 5;
                var middayStartHour = 10;
                var eveningStartHour = 17;
                var nighttimeStartHour = 20;

                if (dateTime.Hour >= sunriseStartHour && dateTime.Hour < middayStartHour)
                {
                    return sunriseImages.Random();
                }
                if (dateTime.Hour >= middayStartHour && dateTime.Hour < eveningStartHour)
                {
                    return middayImages.Random();
                }
                if (dateTime.Hour >= eveningStartHour && dateTime.Hour < nighttimeStartHour)
                {
                    return eveningImages.Random();
                }
                nighttimeImages.Random();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return genericTimeImages.Random();
        }
    }
}
