using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quanlychitieu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [RelayCommand]
        async Task GoToDetails()
        {
            await Shell.Current.GoToAsync(nameof(RecentTransactionsView), true);
        }
    }
}
