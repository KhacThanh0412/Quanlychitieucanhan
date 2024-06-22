using CommunityToolkit.Maui;
using Quanlychitieu.Navigation;

namespace Quanlychitieu
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<HttpClient>();

            services.AddSingleton<IDataAccessRepo, DataAccessRepo>();
            services.AddSingleton<IUsersRepository, UserRepository>();
            services.AddSingleton<ISettingsServiceRepository, SettingsServiceRepository>();
            services.AddSingleton<IIncomeRepository, IncomeRepository>();
            services.AddSingleton<IExpendituresRepository, ExpendituresRepository>();
            services.AddSingleton<INavigationCommunityPopupService, NavigationCommunityPopupService>();
            services.AddSingleton<IDebtRepository, DebtRepository>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddTransient<HomePage>();
            services.AddTransient<LoginPage>();
            services.AddTransient<UserSettingsPage>();
            services.AddTransient<IncomesPage>();
            services.AddTransient<AddIncomePage>();
            services.AddTransient<AddExpendituresPage>();
            services.AddTransient<ManageExpenditures>();
            services.AddTransient<DebtsOverviewPage>();
            services.AddTransient<AddSertDebtPage>();
            services.AddTransient<DebtLendingPage>();

            services.AddTransient<HomeViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<UserSettingsViewModel>();
            services.AddTransient<IncomesViewModel>();
            services.AddTransient<AddIncomeViewModel>();
            services.AddTransient<AddExpendituresViewModel>();
            services.AddTransient<ManageExpendituresViewModel>();
            services.AddTransient<ManageDebtsViewModel>();
            services.AddTransient<AddSertDebtViewModel>();
            services.AddTransient<DebtLendingViewModel>();

            return services;
        }
    }
}
