using Android.Content;
using Android.Views;
using Android.Widget;
using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.Core.Views;
using Microsoft.Maui.Platform;
using Quanlychitieu.Helpers;
using Quanlychitieu.Navigation;
using Quanlychitieu.Views.Controls;

namespace Quanlychitieu.Handlers
{
    public class CustomDialog : MauiPopup
    {
        private OnGlobalLayoutListener _onGlobalLayoutListener;
        public event EventHandler<MotionEvent> DispatchTouch;
        public event EventHandler<MotionEvent> PopupDismissEvent;
        private bool _isDispose;
        public CustomDialog(Context context, IMauiContext mauiContext)
            : base(context, mauiContext)
        {
            this.Window?.SetSoftInputMode(SoftInput.AdjustResize);
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            bool dispatch = base.DispatchTouchEvent(ev);

            if (!_isDispose)
            {
                if (ev.Action == MotionEventActions.Down ||
                ev.Action == MotionEventActions.Move ||
                ev.Action == MotionEventActions.Up)
                {
                    // Search EditText Control
                    var editors = (Window.DecorView as ViewGroup).GetChildrenOfType<EditText>();

                    foreach (var editor in editors)
                    {
                        var tapArea = GetAreaEditText(editor);
                        bool isTapped = tapArea.Contains(ev.RawX, ev.RawY);
                        if (isTapped)
                        {
                            if (!editor.HasFocusable)
                            {
                                editor.Focusable = true;
                                editor.FocusableInTouchMode = true;
                                editor.RequestFocus();
                            }

                            // Touch event intercept
                            editor.Parent.RequestDisallowInterceptTouchEvent(true);
                        }
                    }
                }

                DispatchTouch?.Invoke(this, ev);
            }

            return dispatch;
        }

        private Rect GetAreaEditText(EditText editText)
        {
            int[] position = new int[2];
            editText.GetLocationOnScreen(position);
            return new Rect(position[0], position[1], editText.Width, editText.Height);
        }

        public override void OnBackPressed()
        {
            CustomCommunityPopupHandler.CommunityPopupView?.OnBackButtonPressed();
            ServiceHelper.GetService<INavigationCommunityPopupService>().PopPopup();
        }

        protected override void OnStart()
        {
            var widthRequest = CustomCommunityPopupHandler.CommunityPopupView.Content.Width;
            var heightRequest = CustomCommunityPopupHandler.CommunityPopupView.Content.Height;
            if (CommunityPopupAttachedProperties.GetFillScreenPercent(CustomCommunityPopupHandler.CommunityPopupView).Width > 0)
            {
                widthRequest = Constant.WidthOfDevice * CommunityPopupAttachedProperties.GetFillScreenPercent(CustomCommunityPopupHandler.CommunityPopupView).Width;
                heightRequest = Constant.HeightOfDevice * CommunityPopupAttachedProperties.GetFillScreenPercent(CustomCommunityPopupHandler.CommunityPopupView).Height;
            }

            if (widthRequest > 0 || heightRequest > 0)
                CustomCommunityPopupHandler.CommunityPopupView.Size = new Size(widthRequest, heightRequest);
            CustomCommunityPopupHandler.CommunityPopupView?.OnViewApearing();
            base.OnStart();
            _onGlobalLayoutListener = new OnGlobalLayoutListener(Window.DecorView);
            Window?.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(_onGlobalLayoutListener);
        }

        class OnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            private Android.Views.View _decorView;

            public OnGlobalLayoutListener(Android.Views.View decorView)
            {
                _decorView = decorView;
            }

            public void OnGlobalLayout()
            {
                var windowVisibleDisplayFrame = new Android.Graphics.Rect();
                _decorView.RootView.GetWindowVisibleDisplayFrame(windowVisibleDisplayFrame);
                int visibleDecorViewHeight = windowVisibleDisplayFrame.Height();
                int currentKeyboardHeight = _decorView.Height - windowVisibleDisplayFrame.Bottom;
                if (currentKeyboardHeight > 0)
                    ListenerManager.Instance.SentEvent(Constant.KeyListeners.KeyboardShow, currentKeyboardHeight);
                else
                    ListenerManager.Instance.SentEvent(Constant.KeyListeners.KeyboardHide, null);
            }
        }

        public override void Dismiss()
        {
            CustomCommunityPopupHandler.CommunityPopupView?.OnViewDisappearing();
            base.Dismiss();
        }

        protected override void Dispose(bool disposing)
        {
            _isDispose = disposing;
            base.Dispose(disposing);
            PopupDismissEvent?.Invoke(this, null);
            Window?.DecorView.ViewTreeObserver.RemoveOnGlobalLayoutListener(_onGlobalLayoutListener);
        }
    }

    public partial class CustomCommunityPopupHandler : PopupHandler
    {
        public static CustomCommunityPopup CommunityPopupView;
        protected override MauiPopup CreatePlatformElement()
        {
            try
            {
                _ = MauiContext ?? throw new InvalidOperationException("MauiContext is null, please check your MauiApplication.");
                _ = MauiContext.Context ?? throw new InvalidOperationException("Android Context is null, please check your MauiApplication.");
            }
            catch (Exception e)
            {
            }

            return new CustomDialog(MauiContext.Context, MauiContext);
        }

        protected override void ConnectHandler(MauiPopup platformView)
        {
            CommunityPopupView = VirtualView as CustomCommunityPopup;
            platformView.ShowEvent += PlatformView_ShowEvent;
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(MauiPopup platformView)
        {
            platformView.ShowEvent -= PlatformView_ShowEvent;
            base.DisconnectHandler(platformView);
        }

        private void PlatformView_ShowEvent(object sender, EventArgs e)
        {
            PlatformView.CurrentFocus?.ClearFocus();
            CommunityPopupView?.OnViewApeared();
        }
    }
}
