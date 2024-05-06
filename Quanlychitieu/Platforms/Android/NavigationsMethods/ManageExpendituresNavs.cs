using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public static class ManageExpendituresNavs
    {
        public static async Task FromManageExpToUpsertExpenditures(Dictionary<string, object> navParams)
        {
            await Shell.Current.GoToAsync(nameof(UpSertExpenditurePage), true, navParams);
        }
        public static async Task FromUpsertExpToManageExpenditures(Dictionary<string, object> navParams)
        {
           await Shell.Current.GoToAsync(nameof(ManageExpenditures), true, navParams);
        }

        public static async Task FromManageExpToSingleMonthStats(Dictionary<string, object> navParams)
        {
            await Shell.Current.GoToAsync(nameof(StatisticsPage), true, navParams);
        }
        public static async Task ReturnOnce()
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}
