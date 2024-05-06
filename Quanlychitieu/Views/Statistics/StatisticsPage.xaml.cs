using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel;
using Quanlychitieu.ViewModels.Statistics;

namespace Quanlychitieu.Views.Statistics;

public partial class StatisticsPage
{
    private readonly StatisticsPageViewModel viewModel;
    public StatisticsPage(StatisticsPageViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.LoadState();

        if (!viewModel.IsLoaded)
        {
            viewModel.PageLoaded();
            YearPicker.SelectedItem = DateTime.Now.Year;//.ToString();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        viewModel.SaveState();
    }
    protected override bool OnBackButtonPressed()
    {
        Debug.WriteLine("Back button pressed");
        return base.OnBackButtonPressed();
    }
    private void BarChart_ChartPointPointerDown(IChartView chart, ChartPoint point)
    {
    }
}