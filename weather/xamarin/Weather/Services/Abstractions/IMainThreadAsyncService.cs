using System;
using System.Threading.Tasks;

namespace Weather.Services.Abstractions
{
    public interface IMainThreadAsyncService
    {
        Task<T> RunOnMainThreadAsync<T>(Func<Task<T>> func);
    }
}
