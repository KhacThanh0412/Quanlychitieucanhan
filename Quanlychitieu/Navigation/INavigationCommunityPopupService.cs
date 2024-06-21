using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Navigation
{
    public interface INavigationCommunityPopupService
    {
        Task<object> PushAsync<TViewModel>(object param = null)
            where TViewModel : BaseViewModel;
        void PopPopup();
        Stack<Popup> GetPopupStack();
    }
}
