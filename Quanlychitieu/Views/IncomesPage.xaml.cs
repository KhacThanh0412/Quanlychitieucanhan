using Quanlychitieu.ViewModels.Incomes;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.Views;

public partial class IncomesPage : ContentPage
{
    private readonly IncomesViewModel _vm;
    List<string> FilterResult { get; set; }
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
    private async void ExportToPDFImageButton_Clicked(object sender, EventArgs e)
    {
        await Task.Delay(1);
    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Task.Delay(1);
        //PopUpCloseResult result = (PopUpCloseResult)await Shell.Current.ShowPopupAsync(new InputPopUpPage(InputType.Numeric, new List<string>() { "Amount" }, "Enter New Pocket Money"));
        //if (result.Result is PopupResult.OK)
        //{
        //    double NewAmount = (double)result.Data;
        //    await viewModel.ResetUserPocketMoney(NewAmount);
        //}
    }
}
