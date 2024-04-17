using Quanlychitieu.ViewModels;

namespace Quanlychitieu.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = this;
	}

    private async void TapGestureRecognizer_TappedAsync(object sender, TappedEventArgs e)
    {
       await Shell.Current.GoToAsync(nameof(RecentTransactionsView), true);
    }
}