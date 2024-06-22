using Quanlychitieu.Navigation;
using Quanlychitieu.Views.Debts;
using Quanlychitieu.Views.Expenditures.PlannedExpenditures.MonthlyPlannedExp;
using Quanlychitieu.Views.Statistics;

namespace Quanlychitieu
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            NavGraph.RegisterRoute();
            Routing.RegisterRoute(nameof(UpSertExpenditurePage), typeof(UpSertExpenditurePage));

            Routing.RegisterRoute(nameof(StatisticsPage), typeof(StatisticsPage));
            Routing.RegisterRoute(nameof(SingleMonthStatsPage), typeof(SingleMonthStatsPage));

            Routing.RegisterRoute(nameof(EditUserSettingsPage), typeof(EditUserSettingsPage));

            Routing.RegisterRoute(nameof(ManageMonthlyPlannedExpendituresPage), typeof(ManageMonthlyPlannedExpendituresPage));
            Routing.RegisterRoute(nameof(DetailsOfMonthlyPlannedExpPage), typeof(DetailsOfMonthlyPlannedExpPage));
            Routing.RegisterRoute(nameof(UpSertMonthlyPlannedExpPage), typeof(UpSertMonthlyPlannedExpPage));

            Routing.RegisterRoute(nameof(DebtsOverviewPage), typeof(DebtsOverviewPage));
            Routing.RegisterRoute(nameof(ManageBorrowingsPage), typeof(ManageBorrowingsPage));
            Routing.RegisterRoute(nameof(ManageLendingsPage), typeof(ManageLendingsPage));
            Routing.RegisterRoute(nameof(SingleDebtDetailsPage), typeof(SingleDebtDetailsPage));
        }
    }
}
