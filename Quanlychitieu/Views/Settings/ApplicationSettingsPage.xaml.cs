using Microsoft.Maui.Controls;
using Quanlychitieu.ViewModels.Settings;

namespace Quanlychitieu.Views.Settings;

public partial class ApplicationSettingsPage : ContentPage
{
    UserSettingsViewModel viewModel;
    public ApplicationSettingsPage(UserSettingsViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = vm;
        // viewModel.SetThemeConfig();
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var uri = new Uri("https://github.com/YBTopaz8/FlowHub-MAUI");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        await Clipboard.SetTextAsync(uri.ToString());
    }
}
