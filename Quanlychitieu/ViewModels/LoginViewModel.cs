using Quanlychitieu.Helpers;
using Quanlychitieu.Navigation;

namespace Quanlychitieu.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ISettingsServiceRepository settingsRepo;
        private readonly IUsersRepository userRepo;
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

                ErrorMessageVisible = false;
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
    }
}
