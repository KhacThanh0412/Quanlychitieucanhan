using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;
using Mopups.Pages;
using Quanlychitieu.Helpers;
using AView = Android.Views.View;
using View = Microsoft.Maui.Controls.View;
using Quanlychitieu.Handlers;
using Quanlychitieu.Views.Controls;
using Quanlychitieu.Navigation;
using Quanlychitieu.Services;

namespace Quanlychitieu.Behaviors
{
    public partial class TapToCloseKeyboardBehavior
    {
        private Dictionary<string, Dictionary<Type, List<View>>> _editTextAlive;
        private GestureDetector _gestureDetector;
        protected override void OnAttachedTo(View bindable, AView platformView)
        {
            if (_editTextAlive == null)
            {
                _editTextAlive = new();
            }

            var rootPageTitle = Shell.Current?.CurrentItem?.CurrentItem?.Title ?? string.Empty;
            var currentWindowType = GetCurrentWindow().GetType();
            if (_editTextAlive.ContainsKey(rootPageTitle))
            {
                var viewsByPage = _editTextAlive[rootPageTitle];
                if (!viewsByPage.ContainsKey(currentWindowType))
                {
                    viewsByPage.Add(currentWindowType, new List<View> { bindable });
                }
                else
                {
                    var views = viewsByPage[currentWindowType];
                    views.RemoveAll(v => v.Window == null);
                    if (!views.Contains(bindable))
                    {
                        views.Add(bindable);
                    }
                }
            }
            else
            {
                _editTextAlive.Add(rootPageTitle, new Dictionary<Type, List<View>>
                {
                    { currentWindowType, new List<View> { bindable } }
                });
            }

            base.OnAttachedTo(bindable, platformView);
            AddTouchListeners(platformView);
        }

        protected override void OnDetachedFrom(View bindable, AView platformView)
        {
            base.OnDetachedFrom(bindable, platformView);
            Type type;
            var currentPage = FindParentPage(bindable);
            if (currentPage != null)
            {
                type = currentPage.GetType();
            }
            else
            {
                type = FindParentPopup(bindable).GetType();
            }

            var rootPageTitle = Shell.Current?.CurrentItem?.CurrentItem?.Title ?? string.Empty;
            bool isPageAlive = Shell.Current.Navigation.NavigationStack.FirstOrDefault(p => p?.GetType() == type) != null;
            if (!isPageAlive && _editTextAlive.ContainsKey(rootPageTitle) && _editTextAlive[rootPageTitle].ContainsKey(type))
            {
                _editTextAlive[rootPageTitle][type].Clear();
                _editTextAlive[rootPageTitle].Remove(type);
                if (!_editTextAlive[rootPageTitle].Any())
                {
                    _editTextAlive.Remove(rootPageTitle);
                }

                ClearTouchListener(currentPage);
            }

            if (!_editTextAlive.Any())
            {
                _gestureDetector?.Dispose();
            }
        }

        private void OnDispatchTouch(object sender, MotionEvent e)
        {
            if (sender is null)
                return;
            if (e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Cancel)
            {
                var rootPageTitle = Shell.Current?.CurrentItem?.CurrentItem?.Title ?? string.Empty;
                var windowType = GetCurrentWindow().GetType();
                if (_editTextAlive.TryGetValue(rootPageTitle, out var viewsByPage) && viewsByPage.TryGetValue(windowType, out var editViews))
                {
                    foreach (var view in editViews)
                    {
                        if (view != null)
                        {
                            var platformView = view.Handler.PlatformView as AView;
                            if (Build.VERSION.SdkInt < BuildVersionCodes.P && e.Action == MotionEventActions.Down)
                            {
                                platformView.Focusable = true;
                                platformView.FocusableInTouchMode = true;
                            }

                            if (view is Editor && platformView.Parent != null)
                            {
                                // TODO: scroll inside control https://github.com/dotnet/maui/issues/10252
                                platformView.Parent.RequestDisallowInterceptTouchEvent(e.Action == MotionEventActions.Down);
                                break;
                            }
                        }
                    }
                }
            }

            AView currentFocus;
            if (sender is NotifyContentViewGroup)
            {
                currentFocus = Platform.CurrentActivity.CurrentFocus;
            }
            else
            {
                currentFocus = (sender as CustomDialog)?.CurrentFocus;
            }

            if (currentFocus != null)
            {
                _gestureDetector.OnTouchEvent(e);
            }
        }

        private void AddTouchListeners(object sender)
        {
            var currentPage = GetCurrentWindow();
            GestureListener gestureListener = null;
            if (currentPage != null)
            {
                if (currentPage is Page || currentPage is PopupPage)
                {
                    if ((currentPage as IView).Handler.PlatformView is NotifyContentViewGroup platformPageView)
                    {
                        platformPageView.DispatchTouch += OnDispatchTouch;
                        gestureListener = new GestureListener(currentPage);
                    }
                }
                else
                {
                    if ((currentPage as CustomCommunityPopup).Handler.PlatformView is CustomDialog platformPopupView)
                    {
                        platformPopupView.DispatchTouch += OnDispatchTouch;
                        gestureListener = new GestureListener(platformPopupView);
                    }
                }

                _gestureDetector = new GestureDetector((sender as AView).Context, gestureListener);
            }
        }

        private void ClearTouchListener(object currentPage = null)
        {
            currentPage ??= GetCurrentWindow();
            if (currentPage != null)
            {
                if (currentPage is Page || currentPage is PopupPage)
                {
                    if ((currentPage as IView).Handler.PlatformView is NotifyContentViewGroup platformPageView)
                    {
                        platformPageView.DispatchTouch -= OnDispatchTouch;
                    }
                }
                else
                {
                    if ((currentPage as CustomCommunityPopup).Handler.PlatformView is CustomDialog platformPopupView)
                    {
                        platformPopupView.DispatchTouch -= OnDispatchTouch;
                    }
                }
            }
        }

        private object GetCurrentWindow()
        {
            var currentPopup = ServiceHelper.GetService<INavigationCommunityPopupService>().GetPopupStack().LastOrDefault();
            if (currentPopup != null)
                return currentPopup;
            return AppShell.Current.CurrentPage;
        }

        private Page FindParentPage(Element view)
        {
            try
            {
                if (view is Page page)
                    return page;
                if (view.Parent != null)
                    return FindParentPage(view.Parent);
            }
            catch
            {
            }

            return null;
        }

        private Popup FindParentPopup(Element view)
        {
            try
            {
                if (view is CustomCommunityPopup popup)
                    return popup;
                if (view.Parent != null)
                    return FindParentPopup(view.Parent);
            }
            catch
            {
            }

            return null;
        }

        private class GestureListener : Java.Lang.Object, GestureDetector.IOnGestureListener
        {
            private bool _isPopup;
            private CustomDialog _platformPopupView;
            private NotifyContentViewGroup _pageView;
            public GestureListener(object currentWindow)
            {
                if (currentWindow is Page page)
                {
                    _pageView = page.Handler.PlatformView as NotifyContentViewGroup;
                    _isPopup = false;
                }
                else if (currentWindow is CustomDialog dialog)
                {
                    _platformPopupView = dialog;
                    _isPopup = true;
                }
            }

            public bool OnDown(MotionEvent e)
            {
                return false;
            }

            public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                return false;
            }

            public void OnLongPress(MotionEvent e)
            {
            }

            public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                return false;
            }

            public void OnShowPress(MotionEvent e)
            {
            }

            public bool OnSingleTapUp(MotionEvent e)
            {
                var currentFocus = GetCurrentFocus();
                if (currentFocus != null && IsOutsideThisEntryTouch(currentFocus, e.RawX, e.RawY))
                {
                    if (!IsAnotherEditViewsTouch(e.RawX, e.RawY))
                    {
                        if (_isPopup)
                        {
                            InputMethodManager imm = (InputMethodManager)global::Android.App.Application.Context.GetSystemService(Context.InputMethodService);
                            imm.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                            (currentFocus as EditText)?.ClearFocus();
                        }
                        else
                        {
                            ServiceHelper.GetService<IDeviceService>().HideKeyboard();
                        }
                    }

                    if (Build.VERSION.SdkInt < BuildVersionCodes.P)
                    {
                        var editTextsInView = _isPopup ? (Platform.CurrentActivity as MainActivity).EditTextsInView : ActivityExtensions.GetVisibleEditTexts(_pageView);
                        foreach (var item in editTextsInView)
                        {
                            item.Focusable = false;
                        }
                    }
                }

                return true;
            }

            private AView GetCurrentFocus()
                => _isPopup ? _platformPopupView?.CurrentFocus : Platform.CurrentActivity.CurrentFocus;
            private Rect GetAreaEditText(EditText editText)
            {
                int[] position = new int[2];
                editText.GetLocationOnScreen(position);
                return new Rect(position[0], position[1], editText.Width, editText.Height);
            }

            private Rect GetAreaClearSearchBar(AView clearButton)
            {
                int[] position = new int[2];
                clearButton.GetLocationOnScreen(position);
                return new Rect(position[0], position[1], clearButton.Width, clearButton.Height);
            }

            private bool IsAnotherEditViewsTouch(double xCoordinate, double yCoordinate)
            {
                bool isAnotherTap = false;
                var editTextsInView = _pageView == null ? (Platform.CurrentActivity as MainActivity).EditTextsInView : ActivityExtensions.GetVisibleEditTexts(_pageView);
                foreach (var item in editTextsInView)
                {
                    if (!item.Enabled)
                        continue;
                    var tapArea = GetAreaEditText(item);
                    if (tapArea.Contains(xCoordinate, yCoordinate) && item.Enabled)
                    {
                        isAnotherTap = true;
                        break;
                    }
                }

                return isAnotherTap;
            }

            private bool IsOutsideThisEntryTouch(AView currentFocus, double xCoordinate, double yCoordinate)
            {
                if (currentFocus is EditText editText)
                {
                    var editTextArea = GetAreaEditText(editText);
                    return !editTextArea.Contains(xCoordinate, yCoordinate);
                }

                return true;
            }
        }
    }

    internal class NotifyContentViewGroup : ContentViewGroup
    {
        public event EventHandler<MotionEvent> DispatchTouch;
        public NotifyContentViewGroup(Context context)
            : base(context)
        {
            SetClipChildren(false);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            var result = base.DispatchTouchEvent(e);
            DispatchTouch?.Invoke(this, e);
            return result;
        }
    }
}
