using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
