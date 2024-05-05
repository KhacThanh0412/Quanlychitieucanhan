using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class ManageIncomesNavs
    {
        public async Task FromManageIncToUpsertIncome(Dictionary<string, object> navParams)
        {
            await Task.Delay(1);
            // await Shell.Current.GoToAsync(nameof(UpSertIncomePageM), true, navParams);
        }
        public async Task FromUpsertIncToManageIncome(Dictionary<string, object> navParams)
        {
            await Task.Delay(1);
            // await Shell.Current.GoToAsync(nameof(ManageIncomesM), true, navParams);
        }
        public static async Task ReturnOnce()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
