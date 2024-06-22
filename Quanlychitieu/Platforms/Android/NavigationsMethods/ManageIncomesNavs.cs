using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class ManageIncomesNavs
    {
        public async Task FromUpsertIncToManageIncome(Dictionary<string, object> navParams)
        {
            await Task.Delay(0);
        }
        public static async Task ReturnOnce()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
