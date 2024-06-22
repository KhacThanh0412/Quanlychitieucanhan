using Quanlychitieu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Navigation
{
    public static class NavGraph
    {
        public static void RegisterRoute()
        {
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(UserSettingsPage), typeof(UserSettingsPage));
            Routing.RegisterRoute(nameof(IncomesPage), typeof(IncomesPage));
            Routing.RegisterRoute(nameof(AddIncomePage), typeof(AddIncomePage));
            Routing.RegisterRoute(nameof(ManageExpenditures), typeof(ManageExpenditures));
            Routing.RegisterRoute(nameof(AddExpendituresPage), typeof(AddExpendituresPage));
            Routing.RegisterRoute(nameof(AddSertDebtPage), typeof(AddSertDebtPage));
            Routing.RegisterRoute(nameof(DebtLendingPage), typeof(DebtLendingPage));
        }
    }
}
