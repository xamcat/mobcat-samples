using System;
using System.Threading.Tasks;
using Weather.Models;

namespace Weather.Services.Abstractions
{
    public interface IGeolocationService
    {
        Task<Coordinates> GetLastKnownLocationAsync();
        Task<Coordinates> GetLocationAsync();
    }
}