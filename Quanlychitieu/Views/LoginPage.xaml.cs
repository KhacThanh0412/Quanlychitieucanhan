using AndroidX.Lifecycle;
using Microsoft.Maui.Controls.Platform;
using Quanlychitieu.ViewModels;
using UraniumUI.Material.Controls;

namespace Quanlychitieu.Views;

public partial class LoginPage : ContentPage
{
	private readonly LoginViewModel viewModel;
    public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {
            if (view is Picker)
            {
                Android.Graphics.Drawables.GradientDrawable gd = new();
                gd.SetColor(Android.Graphics.Color.Transparent);

                handler.PlatformView.SetBackground(gd);
            }
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(0);
        // await viewModel.PageLoaded();
    }

    private async Task ShowRegisterForm()
    {
        LoginForm.IsVisible = false;
        SignUpUnFocused.IsVisible = false;
        RegisterForm.IsVisible = true;
        LoginUnFocused.IsVisible = true;
        SignUpFocused.IsVisible = true;
        LoginFocused.IsVisible = false;

        await Task.WhenAll(BorderFadeIn(LoginUnFocused), BorderFadeIn(SignUpFocused),
            VSLayoutFadeIn(RegisterForm),
            BorderFadeOut(SignUpUnFocused),
            BorderFadeOut(LoginFocused),
            VSLayoutFadeOut(LoginForm));
    }

    private async Task ShowLoginForm()
    {
        LoginForm.IsVisible = true;
        SignUpUnFocused.IsVisible = true;

        RegisterForm.IsVisible = false;
        LoginUnFocused.IsVisible = false;
        SignUpFocused.IsVisible = false;
        LoginFocused.IsVisible = true;

        _ = await Task.WhenAll(BorderFadeOut(LoginUnFocused), BorderFadeOut(SignUpFocused),
            VSLayoutFadeOut(RegisterForm),
            BorderFadeIn(LoginFocused),
            BorderFadeIn(SignUpUnFocused),
            VSLayoutFadeIn(LoginForm));
    }

    private void ToggleFormAndValidation(bool HasLoginRemembered, bool isLoginVisible)
    {
        if (HasLoginRemembered && !isLoginVisible)
        {
            LoginForm.IsVisible = false;
            RegisterForm.IsVisible = false;
            LoginSignUpTab.IsVisible = false;
        }
        else
        if (!HasLoginRemembered)
        {
            RegisterForm.IsVisible = false;
            LoginForm.IsVisible = true;
            LoginSignUpTab.IsVisible = true;
        }
    }

    private void SwitchToLoginPageTapped(object sender, TappedEventArgs e)
    {
        LoginSignUpTab.IsVisible = true;
        LoginForm.IsVisible = true;
        RegisterForm.IsVisible = false;
    }

    private async void LoginUnFocused_Tapped(object sender, TappedEventArgs e)
    {
        TextField s = new();
        Entry ss = new();
        Image sss = new();

        await ShowLoginForm();
    }

    private async void SignUpUnFocused_Tapped(object sender, TappedEventArgs e)
    {
        await ShowRegisterForm();
    }

    uint animationSpeed = 300;
    Easing animationIn = Easing.CubicIn;
    Easing animationOut = Easing.CubicOut;
    Task<bool> VSLayoutFadeOut(StackLayout Form)
    {
        return Form.FadeTo(0, animationSpeed, animationOut);
    }
    Task<bool> VSLayoutFadeIn(StackLayout Form)
    {
        return Form.FadeTo(1, animationSpeed, animationIn);
    }
    Task<bool> BorderFadeOut(Border border)
    {
        return border.FadeTo(0, animationSpeed, animationOut);
    }
    Task<bool> BorderFadeIn(Border border)
    {
        return border.FadeTo(1, animationSpeed, animationIn);
    }
}