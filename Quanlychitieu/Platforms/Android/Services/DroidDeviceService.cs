using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using CommunityToolkit.Maui.Core.Platform;
using Java.Util;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Application = Android.App.Application;
using Environment = System.Environment;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace Quanlychitieu.Services
{
    public class DroidDeviceService : IDeviceService
    {
        public string Model => DeviceInfo.Model;
        public static TaskCompletionSource<bool> RequestPermissionCompletionSource { get; set; }
        public void HideKeyboard()
        {
            var context = Android.App.Application.Context;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null)
            {
                var activity = Platform.CurrentActivity;
                activity.Window.DecorView.ClearFocus();
                var isKeyboardShow = IsSoftKeyboardVisible(activity.Window.DecorView.RootView);
                if (isKeyboardShow)
                    inputMethodManager.HideSoftInputFromWindow(activity.Window.DecorView.RootView.WindowToken, HideSoftInputFlags.None);
            }
        }

        public bool IsSoftKeyboardVisible(Android.Views.View view)
        {
            if (view != null)
            {
                var insets = ViewCompat.GetRootWindowInsets(view);
                if (insets == null)
                    return false;
                var result = insets.IsVisible(WindowInsetsCompat.Type.Ime());
                return result;
            }

            return false;
        }
    }
}
