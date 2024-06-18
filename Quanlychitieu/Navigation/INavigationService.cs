using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Navigation
{
    public interface INavigationService
    {
        Task PushToPageAsync<T>(object param = null, bool isPushModal = false, bool animate = true)
            where T : ContentPage;
        Task PopPageAsync(bool isPopModal = false, bool animate = true);
        Task PopToPageInStackAsync(Type targetPageType, bool isPopModal = false, bool animate = true);
        Task PopToRootAsync(bool animate = true);
        int GetNumberStack();
        Task PushToWebViewAsync<T>(object param)
            where T : ContentPage;
    }
}
