using Quanlychitieu.ViewModels.Debts;
using Quanlychitieu.Models;

namespace Quanlychitieu.Views.Debts;

public partial class SingleDebtDetailsPage : UraniumContentPage
{
    private readonly ManageDebtsViewModel viewModel;
    private readonly UpSertDebtViewModel UpSertVM;
    private UpSertDebtBottomSheet UpSertDebtbSheet;
    private UpSertInstallmentBSheet UpSertInstallmentBSheet;
    public SingleDebtDetailsPage(ManageDebtsViewModel vm, UpSertDebtViewModel upSertDebtVm)
	{
		InitializeComponent();
        this.viewModel = vm;
        BindingContext = vm;

        UpSertVM = upSertDebtVm;

        UpSertDebtbSheet = new(upSertDebtVm)
        {
            BindingContext = upSertDebtVm
        };

        UpSertInstallmentBSheet = new(upSertDebtVm)
        {
            BindingContext = upSertDebtVm
        };

        this.Attachments.Add(UpSertDebtbSheet);
        this.Attachments.Add(UpSertInstallmentBSheet);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.PageLoaded();
        viewModel.RefreshTitleText();
    }

    private void EditFlowHoldBtn_Clicked(object sender, EventArgs e)
    {
        UpSertVM.SingleDebtDetails = viewModel.SingleDebtDetails;
        UpSertVM.IsLent = viewModel.SingleDebtDetails.DebtType.Equals(DebtType.Lent);
        UpSertVM.IsBorrow = viewModel.SingleDebtDetails.DebtType.Equals(DebtType.Borrowed);

        UpSertVM.PageLoaded();

        UpSertDebtbSheet.IsPresented = true;
    }
    
    private void UpSertInstallmentTapGR_Tapped(object sender, TappedEventArgs e)
    {
        UpSertVM.SingleDebtDetails = viewModel.SingleDebtDetails;
        
        InstallmentPayments selectedInstallment = null;
        if (sender is FlexLayout)
        {
            var se = sender as FlexLayout;
            selectedInstallment = se.BindingContext as InstallmentPayments;
        }
        else
        {
            var se = sender as Label;
            selectedInstallment = se.Parent.BindingContext as InstallmentPayments;
        }
        UpSertVM.SingleInstallmentPayment = selectedInstallment is null ? new() { AmountPaid = 0, DatePaid = DateTime.Now } : selectedInstallment;
        UpSertVM.selectedInstallmentInitialAmount = selectedInstallment is null ? 0 : selectedInstallment.AmountPaid;
        UpSertInstallmentBSheet.ShowDeleteBtn = true;
        UpSertVM.IsUpSertInstallmentBSheetPresent= true;


    }

    private void AddInstallmentBtn_Clicked(object sender, EventArgs e)
    {
        UpSertVM.SingleDebtDetails = viewModel.SingleDebtDetails;
        UpSertVM.SingleInstallmentPayment = new InstallmentPayments() { AmountPaid = 0 , DatePaid = DateTime.Now };
        UpSertInstallmentBSheet.ShowDeleteBtn = false;
        UpSertVM.IsUpSertInstallmentBSheetPresent = true;
    }
}