

using Quanlychitieu.ViewModels.Debts;

namespace Quanlychitieu.Views.Debts;

public partial class UpSertInstallmentBSheet : BottomSheetView
{
    readonly UpSertDebtViewModel viewModel;
    public bool ShowDeleteBtn { get; set; }
    public UpSertInstallmentBSheet(UpSertDebtViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        this.BindingContext = vm;
    }
    protected override void OnOpened()
    {
        base.OnOpened();
        DeleteInstallmentBtn.IsVisible = ShowDeleteBtn;
    }
    private void AmountPaid_Focused(object sender, FocusEventArgs e)
    {
        if (AmountPaid.Text == "1")
        {
            AmountPaid.Text = "";
        }
    }
}