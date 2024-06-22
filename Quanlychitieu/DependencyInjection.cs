using CommunityToolkit.Maui;
using Mopups.Interfaces;
using Mopups.Services;
using Quanlychitieu.Navigation;
using Quanlychitieu.Services;
using Quanlychitieu.ViewModels;
using Quanlychitieu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddTransient<HomePage>();
            services.AddSingleton<LoginPage>();
            services.AddTransient<UserSettingsPage>();
            services.AddTransient<IncomesPage>();
            services.AddTransient<AddIncomePage>();
            services.AddTransient<AddExpendituresPage>();
            services.AddSingleton<ManageExpenditures>();

            services.AddTransient<HomeViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddTransient<UserSettingsViewModel>();
            services.AddTransient<IncomesViewModel>();
            services.AddTransient<AddIncomeViewModel>();
            services.AddTransient<AddExpendituresViewModel>();
            services.AddTransient<ManageExpendituresViewModel>();

            return services;
        }
    }
}
