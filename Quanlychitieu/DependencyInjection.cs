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
            services.AddSingleton<FinanceService>();
            
            services.AddTransient<HomePage>();
            services.AddTransient<RecentTransactionsView>();
            services.AddSingleton<LoginPage>();

            services.AddTransient<HomeViewModel>();
            services.AddTransient<RecentTransactionsViewModel>();
            return services;
        }
    }
}
