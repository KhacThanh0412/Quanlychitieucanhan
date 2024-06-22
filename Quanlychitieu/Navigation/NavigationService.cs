using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Navigation
{
    public class NavigationService : INavigationService
    {
        public bool IsProcessing;
        protected INavigation Navigation
        {
            get
            {
                INavigation navigation = Shell.Current?.Navigation;
                if (navigation is not null)
                    return navigation;
                else
                    throw new Exception();
            }
        }

        public async Task PopPageAsync(bool isPopModal = false, bool animate = true)
        {
            if (IsProcessing)
                return;

            try
            {
                IsProcessing = true;
                if (Navigation.NavigationStack != null && Navigation.NavigationStack?.Count > 1 && !isPopModal)
                {
                    await Shell.Current.GoToAsync("..", animate: animate);
                }
                else if (Navigation.ModalStack != null && Navigation.ModalStack?.Count > 0 && isPopModal)
                {
                    await Navigation.PopModalAsync(animated: animate);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public async Task PopToRootAsync(bool animate = true)
        {
            if (IsProcessing)
                return;
            try
            {
                IsProcessing = true;
                if (Navigation.NavigationStack.Count > 0)
                {
                    await Navigation.PopToRootAsync(animated: animate);
                    await NavMethodExtendsion.ClearNavigationStackAsync();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public async Task PopToPageInStackAsync(Type targetPageType, bool isPopModal = false, bool animate = true)
        {
            if (IsProcessing)
                return;

            try
            {
                IsProcessing = true;

                var currentPageStack = Shell.Current.Navigation.NavigationStack;
                var count = currentPageStack.Count;
                if (count > 0)
                {
                    var targetPage = currentPageStack.FirstOrDefault(p => p?.GetType() == targetPageType);
                    if (targetPage is null)
                    {
                        await Navigation.PopToRootAsync(true);
                        await NavMethodExtendsion.ClearNavigationStackAsync();
                    }
                    else
                    {
                        var targetIndex = currentPageStack.ToList().IndexOf(targetPage);
                        foreach (var p in currentPageStack.ToList().GetRange(targetIndex + 1, currentPageStack.Count - targetIndex - 1))
                        {
                            Shell.Current.Navigation.RemovePage(p);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public async Task PushToPageAsync<T>(object param = null, bool isPushModal = false, bool animate = true)
            where T : ContentPage
        {
            if (IsProcessing)
                return;
            IsProcessing = true;
            var currentViewModel = App.CurrentViewModel;
            var currentPage = AppShell.Current.CurrentPage as ContentPage;
            var tapGestureRecognizer = new TapGestureRecognizer();
            // TODO:  https://github.com/dotnet/maui/issues/10252
            currentPage?.Content.GestureRecognizers.Add(tapGestureRecognizer);
            var page = NavMethodExtendsion.ResolvePage<T>();
            if (page is not null)
            {
                var toViewModel = NavMethodExtendsion.GetBaseViewModel(page);
                if (toViewModel is not null)
                {
                    var result = await NavMethodExtendsion.PushToNavigationStackAsync(page.ToString());
                    if (result)
                    {
                        Shell.SetBackButtonBehavior(page, new BackButtonBehavior { Command = toViewModel.BackButtonClickedCommand });
                        toViewModel.IsPushPageWithNavService = true;
                        await toViewModel.InitAsync(param);
                        page.Appearing += OnAppearingAsync;
                        page.Disappearing += OnDisapearingAsync;
                        if (isPushModal)
                            await Navigation.PushModalAsync(page, animated: animate);
                        else
                            await Navigation.PushAsync(page, animated: animate);
                    }
                }
            }

            // TODO:  https://github.com/dotnet/maui/issues/10252
            currentPage.Content.GestureRecognizers.Remove(tapGestureRecognizer);
            IsProcessing = false;
        }

        private async void OnAppearingAsync(object sender, EventArgs e)
        {
            var toViewModel = NavMethodExtendsion.GetBaseViewModel(sender as ContentPage);
            await toViewModel.ViewIsAppearingAsync();
        }

        private async void OnDisapearingAsync(object sender, EventArgs e)
        {
            bool isForwardNavigation = Navigation.NavigationStack.Count > 1
               && Navigation.NavigationStack[^2] == sender;

            if (sender is ContentPage thisPage)
            {
                if (!isForwardNavigation)
                {
                    var toViewModel = NavMethodExtendsion.GetBaseViewModel(sender as ContentPage);
                    if (!toViewModel.LoadDataOnAppearing)
                        thisPage.Appearing -= OnAppearingAsync;
                }

                await CallNavigatedFromAsync(thisPage);
            }
        }

        private async Task<Task> CallNavigatedFromAsync(ContentPage thisPage)
        {
            var fromViewModel = NavMethodExtendsion.GetBaseViewModel(thisPage);

            if (fromViewModel is not null)
            {
                await NavMethodExtendsion.PopToNavigationStackAsync(thisPage.ToString());
                if (Navigation == null || !Navigation.NavigationStack.Contains(thisPage))
                {
                    thisPage.NavigatedFrom -= OnDisapearingAsync;
                }

                return fromViewModel.ViewIsDestroyedAsync();
            }

            return Task.CompletedTask;
        }

        public int GetNumberStack()
            => Navigation.NavigationStack.Count;

        public async Task PushToWebViewAsync<T>(object param)
            where T : ContentPage
        {
            await this.PushToPageAsync<T>(param, isPushModal: true);
        }
    }
}
