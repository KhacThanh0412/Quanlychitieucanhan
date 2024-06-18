using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public static class ServiceHelper
    {
        public static T GetService<T>() => Current.GetService<T>();

        public static IServiceProvider Current => IPlatformApplication.Current.Services;

        public static TViewModel GetViewModel<TViewModel>()
            where TViewModel : BaseViewModel
        {
            return Current.GetService<TViewModel>() ?? throw new ResolveViewModelException<TViewModel>();
        }

        public static TViewModel GetViewModelObservable<TViewModel>()
            where TViewModel : ObservableObject
        {
            return Current.GetService<TViewModel>() ?? throw new ResolveViewModelException<TViewModel>();
        }
    }
}
