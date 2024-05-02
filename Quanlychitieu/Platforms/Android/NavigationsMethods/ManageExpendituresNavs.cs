using System;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public static class ManageExpendituresNavs
    {
        public static async Task FromManageExpToUpsertExpenditures(Dictionary<string, object> navParams)
        {
            await Shell.Current.GoToAsync(nameof(UpSertExpenditurePageM), true, navParams);
        }
        public static async Task FromUpsertExpToManageExpenditures(Dictionary<string, object> navParams)
        {
            await Shell.Current.GoToAsync(nameof(ManageExpendituresM), true, navParams);
        }

        public static async Task FromManageExpToSingleMonthStats(Dictionary<string, object> navParams)
        {
            await Shell.Current.GoToAsync(nameof(StatisticsPageM), true, navParams);
        }
        public static async Task ReturnOnce()
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}

