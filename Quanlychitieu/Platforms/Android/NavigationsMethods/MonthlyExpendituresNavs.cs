using System;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class MonthlyExpendituresNavs
    {
        //public async Task FromManageMonthlyPlannedToDetailsSingleMonthPlan(Dictionary<string, object> navParams)
        //{

        //}
        //public async Task NavigateToUpsertMonthlyPlannedExpenditure(Dictionary<string, object> navParams)
        //{

        //}

        public async Task ReturnOnce()
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}

