using Microsoft.Maui.Controls.Platform.Compatibility;
using Quanlychitieu.Helpers;
using Quanlychitieu.Navigation;

namespace Quanlychitieu.Handlers
{
    public partial class ShellItemHandler : ShellItemRenderer
    {
        INavigationService _navigationService => ServiceHelper.GetService<INavigationService>();

        public EventHandler MessUnreadUpdated;

        public ShellItemHandler(IShellContext shellContext)
            : base(shellContext)
        {
        }

        void PerformTabReselected()
        {
            var currentVM = App.CurrentViewModel;
            if (currentVM != null)
            {
                if (!PageAttachedProperties.GetIsDisableBackButton(AppShell.Current.CurrentPage) && !currentVM.IsBusy)
                {
                    _navigationService.PopToRootAsync();
                }
            }
        }
    }
}
