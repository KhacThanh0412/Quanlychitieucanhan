﻿using CommunityToolkit.Mvvm.Input;
using Quanlychitieu.AdditionalResourcefulAPIClasses;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Platforms.Android.NavigationsMethods;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;
using Quanlychitieu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ISettingsServiceRepository settingsRepo;
        private readonly IUsersRepository userRepo;
        private readonly CountryAndCurrencyCodes countryAndCurrency = new();

        readonly LoginNavs NavFunctions = new();
        public LoginViewModel(ISettingsServiceRepository sessionServiceRepository, IUsersRepository userRepository)
        {
            settingsRepo = sessionServiceRepository;
            userRepo = userRepository;
        }

        [ObservableProperty]
        public List<string> countryNamesList = [];

        [ObservableProperty]
        public string username;

        [ObservableProperty]
        public double pocketMoney;

        [ObservableProperty]
        public bool hasLoginRemembered = true;

        [ObservableProperty]
        public UsersModel currentUser;

        private string userCurrency;

        [ObservableProperty]
        private bool errorMessageVisible;

        private string userId;

        [ObservableProperty]
        private bool isLoginFormVisible;

        [ObservableProperty]
        private bool isRegisterFormVisible;

        [ObservableProperty]
        private bool isQuickLoginVisible;

        [ObservableProperty]
        private bool registerAccountOnline;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool showQuickLoginErrorMessage;

        [ObservableProperty]
        private bool isLoginOnlineButtonClicked;

        readonly string LoginDetectFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuickLogin.text");

        [RelayCommand]
        public async Task PageLoaded()
        {
            if (IsQuickLoginDetectionFilePresent())
            {
                if (!await userRepo.CheckIfAnyUserExists())
                {
                    File.Delete(LoginDetectFile);
                    await PageLoaded();
                }
                Username = await settingsRepo.GetPreference<string>("Username", null);
                if (Username is null)
                {
                    File.Delete(LoginDetectFile);
                    await PageLoaded();
                }
                IsQuickLoginVisible = true;
                userId = await settingsRepo.GetPreference<string>(nameof(CurrentUser.Id), null);
                userRepo.User = await userRepo.GetUserAsync(userId); 
            }
            else
            {
                CurrentUser = new();

                HasLoginRemembered = false;
                CountryNamesList = countryAndCurrency.GetCountryNames();

                if (await userRepo.CheckIfAnyUserExists())
                {
                    CurrentUser.Id = userId;
                    if (userId is null)
                    {
                        IsLoginFormVisible = true;
                        HasLoginRemembered = false;
                    }

                    IsLoginFormVisible = false;
                }
                else
                {
                    await settingsRepo.ClearPreferences();
                    HasLoginRemembered = false;
                }
            }
        }

        [ObservableProperty]
        string selectedCountry;

        [RelayCommand]
        public void CurrencyFromCountryPicked()
        {
            var dict = countryAndCurrency.LoadDictionaryWithCountryAndCurrency();
            CurrentUser.UserCountry = SelectedCountry;
            dict.TryGetValue(SelectedCountry, out userCurrency);
        }

        [RelayCommand]
        public async Task GoToHomePageFromRegister()
        {
            CurrentUser.Email = CurrentUser.Email.Trim();
            CurrentUser.Id = Guid.NewGuid().ToString();
            CurrentUser.UserCurrency = userCurrency;
            CurrentUser.PocketMoney = PocketMoney;
            CurrentUser.RememberLogin = true;
            if (await userRepo.AddUserAsync(CurrentUser))
            {
                await settingsRepo.SetPreference(nameof(CurrentUser.Id), CurrentUser.Id);
                await settingsRepo.SetPreference("Username", CurrentUser.Username);
                await settingsRepo.SetPreference(nameof(CurrentUser.UserCurrency), CurrentUser.UserCurrency);

                if (!File.Exists(LoginDetectFile))
                {
                    File.Create(LoginDetectFile).Close();
                }

                await Shell.Current.DisplayAlert("Đăng ký tài khoản", "Tài khoản đã được tạo", "Ok");
                await NavFunctions.GoToHomePage();

                IsQuickLoginVisible = false;
            }
            else
            {
            }
        }

        [RelayCommand]
        public async Task GoToHomePageFromLogin()
        {
            ErrorMessageVisible = false;
            IsBusy = true;
            UsersModel User = new();
            User = await userRepo.GetUserAsync(CurrentUser.Email.Trim(), CurrentUser.Password);

            if (User is null)
            {
                IsBusy = false;
                ErrorMessageVisible = true;
            }
            else
            {
                if (!File.Exists(LoginDetectFile))
                {
                    File.Create(LoginDetectFile).Close();
                }
                CurrentUser = User;
                userRepo.User = await userRepo.GetUserAsync(CurrentUser.Id); //initialized user to be used by the entire app
                await settingsRepo.SetPreference<string>(nameof(CurrentUser.Id), CurrentUser.Id);
                await settingsRepo.SetPreference<string>("Username", CurrentUser.Username);
                await settingsRepo.SetPreference<string>(nameof(CurrentUser.UserCurrency), CurrentUser.UserCurrency);

                await SyncAndNotifyAsync();
                IsBusy = false;

                IsQuickLoginVisible = true;
                await NavFunctions.GoToHomePage();
            }
        }

        public async Task QuickLogin()
        {

            if (File.Exists(LoginDetectFile))
            {
                IsQuickLoginVisible = false;
                userRepo.User = await userRepo.GetUserAsync(userId); //initialized user to be used by the entire app                                
                await NavFunctions.GoToHomePage();
            }
            else
            {
                ShowQuickLoginErrorMessage = true;
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
        bool IsQuickLoginDetectionFilePresent()
        {
            return File.Exists(LoginDetectFile);
        }

        void DeletedLoginDetectFile()
        {
            File.Delete(LoginDetectFile);
        }
    }
}
