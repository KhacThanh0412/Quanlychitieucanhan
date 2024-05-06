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

            MainPage = new AppShell();

            AppThemesSettings.ThemeSettings.SetTheme();
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
