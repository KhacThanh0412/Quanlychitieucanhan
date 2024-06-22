using Quanlychitieu.Models;

namespace Quanlychitieu.Views;

public partial class ManageExpenditures : ContentPage
{
    private readonly ManageExpendituresViewModel _vm;
    public ManageExpenditures(ManageExpendituresViewModel vm)
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
