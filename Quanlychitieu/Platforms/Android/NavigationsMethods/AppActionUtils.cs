using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public static class AppActionUtils
    {
        public static async Task HomePageQuickAddFlowOut()
        {
            var navParam = new Dictionary<string, object>
            {
                { "StartAction", 1 }
            };
            await GoToHome(navParam);
        }
        public static async Task HomePageQuickAddFlowIn()
        {
            var navParam = new Dictionary<string, object>
            {
                { "StartAction", 2 }
            };
            await GoToHome(navParam);
        }

        private static async Task GoToHome(Dictionary<string, object> navParam)
        {
            await Shell.Current.GoToAsync("//HomePage", default, navParam);
        }
    }
}
