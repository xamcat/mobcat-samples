using System;
using System.Threading.Tasks;
using Microsoft.MobCAT;
using Weather.Services.Abstractions;
using Xamarin.Essentials;

namespace Weather.Services
{
    public class MainThreadAsyncService : IMainThreadAsyncService
    {
        public Task<T> RunOnMainThreadAsync<T>(Func<Task<T>> func)
        {
            var tcs = new TaskCompletionSource<T>();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var result = await func.Invoke();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}
