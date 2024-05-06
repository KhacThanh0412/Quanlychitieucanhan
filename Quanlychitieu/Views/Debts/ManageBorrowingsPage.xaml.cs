
using Quanlychitieu.ViewModels.Debts;
using Quanlychitieu.Models;

namespace Quanlychitieu.Views.Debts;

public partial class ManageBorrowingsPage : UraniumContentPage
{
    readonly ManageDebtsViewModel viewModel;
    readonly UpSertDebtViewModel UpSertVM;
    UpSertDebtBottomSheet UpSertDebtbSheet;
    public ManageBorrowingsPage(ManageDebtsViewModel vm, UpSertDebtViewModel upSertDebtVM)
    {
        InitializeComponent();
        viewModel = vm;
        UpSertVM = upSertDebtVM;
    }
    protected override void OnAppearing()
    {
        //if (UpSertDebtbSheet.IsPresented)
        //{
        //    UpSertDebtbSheet.IsPresented = false;
        //}
        base.OnAppearing();
        viewModel.PageLoaded();
    }

    private void DebtsSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

        SearchBar searchBar = (SearchBar)sender;
        //DebtsSearchBar.ItemsSource = DataService.GetSearchResults(searchBar.Text);
        viewModel.SearchBarCommand.Execute(searchBar.Text);
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        this.BindingContext = viewModel;

        UpSertDebtbSheet = new(UpSertVM);
        this.Attachments.Add(UpSertDebtbSheet);
    }
    private void AddNewFlowHoldBtn_Clicked(object sender, EventArgs e)
    {
        UpSertVM.SingleDebtDetails = new DebtModel()
        {
            Amount = 1,
            PersonOrOrganization = new PersonOrOrganizationModel(),
            Currency = viewModel.UserCurrency,
            DebtType = DebtType.Borrowed
        };

        UpSertVM.PageLoaded();
        UpSertDebtbSheet.IsPresented = true;

    }

    private void EditDebtBtn_Clicked(object sender, EventArgs e)
    {
        var ss = (SwipeItem)sender;
        var selectedDebtItem = (DebtModel)ss.BindingContext;

        UpSertVM.SingleDebtDetails = selectedDebtItem;
        UpSertVM.PageLoaded();

        UpSertDebtbSheet.IsPresented = true;
    }
}
