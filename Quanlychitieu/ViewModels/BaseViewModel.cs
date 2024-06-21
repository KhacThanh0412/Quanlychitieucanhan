using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Quanlychitieu.Helpers;
using Quanlychitieu.Navigation;
using Quanlychitieu.Services;

namespace Quanlychitieu.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool _isServiceStop;
        [ObservableProperty]
        private bool _isShowMaintenanceBanner;
        [ObservableProperty]
        private bool _isCustomTitle;
        [ObservableProperty]
        private bool _isShowButtonSubizChat;
        [ObservableProperty]
        private bool _isRefreshing;
        [ObservableProperty]
        private bool _isEnableScroll = true;
        public ViewModelBusyManager BusyManager { get; }
        public IDataService DataService { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        protected INavigationCommunityPopupService NavigationCommunityPopupService { get; private set; }
        private object _initData;
        public virtual object AppearingTreasureData => _initData;
        public virtual bool ShouldRotate => false;
        public bool BlockBackButton { get; set; }

        public CompositeDisposable Disposables { get; } = new CompositeDisposable();
        private readonly ISubject<Unit> _whenContainerReleased = new Subject<Unit>();

        public virtual Task InitializeAsync()
        {
            byte count = 1;
            while (Shell.Current?.CurrentPage?.Parent == null && count < 21)
            {
                MainThread.BeginInvokeOnMainThread(async () => await Task.Delay(count * 100));
                count++;
            }

            if (Shell.Current?.CurrentPage?.Parent is Page page)
            {
                page.ObserveProperty<object>("Renderer")
                    .Subscribe(async _ =>
                    {
                        await Task.Delay(1);
                        if (!page.IsAlive())
                        {
                            _whenContainerReleased.OnNext(Unit.Default);
                        }
                    }).DisposeWith(Disposables);
            }

            Initialized = true;
            return Task.CompletedTask;
        }

        public virtual bool ShouldLoadData { get; set; } = true;
        public Page CurrentPage => Shell.Current?.CurrentPage;

        public virtual Task LoadDataAsync()
        {
            return Task.FromResult(true);
        }

        public bool Initialized;
        public readonly bool LoadDataOnAppearing;
        public bool LoadDataOnlyOne;
        public bool IsPushPageWithNavService;
        public bool IsNavigationInProgress
        {
            get
            {
                var currentShell = AppShell.Current as AppShell;
                return IsBusy
                    || BackButtonClickedCommand.IsRunning
                    || (NavigationService as NavigationService).IsProcessing;
            }
        }

        public BaseViewModel( bool loadDataOnAppearing = true, bool appearingTreasure = true, bool isCustomTitle = false, bool isLoadingIconAppearing = true, bool blockBackButton = false)
        {
            // NOTE: The appearingTreasure parameter is now obsolete.
            // DataService = dataService;
            NavigationService = ServiceHelper.GetService<INavigationService>();
            NavigationCommunityPopupService = ServiceHelper.GetService<INavigationCommunityPopupService>();
            LoadDataOnAppearing = loadDataOnAppearing;
            IsCustomTitle = isCustomTitle;
            BusyManager = new ViewModelBusyManager((busy) => IsBusy = busy, isLoadingIconAppearing);
            BlockBackButton = blockBackButton;
        }

        public virtual Task InitAsync(object initData)
        {
            _initData = initData;
            return Task.CompletedTask;
        }

        private bool _isDisappeared;
        public virtual Task ViewIsDestroyedAsync()
        {
            var currentActivity = Platform.CurrentActivity;
            var currentFocus = currentActivity?.Window?.CurrentFocus;

            _isDisappeared = true;

            return Task.CompletedTask;
        }

        public virtual Task ViewIsRemovedAsync()
        {
            if (!_isDisappeared)
            {
                ViewIsDestroyedAsync();
            }

            return Task.CompletedTask;
        }

        public async virtual Task ViewIsAppearingAsync()
        {
            IsBusy = true;
            var initialized = Initialized;
            if (!initialized)
            {
                await this.InitializeAsync();
                Initialized = true;
            }

            if (ShouldLoadData && (LoadDataOnAppearing || !initialized))
            {
                //if (initialized && DataService.UpdateMasterTask != null && !DataService.UpdateMasterTask.Task.IsCompleted)
                //    await DataService.UpdateMasterTask.Task;
                //if (initialized && DataService.LoadingDealerTask != null && !DataService.LoadingDealerTask.Task.IsCompleted)
                //    await DataService.LoadingDealerTask.Task;
                await MainThread.InvokeOnMainThreadAsync(LoadDataAsync);
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task BackButtonClickedAsync(Action action = null)
        {
            if (IsNavigationInProgress)
            {
                return;
            }

            IsBusy = true;
            if (action is not null)
            {
                action.Invoke();
            }

            await NavigationService.PopPageAsync();
            await Task.Delay(100).ContinueWith(t =>
            {
                IsBusy = false;
            }).ConfigureAwait(false);
        }
    }

    public class ViewModelBusyManager
    {
        private int _requestCount;
        private bool _active = true;
        public bool Active
        {
            get => _active;

            set
            {
                if (!value)
                {
                    _active = value;
                    _requestCount = 0;
                    _whenRequestCountChanged.OnNext(0);
                }

                _active = value;
            }
        }

        private readonly ISubject<int> _whenRequestCountChanged = new Subject<int>();

        public ViewModelBusyManager(Action<bool> setBusyFunc, bool isLoadingIconAppearing)
        {
            Active = isLoadingIconAppearing;
            _whenRequestCountChanged
                .Select(count => count > 0)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(busy =>
                {
                    setBusyFunc(busy);
                });
        }

        public void IncreaseRequest()
        {
            if (Active)
                _whenRequestCountChanged.OnNext(++_requestCount);
        }

        public void DecreaseRequest()
        {
            if (_requestCount > 0 && Active)
                _whenRequestCountChanged.OnNext(--_requestCount);
        }
    }
}
