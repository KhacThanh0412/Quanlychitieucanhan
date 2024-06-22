using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;

namespace Quanlychitieu.Helpers
{
    public static class BindableObjectExtensions
    {
        public static void InvokeViewAndViewModelAction<T>(this BindableObject view, Action<T> action)
            where T : class
        {
            if (view is T viewAsT)
            {
                action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                action(viewModelAsT);
            }
        }

        public static async Task InvokeViewAndViewModelActionAsync<T>(this BindableObject view, Func<T, Task> action)
            where T : class
        {
            if (view is T viewAsT)
            {
                await action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                await action(viewModelAsT);
            }
        }

        public static TViewModel GetViewModel<TViewModel>(this BindableObject element)
            where TViewModel : class
        {
            return element.BindingContext as TViewModel;
        }

        public static Page GetCurrentPage(this Page mainPage) =>
           _getCurrentPage(mainPage);

        private static Func<Page, Page> _getCurrentPage = mainPage =>
        {
            var page = mainPage;
            Page child = null;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            if (page is AppShell appShell)
                child = appShell.CurrentPage;
            else
                child = page;
            return child;
        };

        public static Page GetPage(this BindableObject binableObj)
        {
            if (binableObj is Page page)
                return page;

            if (binableObj is Element element)
                return GetPage(element.Parent);

            return null;
        }

        public static Popup GetRootPopup(this Element element)
        {
            try
            {
                if (element is Popup popup)
                    return popup;
                return GetRootPopup(element.Parent);
            }
            catch
            {
            }

            return null;
        }

        public static bool IsAlive(this Element element)
        {
            if (element == null)
                return false;
            if (element == App.Current)
                return true;
            return IsAlive(element.Parent);
        }

        public static BaseViewModel GetPageViewModel(this BindableObject binableObj)
        {
            if (binableObj == null)
                return null;

            if (binableObj.BindingContext is BaseViewModel viewmodel)
                return viewmodel;

            if (binableObj is Page)
                return null;

            if (binableObj is Element element)
                return GetPageViewModel(element.Parent);

            return null;
        }

        public static string GetPropertyBindingPath(this BindableObject obj, BindableProperty property)
        {
            string pathValue = null;
            if (obj != null)
            {
                var getContextMethod = typeof(BindableObject).GetRuntimeMethods().First(x => x.Name == "GetContext");
                object propertyContext = getContextMethod?.Invoke(obj, new object[] { property });

                var bindingList = propertyContext
                    .GetType()
                    .GetRuntimeField("Bindings")?
                    .GetValue(propertyContext) as System.Collections.IEnumerable;

                if (bindingList != null)
                {
                    var firstBinding = bindingList
                        .Cast<object>()
                        .FirstOrDefault();

                    if (firstBinding != null)
                    {
                        var binding = firstBinding.GetType().GetRuntimeProperty("Value").GetValue(firstBinding);
                        if (binding != null)
                        {
                            pathValue = binding.GetType().GetRuntimeProperty("Path").GetValue(binding) as string;
                        }
                    }
                }
            }

            return pathValue;
        }

        public static IObservable<TProperty> ObserveBindingContext<TProperty>(this BindableObject bindableObject)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(
                                handler => handler.Invoke,
                                h => bindableObject.BindingContextChanged += h,
                                h => bindableObject.BindingContextChanged -= h)
                    .Select(e =>
                    {
                        var propertyValue = bindableObject.BindingContext;
                        if (propertyValue is TProperty)
                        {
                            return (TProperty)propertyValue;
                        }

                        return default;
                    });
        }

        public static IObservable<TProperty> ObserveBindableProperty<TProperty>(this BindableObject bindableObject, BindableProperty bindableProperty)
        {
            if (bindableObject != null)
            {
                var propertyName = bindableProperty.PropertyName;

                return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => bindableObject.PropertyChanged += h,
                                h => bindableObject.PropertyChanged -= h)
                            .Where(e => e.EventArgs.PropertyName == propertyName)
                            .Select(e =>
                            {
                                var propertyValue = bindableObject.GetValue(bindableProperty);
                                if (propertyValue is TProperty)
                                {
                                    return (TProperty)propertyValue;
                                }

                                return default;
                            });
            }

            return Observable.Return(default(TProperty));
        }

        public static IObservable<TProperty> ObserveProperty<TProperty>(this object context, string propertyName)
        {
            if (context is INotifyPropertyChanged notifyObject)
            {
                return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                            handler => handler.Invoke,
                            h => notifyObject.PropertyChanged += h,
                            h => notifyObject.PropertyChanged -= h)
                        .Where(e => e.EventArgs.PropertyName == propertyName)
                        .Select(e =>
                        {
                            var propertyValue = context.GetPropertyValue(propertyName);
                            if (propertyValue is TProperty)
                            {
                                return (TProperty)propertyValue;
                            }

                            return default;
                        });
            }

            return Observable.Return(default(TProperty));
        }
    }
}
