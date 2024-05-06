using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class MonthlyPlannedExpNavs
    {
        public async Task ToUpSertMonthlyPlanned(Dictionary<string, object> NavigationParamaters)
        {
            await Shell.Current.GoToAsync(nameof(UpSertMonthlyPlannedExpPage), true, NavigationParamaters);
        }

        public async Task ToDetailsMonthlyPlanned(Dictionary<string, object> NavigationParamaters)
        {
            await Shell.Current.GoToAsync(nameof(DetailsOfMonthlyPlannedExpPage), true, NavigationParamaters);
        }
        public async Task ReturnToDetailsMonthlyPlanned(Dictionary<string, object> NavigationParamaters)
        {
            await Shell.Current.GoToAsync("../DetailsOfMonthlyPlannedExpPage", true, NavigationParamaters);
        }

        public async Task ReturnOnce(Dictionary<string, object> NavigationParamaters)
        {
            await Shell.Current.GoToAsync("..", true, NavigationParamaters);
        }
        public async Task ReturnOnce()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
