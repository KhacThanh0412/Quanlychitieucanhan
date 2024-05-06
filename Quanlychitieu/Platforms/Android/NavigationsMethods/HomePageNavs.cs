using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class HomePageNavs
    {
        public async Task FromHomePageToUpsertExpenditure(Dictionary<string, object> navParams)
        {
            await Task.Delay(1);
            await Shell.Current.GoToAsync(nameof(UpSertExpenditurePage), navParams);
        }
    }
}
