using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;

namespace Quanlychitieu.Handlers
{
    public class CustomEntryHandler : EntryHandler
    {
        protected override void ConnectHandler(AppCompatEditText platformView)
        {
            PlatformView.BackgroundTintList = global::Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            base.ConnectHandler(platformView);
        }
    }
}
