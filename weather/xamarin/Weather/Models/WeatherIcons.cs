using System;
namespace Weather.Models
{
    public static class WeatherIcons
    {
        public static string Lookup(string WeatherDescription)
        {
            char symbolChar = '\uf00d';

            switch (WeatherDescription.ToLower())
            {
                case "sunny":
                    symbolChar = '\uf00d';
                    break;
                case "cloudy":
                    symbolChar = '\uf002';
                    break;
                case "fog":
                    symbolChar = '\uf003';
                    break;
                case "hail":
                    symbolChar = '\uf004';
                    break;
                case "snow":
                    symbolChar = '\uf00a';
                    break;
                case "showers":
                    symbolChar = '\uf009';
                    break;
                case "thunderstorms":
                    symbolChar = '\uf010';
                    break;
                case "rain":
                    symbolChar = '\uf006';
                    break;
                case "windy":
                    symbolChar = '\uf085';
                    break;
                case "smoke":
                    symbolChar = '\uf062';
                    break;
                case "alien":
                    symbolChar = '\uf075';
                    break;
            }
            return symbolChar.ToString();
        }
    }
}