using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Quanlychitieu.AdditionalResourcefulAPIClasses;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Platforms.Android.NavigationsMethods;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;
using Quanlychitieu.ViewModels;
using Quanlychitieu.Views;

namespace Quanlychitieu.ViewModels;

public partial class UserSettingsViewModel : BaseViewModel
{
    private readonly IUsersRepository usersRepository;
    private readonly IExpendituresRepository expendituresRepository;
    private readonly IIncomeRepository incomeRepository;
    private readonly IDebtRepository debtRepository;
    [ObservableProperty]
    public double pocketMoney;
    [ObservableProperty]
    public string userCurrency;
    [ObservableProperty]
    public double totalIncomeAmount;
    [ObservableProperty]
    public double totalExpendituresAmount;
    [ObservableProperty]
    public double totalBorrowedCompletedAmount;
    [ObservableProperty]
    public double totalBorrowedPendingAmount;
    [ObservableProperty]
    public double totalLentCompletedAmount;
    [ObservableProperty]
    public double totalLentPendingAmount;
    [ObservableProperty]
    public string selectCountryCurrency;
    [ObservableProperty]
    private UsersModel _activeUser;

    [ObservableProperty]
    bool isNotInEditingMode;

    public UserSettingsViewModel(IUsersRepository usersRepository)
    {
        this.usersRepository = usersRepository;
    }

    public async override Task LoadDataAsync()
    {
        var userJson = await SecureStorage.GetAsync("user");
        if (!string.IsNullOrEmpty(userJson))
        {
            ActiveUser = JsonConvert.DeserializeObject<UsersModel>(userJson);
        }

        // GetTotals();
        await base.LoadDataAsync();
    }

    private void GetTotals()
    {
        TotalExpendituresAmount = expendituresRepository.ExpendituresList.Select(x => x.AmountSpent).Sum();
        TotalIncomeAmount = ActiveUser.TotalIncomeAmount;

        var filteredAndSortedDebts = debtRepository.DebtList
                        .Where(x => !x.IsDeleted)
                        .Distinct()
                        .ToList();
        var BorrowedCompletedList = new ObservableCollection<DebtModel>(filteredAndSortedDebts
            .Where(x => x.DebtType == DebtType.Borrowed && x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime)); //tổng nợ đã được trả lại

        var LentCompletedList = new ObservableCollection<DebtModel>(filteredAndSortedDebts
            .Where(x => x.DebtType == DebtType.Lent && x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime));//Tổng nợ đã được trả hoàn toàn từ người dùng

        var BorrowedPendingList = new ObservableCollection<DebtModel>(filteredAndSortedDebts
            .Where(x => x.DebtType == DebtType.Borrowed && !x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime));//tổng nợ vẫn đang chờ trả

        var LentPendingList = new ObservableCollection<DebtModel>(filteredAndSortedDebts
            .Where(x => x.DebtType == DebtType.Lent && !x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime)); //tổng nợ vẫn đang chờ người dùng trả

        TotalBorrowedCompletedAmount = BorrowedCompletedList.Sum(x => x.Amount);
        TotalBorrowedPendingAmount = BorrowedPendingList.Sum(x => x.Amount);
        TotalLentCompletedAmount = LentCompletedList.Sum(x => x.Amount);
        TotalLentPendingAmount = LentPendingList.Sum(x => x.Amount);
    }

    [RelayCommand]
    public async Task LogOutUser()
    {
        bool response = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Bạn có muốn đăng xuất không?"));
        if (response)
        {
            string LoginDetectFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuickLogin.text");
            File.Delete(LoginDetectFile);

            await usersRepository.LogOutUserAsync();
            await expendituresRepository.LogOutUserAsync();
            await debtRepository.LogOutUserAsync();
        }
    }

    [RelayCommand]
    public async Task GoToEditUserSettingsPage()
    {
        await Shell.Current.GoToAsync(nameof(EditUserSettingsPage), true);
    }

    [RelayCommand]
    public async Task UpdateUserInformation()
    {
        bool dialogResult = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Lưu thông tin cá nhân?"));
        if (dialogResult)
        {
            ActiveUser.DateTimeOfPocketMoneyUpdate = DateTime.UtcNow;
            await expendituresRepository.GetAllExpendituresAsync();
            if (await usersRepository.UpdateUserAsync(ActiveUser))
            {
                usersRepository.User = ActiveUser;

                CancellationTokenSource cancellationTokenSource = new();
                const ToastDuration duration = ToastDuration.Short;
                const double fontSize = 16;
                const string text = "Hồ sơ cá nhân đã cập nhật!";
                var toast = Toast.Make(text, duration, fontSize);
                await toast.Show(cancellationTokenSource.Token);
                await Shell.Current.GoToAsync("..", true);
            }
        }
        IsNotInEditingMode = true;
    }
}
