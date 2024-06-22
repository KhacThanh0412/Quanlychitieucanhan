namespace Quanlychitieu.Views;

public partial class AddIncomePage : ContentPage
{
	public AddIncomePage(AddIncomeViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
	}
}