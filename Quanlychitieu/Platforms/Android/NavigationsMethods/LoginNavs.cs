using System;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class LoginNavs
    {
        public async Task GoToHomePage()
        {
            await Shell.Current.GoToAsync("//HomePageM");
        }
        public async Task GoToLoginInPage()
        {
            await Shell.Current.GoToAsync("//LoginM");
        }
    }
}

