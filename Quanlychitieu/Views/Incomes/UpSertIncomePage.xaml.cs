using Quanlychitieu.ViewModels.Incomes;

namespace Quanlychitieu.Views.Incomes;

public partial class UpSertIncomePage : ContentPage
{
    private readonly UpSertIncomeViewModel viewModel;
    public UpSertIncomePage(UpSertIncomeViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.PageLoadedCommand.Execute(null);
    }

    private void SaveIncBtn_Clicked(object sender, EventArgs e)
    {
        viewModel.UpSertIncomeCommand.Execute(null);
    }
}