using Quanlychitieu.Helpers;
using Quanlychitieu.Navigation;

namespace Quanlychitieu.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ISettingsServiceRepository settingsRepo;
        private readonly IUsersRepository userRepo;
        readonly LoginNavs NavFunctions = new();
        public LoginViewModel(ISettingsServiceRepository sessionServiceRepository, IUsersRepository userRepository)
        {
            settingsRepo = sessionServiceRepository;
            userRepo = userRepository;
        }

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        public UsersModel _currentUser;

        [ObservableProperty]
        private bool errorMessageVisible;

        private string userId;

        [ObservableProperty]
        private bool isLoginFormVisible;

        [RelayCommand]
        public async Task PageLoaded()
        {
            if (await userRepo.CheckIfAnyUserExists())
            {
                if (userId is null)
                {
                    IsLoginFormVisible = true;
                }

                IsLoginFormVisible = false;
            }
            else
            {
                await settingsRepo.ClearPreferences();
            }
        }

        [RelayCommand]
        public async Task GoToHomePageFromRegister()
        {
            bool success = await userRepo.RegisterAsync(UserName, Email, Password);
            if (success)
            {
                await AlertHelper.ShowInformationAlertAsync("Bạn đã đăng ký thành công tài khoản mới");
            }
            else
            {
                await AlertHelper.ShowErrorAlertAsync("Email đã tồn tại vui lòng đăng ký email mới");
            }
        }

        [RelayCommand]
        public async Task GoToHomePageFromLogin()
        {
            ErrorMessageVisible = false;
            try
            {
                var user = await userRepo.LoginAsync(Email, Password);

                if (user == null)
                {
                    ErrorMessageVisible = true;
                    return;
                }

                CurrentUser = user;
                App.Current.MainPage = new AppShell();
                await NavigationService.PushToPageAsync<HomePage>(CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                ErrorMessageVisible = true;
            }
        }

        private async Task SyncAndNotifyAsync()
        {
            try
            {
                CancellationTokenSource cts = new();
                const ToastDuration duration = ToastDuration.Short;
                const double fontSize = 14;
                string text = "Đồng bộ !";
                var toast = Toast.Make(text, duration, fontSize);
                await toast.Show(cts.Token);
            }
            catch (AggregateException aEx)
            {
                foreach (var ex in aEx.InnerExceptions)
                {
                    await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Lỗi khi đồng bộ " + ex.Message));
                }
            }
        }

        //section for themes in windows version. i'll revise this later
        [ObservableProperty]
        int selectedTheme;
        [ObservableProperty]
        bool isLightTheme;

        public void SetThemeConfig()
        {
            SelectedTheme = AppThemesSettings.ThemeSettings.Theme;
            IsLightTheme = SelectedTheme == 0;
        }
        [RelayCommand]
        public void ThemeToggler()
        {
            SelectedTheme = AppThemesSettings.ThemeSettings.SwitchTheme();
            IsLightTheme = !IsLightTheme;
        }
    }
}
