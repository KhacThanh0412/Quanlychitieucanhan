using Google.Android.Material.Badge;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Handlers
{
    public partial class ShellHandler
    {
        ShellBottomNaviHandler _shellBottomNaviHandler;

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            _shellBottomNaviHandler = new ShellBottomNaviHandler(this, shellItem);
            return _shellBottomNaviHandler;
        }
    }

    public class ShellBottomNaviHandler : ShellBottomNavViewAppearanceTracker
    {
        const byte _messageTabIndex = 1;

        BottomNavigationView _bottomNaviView;
        BadgeDrawable _messBadge;
        int? _messTabId => _bottomNaviView?.Menu?.FindItem(_messageTabIndex)?.ItemId;

        public ShellBottomNaviHandler(IShellContext shellContext, ShellItem shellItem)
            : base(shellContext, shellItem)
        {
        }

        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            base.SetAppearance(bottomView, appearance);
            _bottomNaviView = bottomView;
        }
    }
}
