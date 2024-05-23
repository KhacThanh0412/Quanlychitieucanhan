using Quanlychitieu.ViewModels.Settings;

namespace Quanlychitieu.Views.Settings;

public partial class EditUserSettingsPage : ContentPage
{
    private UserSettingsViewModel viewmodel;
    public EditUserSettingsPage(UserSettingsViewModel vm)
    {
        InitializeComponent();
        viewmodel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewmodel.PageLoadedCommand.Execute(null);
        viewmodel.GetCountryNamesList();
        CountryPicker.SelectedItem = viewmodel.ActiveUser.UserCountry;
    }

    private void CountryPicker_SelectedValueChanged(object sender, object e)
    {
        var pickedCountry = CountryPicker.SelectedItem;
        if (pickedCountry is not null)
        {
            viewmodel.CurrencyFromCountryPickedCommand.Execute(pickedCountry.ToString());
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Debug.WriteLine("Tapped");
    }
}