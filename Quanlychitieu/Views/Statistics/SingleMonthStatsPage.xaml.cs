using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel;
using Quanlychitieu.Models;
using Quanlychitieu.ViewModels.Statistics;

namespace Quanlychitieu.Views.Statistics;

public partial class SingleMonthStatsPage : ContentPage
{
    private readonly SingleMonthStatsPageViewModel viewModel;
    ExpendituresModel SpecificExp { get; set; } = new();
    public SingleMonthStatsPage(SingleMonthStatsPageViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.PageLoadedCommand.Execute(null);
        pieChart.Series = viewModel.PieSeries;
        LineChart.Series = viewModel.LineSeries;
        SelectedTitle.Text = "Biggest Flow Out Details";
    }
    ChartPoint obj;
    private void Chart_ChartPointPointerDown(IChartView chart, ChartPoint point)
    {
        if (point is not null)
        {
            obj = point;
            SelectedTitle.Text = "Selected Flow Out Details";
            var SelectedExpIndex = Convert.ToInt32(point.Coordinate.TertiaryValue);
            viewModel.ChangeSelectedExp(SelectedExpIndex);
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Debug.WriteLine($"Double Tapped {obj.Coordinate.PrimaryValue} {obj.Coordinate.SecondaryValue}");
    }
}