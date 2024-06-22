namespace Quanlychitieu.Views;

public partial class AddSertDebtPage : ContentPage
{
	public AddSertDebtPage(AddSertDebtViewModel vm)
	{
        BindingContext = vm;
		InitializeComponent();
	}
}