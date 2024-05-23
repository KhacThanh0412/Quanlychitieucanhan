using Quanlychitieu.ViewModels.Expenditures;
using Quanlychitieu.Models;
using Quanlychitieu.PopUpPages;

namespace Quanlychitieu.Views.Expenditures;

public partial class ManageExpenditures : UraniumContentPage
{
    private readonly ManageExpendituresViewModel viewModel;
    private readonly UpSertExpenditureViewModel upSertExpVM;
    private UpSertExpenditureBottomSheet UpSertExpbSheet;
    public ManageExpenditures(ManageExpendituresViewModel vm, UpSertExpenditureViewModel upSertExpVM)
    {
        InitializeComponent();
        viewModel = vm;
        this.upSertExpVM = upSertExpVM;
        BindingContext = vm;

        UpSertExpbSheet = new(upSertExpVM);
        Attachments.Add(UpSertExpbSheet);

    }

    protected override async void OnAppearing()
    {
        if (UpSertExpbSheet.IsPresented)
        {
            UpSertExpbSheet.IsPresented = false;
        }
        base.OnAppearing();
        
        await viewModel.PageloadedAsync();
        
    }

    private async void ExportToPDFImageButton_Clicked(object sender, EventArgs e)
    {
        if (viewModel.ExpendituresList?.Count < 1)
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Không thể lưu danh sách trống sang PDF"));
        }
        //else
        //{
        //    PrintProgressBarIndic.IsVisible = true;
        //    PrintProgressBarIndic.Progress = 0;
        //    await PrintProgressBarIndic.ProgressTo(1, 1000, easing: Easing.Linear);

        //    await viewModel.PrintExpendituresBtn();
        //    PrintProgressBarIndic.IsVisible = false;
        //}
    }

    private void AddExpBtn_Clicked(object sender, EventArgs e)
    {
        upSertExpVM.SingleExpenditureDetails = new()
        {
            DateSpent = DateTime.Now,
        };
        upSertExpVM.PageLoaded();
        UpSertExpbSheet.IsPresented = true;
    }

    private void EditExpIcon_Clicked(object sender, EventArgs e)
    {
        var imageBtn = sender as SwipeItem;
        var singleExp = (ExpendituresModel)imageBtn.BindingContext;

        upSertExpVM.SingleExpenditureDetails = singleExp; 
        upSertExpVM.PageLoaded();
        UpSertExpbSheet.IsPresented = true;
        
    }

}
