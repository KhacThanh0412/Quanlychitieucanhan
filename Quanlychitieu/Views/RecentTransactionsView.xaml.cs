using Quanlychitieu.ViewModels;

namespace Quanlychitieu.Views;

public partial class RecentTransactionsView : ContentPage
{
    public RecentTransactionsView(RecentTransactionsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}