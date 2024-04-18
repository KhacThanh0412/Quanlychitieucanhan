using Quanlychitieu.ViewModels;

namespace Quanlychitieu.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}