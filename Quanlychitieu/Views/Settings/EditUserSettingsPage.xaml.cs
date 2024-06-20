using Quanlychitieu.ViewModels;

namespace Quanlychitieu.Views.Settings;

public partial class EditUserSettingsPage : ContentPage
{
    private UserSettingsViewModel viewmodel;
    public EditUserSettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private void CountryPicker_SelectedValueChanged(object sender, object e)
    {
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("Tapped");
    }
}