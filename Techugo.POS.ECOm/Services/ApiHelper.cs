using System;
using System.Threading.Tasks;

namespace Techugo.POS.ECOm.Services
{
    public static class ApiHelper
    {
        // Wrap an async API call and show the global loader while it runs
        public static async Task<T> RunWithLoader<T>(Func<Task<T>> action, string message = "Loading...")
        {
            LoadingService.Instance.Show(message);
            try
            {
                return await action().ConfigureAwait(false);
            }
            finally
            {
                // ensure hide runs on UI thread inside LoadingService.Hide
                LoadingService.Instance.Hide();
            }
        }

        public static async Task RunWithLoader(Func<Task> action, string message = "Loading...")
        {
            LoadingService.Instance.Show(message);
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                LoadingService.Instance.Hide();
            }
        }
    }
}