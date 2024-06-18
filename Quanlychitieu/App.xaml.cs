using Quanlychitieu.Helpers;
using Quanlychitieu.Navigation;
using Quanlychitieu.Platforms.Android.NavigationsMethods;
using Quanlychitieu.Utilities;

namespace Quanlychitieu
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var dataAccess = new DataAccessRepo();
            var settingsRepo = new SettingsServiceRepository();
            var usersRepo = new UserRepository(dataAccess);
            MainPage = new NavigationPage(new LoginPage(new LoginViewModel(settingsRepo, usersRepo)));

            AppThemesSettings.ThemeSettings.SetTheme();
        }

        public INavigation GetNavigation()
        {
            if (Application.Current.MainPage is TabbedPage tabbed)
            {
                if (tabbed.CurrentPage is NavigationPage navigationPage)
                {
                    return navigationPage.Navigation;
                }
                else
                {
                    return tabbed.Navigation;
                }
            }
            else
            {
                return Application.Current.MainPage.Navigation;
            }
        }

        public static void HandleAppActions(AppAction action)
        {
            Current.Dispatcher.Dispatch(async () =>
            {
                switch (action.Id)
                {
                    case "add_flow_out":
                        await AppActionUtils.HomePageQuickAddFlowOut();
                        break;
                    case "add_flow_in":
                        await AppActionUtils.HomePageQuickAddFlowIn();
                        break;
                }

            });
        }

        public static BaseViewModel CurrentViewModel
        {
            get
            {
                if (Shell.Current != null && Shell.Current.CurrentPage != null)
                {
                    return Shell.Current.CurrentPage.BindingContext as BaseViewModel;
                }
                else
                {
                    return Application.Current.MainPage.BindingContext as BaseViewModel;
                }
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            window.MinimumHeight = 600;
            window.MinimumWidth = 800;
            window.Title = "Quanlychitieu";
            return window;
        }
    }
}
