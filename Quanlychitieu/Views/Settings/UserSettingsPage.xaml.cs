using Quanlychitieu.ViewModels.Settings;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.Views.Settings;

public partial class UserSettingsPage
{
    private readonly UserSettingsViewModel viewModel;
    public UserSettingsPage(UserSettingsViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.PageLoadedCommand.Execute(null);

        var theme = AppThemesSettings.ThemeSettings.Theme;
        switch (theme)
        {
            case 0:
               // LightThemeToggler.IsEnabled = true;
            //    DarkThemeToggler.IsEnabled = false;
                break;
            case 1:
            //    LightThemeToggler.IsEnabled = false;
              //  DarkThemeToggler.IsEnabled = true;
                break;
        }
    }
}