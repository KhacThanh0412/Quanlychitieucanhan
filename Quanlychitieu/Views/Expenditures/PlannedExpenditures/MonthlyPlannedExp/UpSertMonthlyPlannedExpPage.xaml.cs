using Quanlychitieu.ViewModels.Expenditures.PlannedExpenditures.MonthlyPlannedExp;

namespace Quanlychitieu.Views.Expenditures.PlannedExpenditures.MonthlyPlannedExp;

public partial class UpSertMonthlyPlannedExpPage : ContentPage
{
    private readonly UpSertMonthlyPlannedExpViewModel viewModel;
    public UpSertMonthlyPlannedExpPage(UpSertMonthlyPlannedExpViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
        //	Comments.Text = "";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.PageLoadedCommand.Execute(null);
        //	CommentCheck.IsChecked = viewModel.HasComment;
    }

    private void CommentCheck_CheckChanged(object sender, EventArgs e)
    {
        //if (!CommentCheck.IsChecked)
        //{
        //	viewModel.SingleExpenditureDetails.Comment = "None";
        //}
    }
}