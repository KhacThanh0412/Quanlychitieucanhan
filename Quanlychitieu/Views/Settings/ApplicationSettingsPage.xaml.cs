using Microsoft.Maui.Controls;
using Quanlychitieu.ViewModels;

namespace Quanlychitieu.Views.Settings;

public partial class ApplicationSettingsPage : ContentPage
{
    UserSettingsViewModel viewModel;
    public ApplicationSettingsPage()
    {
        InitializeComponent();
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var uri = new Uri("https://github.com/YBTopaz8/FlowHub-MAUI");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        await Clipboard.SetTextAsync(uri.ToString());
    }
}
