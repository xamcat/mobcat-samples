using System;
using Weather.Services.Abstractions;
using Xamarin.Essentials;

namespace Weather.Services
{
    public class ValueCacheService : IValueCacheService
    {
        public DateTime Load(string key, DateTime defaultValue) => Preferences.Get(key, defaultValue);

        public string Load(string key, string defaultValue) => Preferences.Get(key, defaultValue);

        public bool Load(string key, bool defaultValue) => Preferences.Get(key, defaultValue);

        public void Save(string key, DateTime value) => Preferences.Set(key, value);

        public void Save(string key, string value) => Preferences.Set(key, value);

        public void Save(string key, bool value) => Preferences.Set(key, value);
    }
}