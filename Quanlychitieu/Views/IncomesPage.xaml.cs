using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.Views;

public partial class IncomesPage : ContentPage
{
    private readonly IncomesViewModel _vm;
    public IncomesPage(IncomesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
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
