using System;
namespace Weather.Services.Abstractions
{
    public interface IValueCacheService
    {
        void Save(string key, DateTime value);
        void Save(string key, string value);
        void Save(string key, bool value);

        DateTime Load(string key, DateTime defaultValue);
        string Load(string key, string defaultValue);
        bool Load(string key, bool defaultValue);
    }
}
