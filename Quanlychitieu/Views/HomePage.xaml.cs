using Quanlychitieu.ViewModels.Expenditures;
using Quanlychitieu.ViewModels;
using Quanlychitieu.Views.Expenditures;

namespace Quanlychitieu.Views;

public partial class HomePage : ContentPage
{
    private UpSertExpenditureBottomSheet UpSertExpbSheet;
    public HomePage(HomeViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
        // Attachments.Add(UpSertExpbSheet);
    }

    protected override void OnAppearing()
    {
        //if (UpSertExpbSheet.IsPresented)
        //{
        //    UpSertExpbSheet.IsPresented = false;
        //}
        base.OnAppearing();
        //viewModel.GetUserData();
        //if (!viewModel._isInitialized)
        //{
        //    await viewModel.DisplayInfo();
        //    viewModel._isInitialized = true;
        //}
    }
    private void AddExpBtn_Clicked(object sender, EventArgs e)
    {
        UpSertExpbSheet.IsPresented = true;
        Shell.Current.IsEnabled = false;
    }
}