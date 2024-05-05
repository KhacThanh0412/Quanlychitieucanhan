using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class LoginNavs
    {
        public async Task GoToHomePage()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
        public async Task GoToLoginInPage()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
