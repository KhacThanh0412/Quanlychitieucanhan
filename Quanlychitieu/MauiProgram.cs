using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using UraniumUI;
using SkiaSharp.Views.Maui.Controls.Hosting;
using InputKit.Handlers;
using CommunityToolkit.Maui.Storage;
using Plugin.Maui.CalendarStore;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.DataAccess.Repositories;
using Quanlychitieu.DataAccess;
using Quanlychitieu.ViewModels;
using Quanlychitieu.ViewModels.Expenditures;
using Quanlychitieu.ViewModels.Incomes;
using Quanlychitieu.ViewModels.Settings;
using Quanlychitieu.ViewModels.Expenditures.PlannedExpenditures.MonthlyPlannedExp;
using Quanlychitieu.ViewModels.Statistics;
using Quanlychitieu.ViewModels.Debts;
using Quanlychitieu.Views;
using Quanlychitieu.Views.Expenditures;
using Quanlychitieu.Views.Incomes;
using Quanlychitieu.Views.Settings;
using Quanlychitieu.Views.Expenditures.PlannedExpenditures.MonthlyPlannedExp;
using Quanlychitieu.Views.Statistics;
using Quanlychitieu.Views.Debts;

namespace Quanlychitieu
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp(true)
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialIconFonts();
                    fonts.AddFontAwesomeIconFonts();
                })
                .ConfigureEssentials(essentials =>
                {
                    essentials
                    .AddAppAction("add_flow_out", "Add Flow Out", "Add a Flow Out")
                    //.AddAppAction("add_flow_in", "Add Flow In", "Add a Flow In", "request_money_d.png")
                    .OnAppAction(App.HandleAppActions);
                })

                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseUraniumUIBlurs()
                .UseMauiCommunityToolkit();

            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler(typeof(Shell), typeof(MyShellRenderer));
#endif

                handlers.AddInputKitHandlers();
                handlers.AddUraniumUIHandlers();
            });
            builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);
            /*----------------------- REGISTERING Repositories ------------------------------------------------------------------------*/
            builder.Services.AddSingleton(CalendarStore.Default);
            builder.Services.AddSingleton<IExpendituresRepository, ExpendituresRepository>();
            builder.Services.AddSingleton<IIncomeRepository, IncomeRepository>();
            builder.Services.AddSingleton<IDebtRepository, DebtRepository>();

            builder.Services.AddSingleton<IDataAccessRepo, DataAccessRepo>();
            builder.Services.AddSingleton<ISettingsServiceRepository, SettingsServiceRepository>();
            builder.Services.AddSingleton<IUsersRepository, UserRepository>();
            // builder.Services.AddSingleton<IOnlineCredentialsRepository, OnlineDataAccessRepository>();
            builder.Services.AddSingleton<IPlannedExpendituresRepository, PlannedExpendituresRepository>();

            /*--------------------ADDING VIEWMODELS----------------------------------------------------------------------------------------*/

            /*-- Section for HomePage AND Login --*/
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddTransient<LoginViewModel>();

            /*-- Section for Expenditures --*/
            builder.Services.AddSingleton<UpSertExpenditureViewModel>();
            builder.Services.AddSingleton<ManageExpendituresViewModel>();

            /* -- Section for Incomes --*/
            //builder.Services.AddSingleton<UpSertIncomeViewModel>();
            builder.Services.AddSingleton<ManageIncomesViewModel>();
            builder.Services.AddSingleton<UserSettingsViewModel>();

            /*-- Section for Planned Expenditures --*/
            builder.Services.AddSingleton<ManageMonthlyMonthlyPlannedExpendituresViewModel>();
            builder.Services.AddSingleton<DetailsOfMonthlyPlannedExpViewModel>();
            builder.Services.AddSingleton<UpSertMonthlyPlannedExpViewModel>();

            /*-- Section for Statistics --*/
            builder.Services.AddSingleton<StatisticsPageViewModel>();
            builder.Services.AddSingleton<SingleMonthStatsPageViewModel>();

            /*-- Section for Debts --*/
            builder.Services.AddSingleton<ManageDebtsViewModel>();
            builder.Services.AddSingleton<UpSertDebtViewModel>();
            /*-------------------------------REGISTERING MOBILE VIEWS ---------------------------------------------------------------*/

            /*--  REGISTERING MOBILE VIEWS --*/
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<LoginPage>();

            /*-- Section for Expenditures --*/
            builder.Services.AddSingleton<ManageExpenditures>();
            builder.Services.AddSingleton<UpSertExpenditurePage>();

            /*-- Section for Incomes --*/
            builder.Services.AddSingleton<ManageIncomes>();
            builder.Services.AddSingleton<UpSertIncomePage>();

            /*-- Section for Settings --*/
            builder.Services.AddSingleton<UserSettingsPage>();
            builder.Services.AddSingleton<ApplicationSettingsPage>();
            builder.Services.AddTransient<EditUserSettingsPage>();

            /*-- Section for Monthly Planned Expenditures --*/
            builder.Services.AddSingleton<ManageMonthlyPlannedExpendituresPage>();
            builder.Services.AddSingleton<DetailsOfMonthlyPlannedExpPage>();
            builder.Services.AddSingleton<UpSertMonthlyPlannedExpPage>();

            builder.Services.AddSingleton<StatisticsPage>();
            builder.Services.AddSingleton<SingleMonthStatsPage>();

            /* -- Section for Debts --*/
            builder.Services.AddSingleton<DebtsOverviewPage>();

            builder.Services.AddSingleton<ManageBorrowingsPage>();
            builder.Services.AddSingleton<ManageLendingsPage>();
            builder.Services.AddSingleton<SingleDebtDetailsPage>();

            /*--------------------------------------------------------------------------------------------------------------------------------*/
            return builder.Build();
        }
    }
}