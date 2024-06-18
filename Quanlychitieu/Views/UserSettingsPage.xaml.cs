using Quanlychitieu.ViewModels;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.Views;

public partial class UserSettingsPage : ContentPage
{
    private readonly UserSettingsViewModel _vm;
    public UserSettingsPage(UserSettingsViewModel vm)
    {
        BindingContext = _vm = vm;
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        try
        {
            if (!_vm.IsPushPageWithNavService)
            {
                await _vm.InitAsync(null);
                await _vm.ViewIsAppearingAsync();
            }
        }
        finally
        {
            _vm.IsBusy = false;
        }
    }
}