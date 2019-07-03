using System.Threading.Tasks;
using Microsoft.MobCAT;

namespace News.Helpers
{
    public static class TaskHelper
    {
        public static void HandleResult(this Task source)
        {
            if (source == null)
                return;

            source.ContinueWith(r =>
            {
                var ex = r.Exception?.Flatten();
                Logger.Error(ex);
                System.Diagnostics.Debug.WriteLine(ex);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
