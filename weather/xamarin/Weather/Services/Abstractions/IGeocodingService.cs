using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.Models;

namespace Weather.Services.Abstractions
{
    public interface IGeocodingService
    {
        Task<IEnumerable<Place>> GetPlacesAsync(Coordinates location);
    }
}
