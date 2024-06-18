using Quanlychitieu.ViewModels.Expenditures;
using Quanlychitieu.Models;

namespace Quanlychitieu.Views.Expenditures;

public partial class UpSertExpenditurePage : ContentPage
{
    private readonly UpSertExpenditureViewModel viewModel;

    public UpSertExpenditurePage(UpSertExpenditureViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.PageLoaded();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private void UnitPriceOrQty_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModel.UnitPriceOrQuantityChanged();
    }

    private void UnitPrice_Focused(object sender, FocusEventArgs e)
    {
        if (UnitPrice.Text == "0")
        {
            UnitPrice.Text = "";
        }
    }

    private void TaxCheckbox_CheckChanged(object sender, EventArgs e)
    {
        var s = sender as InputKit.Shared.Controls.CheckBox;
        var tax = (TaxModel)s.BindingContext;
        if (s.IsChecked)
        {
            tax.IsChecked = true;
            viewModel.AddTax(tax);
        }
        else
        {
            tax.IsChecked = false;

            viewModel.RemoveTax(tax);
        }
    }

    private void AddTax_CheckChanged(object sender, EventArgs e)
    {
    }
}