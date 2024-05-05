using Quanlychitieu.Navigation;
using Quanlychitieu.Platforms.Android.NavigationsMethods;

namespace Quanlychitieu
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            NavGraph.RegisterRoute();
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
    }
}
