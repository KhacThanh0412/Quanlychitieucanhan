using Quanlychitieu.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Navigation
{
    public static class NavMethodExtendsion
    {
        private static List<string> _initializePages = new List<string>();
        static HashSet<string> _navigationStack = new HashSet<string>();
        private static TaskCompletionSource<bool> _navigateCompletionSource;
        private static readonly object _syncLock = new object();

        public static void AddInitializePage(string page)
        {
            _initializePages.Add(page);
        }

        public static bool IsInitiallizePage(string page)
        {
            return _initializePages.SingleOrDefault(x => x.Equals(page.ToString())) == null;
        }

        public static void ClearInitializePage()
        {
            if (_initializePages != null)
                _initializePages.Clear();
        }

        public static BaseViewModel GetBaseViewModel(ContentPage p)
           => p?.BindingContext as BaseViewModel;

        public static T ResolvePage<T>()
            where T : ContentPage
        {
            return ServiceHelper.GetService<IServiceProvider>().GetService<T>();
        }

        public static async Task<bool> PushToNavigationStackAsync(string page)
        {
            if (_navigateCompletionSource != null && !_navigateCompletionSource.Task.IsCompleted)
            {
                await _navigateCompletionSource.Task;
            }

            _navigateCompletionSource = new TaskCompletionSource<bool>();
            if (_navigationStack.Count > 0 && _navigationStack.Contains(page))
            {
                _navigateCompletionSource.TrySetResult(false);
                return await _navigateCompletionSource.Task;
            }

            lock (_syncLock)
            {
                _navigationStack.Add(page);
            }

            _navigateCompletionSource.TrySetResult(true);
            return await _navigateCompletionSource.Task;
        }

        public static async Task<bool> PopToNavigationStackAsync(string page)
        {
            if (_navigateCompletionSource != null && !_navigateCompletionSource.Task.IsCompleted)
            {
                await _navigateCompletionSource.Task;
            }

            _navigateCompletionSource = new TaskCompletionSource<bool>();
            if (_navigationStack.Count == 0 && !_navigationStack.Contains(page))
            {
                _navigateCompletionSource.TrySetResult(false);
                return await _navigateCompletionSource.Task;
            }

            lock (_syncLock)
            {
                _navigationStack.Remove(page);
            }

            _navigateCompletionSource.TrySetResult(true);
            return await _navigateCompletionSource.Task;
        }

        public static async Task<bool> ClearNavigationStackAsync()
        {
            if (_navigateCompletionSource != null && !_navigateCompletionSource.Task.IsCompleted)
            {
                await _navigateCompletionSource.Task;
            }

            _navigateCompletionSource = new TaskCompletionSource<bool>();
            if (_navigationStack.Count == 0)
            {
                _navigateCompletionSource.TrySetResult(false);
                return await _navigateCompletionSource.Task;
            }

            lock (_syncLock)
            {
                _navigationStack.Clear();
            }

            _navigateCompletionSource.TrySetResult(true);
            return await _navigateCompletionSource.Task;
        }
    }
}
