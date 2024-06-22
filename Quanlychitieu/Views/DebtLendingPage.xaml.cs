namespace Quanlychitieu.Views;

public partial class DebtLendingPage : ContentPage
{
    readonly DebtLendingViewModel _vm;
	public DebtLendingPage(DebtLendingViewModel vm)
	{
        BindingContext = _vm = vm;
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}