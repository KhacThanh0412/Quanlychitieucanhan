using System;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quanlychitieu.Platforms.Android.NavigationsMethods
{
    public class HomePageNavs
    {
        public async Task FromHomePageToUpsertExpenditure(Dictionary<string, object> navParams)
        {
            await Shell.Current.GoToAsync(nameof(UpSertExpenditurePageM), navParams);
        }
    }
}

