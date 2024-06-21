using Quanlychitieu.ViewModels.Incomes;
using Quanlychitieu.Utilities;
using System.ComponentModel;
using UraniumUI.Material.Controls;

namespace Quanlychitieu.Views;

public partial class UpSertIncomePopUp : Popup
{
    readonly UpSertIncomeViewModel viewModel;
    public UpSertIncomePopUp(UpSertIncomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
        Size = new Size(230, 340);
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ClosePopUp) && viewModel.ClosePopUp)
        {
            if (viewModel.ThisPopUpResult == PopupResult.Cancel)
            {
                Close(new PopUpCloseResult() { Data = null, Result = PopupResult.Cancel });
                return;
            }
            Close(new PopUpCloseResult() { Data = viewModel.SingleIncomeDetails, Result = viewModel.ThisPopUpResult });
        }
    }

    private void AmountReceived_TextChanged(object sender, TextChangedEventArgs e)
    {
        var s = sender as TextField;
        if (s.Text?.Length == 0)
        {
            viewModel.SingleIncomeDetails.AmountReceived = 0;
        }
        viewModel.AmountReceivedChanged();
    }

    private void AmountReceived_Focused(object sender, FocusEventArgs e)
    {
        if (AmountReceived.Text == "0")
        {
            AmountReceived.Text = "";
        }
    }
}