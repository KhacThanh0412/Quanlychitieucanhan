using Quanlychitieu.Navigation;

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
    }
}
