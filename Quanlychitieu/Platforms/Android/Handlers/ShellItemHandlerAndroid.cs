using Android.Views;
using Quanlychitieu.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Handlers
{
    public partial class ShellItemHandler
    {
        bool _pendingSwitchTab;

        protected override void OnTabReselected(ShellSection shellSection)
        {
            base.OnTabReselected(shellSection);
            if ((App.CurrentViewModel?.IsBusy ?? false))
                return;
            PerformTabReselected();
        }

        protected override bool OnItemSelected(IMenuItem item)
        {
            if (_pendingSwitchTab || (App.CurrentViewModel?.IsBusy ?? false))
            {
                return false;
            }

            _pendingSwitchTab = true;
            Task.Delay(400).ContinueWith(t =>
            {
                _pendingSwitchTab = false;
            });
            return base.OnItemSelected(item);
        }

        protected override Task<bool> HandleFragmentUpdate(ShellNavigationSource navSource, ShellSection shellSection, Page page, bool animated)
        {
            if (navSource == ShellNavigationSource.Pop)
            {
                // Android blank screen issue if animation
                animated = false;
            }

            return base.HandleFragmentUpdate(navSource, shellSection, page, animated);
        }
    }
}
