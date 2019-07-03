using System;
namespace Weather.Services.Abstractions
{
    public interface ITimeOfDayImageService
    {
		string GetImageForDateTime(DateTime dateTime);
    }
}
