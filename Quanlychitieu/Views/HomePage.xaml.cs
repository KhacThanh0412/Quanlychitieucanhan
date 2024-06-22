using Quanlychitieu.ViewModels;

namespace Quanlychitieu.Views;

public partial class HomePage : ContentPage
{
    readonly HomeViewModel _vm;
    public HomePage(HomeViewModel vm)
	{
        BindingContext = _vm = vm;
        InitializeComponent();
    }

    protected override async void OnAppearing()
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