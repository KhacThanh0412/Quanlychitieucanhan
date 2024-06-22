namespace Quanlychitieu.Views;

public partial class AddExpendituresPage : ContentPage
{
	public AddExpendituresPage(AddExpendituresViewModel viewModel)
	{
        BindingContext = viewModel;
		InitializeComponent();
	}
}