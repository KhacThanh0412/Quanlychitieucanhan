using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Views;

namespace Quanlychitieu.ViewModels.Incomes;

public partial class ManageIncomesViewModel : ObservableObject
{
    private readonly IIncomeRepository incomeService;
    private readonly IUsersRepository userService;

    public ManageIncomesViewModel(IIncomeRepository incomeRepository, IUsersRepository usersRepository)
    {
        incomeService = incomeRepository;
        userService = usersRepository;
        incomeRepository.IncomesListChanged += HandleIncomesListUpdated;
        usersRepository.UserDataChanged += HandleUserUpdated;
    }

    private void HandleUserUpdated()
    {
        ActiveUser = userService.User;
        UserPocketMoney = ActiveUser.PocketMoney;
        UserCurrency = ActiveUser.UserCurrency;
    }

    [ObservableProperty]
    ObservableCollection<IncomeModel> incomesList;

    [ObservableProperty]
    double totalAmount;

    [ObservableProperty]
    int totalIncomes;

    [ObservableProperty]
    string userCurrency;

    [ObservableProperty]
    double userPocketMoney;

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    string incTitle;

    UsersModel ActiveUser = new();

    [RelayCommand]
    public void PageLoaded()
    {
        var user = userService.User;
        ActiveUser = user;
        UserPocketMoney = ActiveUser.PocketMoney;
        UserCurrency = ActiveUser.UserCurrency;
        FilterGetAllIncomes();
    }
    bool IsLoaded;
    public void FilterGetAllIncomes()
    {
        try
        {
            if (!IsLoaded)
            {
                IsBusy = true;
                IncTitle = "Tất cả thu nhập";
                ApplyChanges();

                IsLoaded = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");
        }
    }

    private async void HandleIncomesListUpdated()
    {
        try
        {
            ApplyChanges();
        }
        catch (Exception ex)
        {
           await Shell.Current.DisplayAlert("Lỗi", ex.Message, "OK");
        }
    }

    private void ApplyChanges()
    {
        var IncList = incomeService.IncomesList
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.DateReceived)
                    .ToList();
        IncomesList = new ObservableCollection<IncomeModel>(IncList);
        OnPropertyChanged(nameof(IncomesList));

        TotalAmount = IncList.AsParallel().Sum(x => x.AmountReceived);
        TotalIncomes = IncList.Count;
    }

    [RelayCommand]
    public async Task ShowAddIncomePopUp()
    {
        if (ActiveUser is null)
        {
            await Shell.Current.DisplayAlert("Đợi", "Vui lòng đợi", "OK");
        }
        else
        {
            var newIncome = new IncomeModel() { DateReceived = DateTime.Now };
            const string PageTitle = "Thêm mới";
            const bool isAdd = true;

            await AddEditIncome(newIncome, PageTitle, isAdd);
        }
    }

    private async Task AddEditIncome(IncomeModel newIncome, string pageTitle, bool isAdd)
    {
        var newUpserIncomeVM = new UpSertIncomeViewModel(incomeService, userService, newIncome, pageTitle, isAdd, ActiveUser);
        await Shell.Current.ShowPopupAsync(new UpSertIncomePopUp(newUpserIncomeVM));
    }

    [RelayCommand]
    public async Task ShowEditIncomePopUp(IncomeModel income)
    {
        await AddEditIncome(income, "Sửa đơn", false);
    }

    [RelayCommand]
    public async Task DeleteIncomeBtn(IncomeModel income)
    {
        CancellationTokenSource cancellationTokenSource = new();
        const ToastDuration duration = ToastDuration.Short;
        const double fontSize = 14;
        const string text = "Xóa đơn";
        var toast = Toast.Make(text, duration, fontSize);

        bool response = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Xác nhận xóa?"));
        if (response)
        {
            var updateDateTime = DateTime.UtcNow;
            income.UpdatedDateTime = updateDateTime;
            var deleteResponse = await incomeService.DeleteIncomeAsync(income);
            if (deleteResponse)
            {
                ActiveUser.TotalIncomeAmount -= income.AmountReceived;
                ActiveUser.PocketMoney -= income.AmountReceived;
                UserPocketMoney -= income.AmountReceived;
                ActiveUser.DateTimeOfPocketMoneyUpdate = updateDateTime;

                await userService.UpdateUserAsync(ActiveUser);
                IncomesList.Remove(income);

                await toast.Show(cancellationTokenSource.Token);
            }
        }
    }

    [RelayCommand]
    public async Task PrintIncomesBtn()
    {
        if (IncomesList?.Count < 1)
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Cannot Save an Empty List to PDF"));
        }
        else
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Saved"));
        }
    }

    [RelayCommand]
    public async Task ResetUserPocketMoney(double amount)
    {
        if (amount != 0)
        {
            ActiveUser.PocketMoney = amount;
            ActiveUser.DateTimeOfPocketMoneyUpdate = DateTime.UtcNow;
            userService.User = ActiveUser;
            await userService.UpdateUserAsync(ActiveUser);

            CancellationTokenSource cancellationTokenSource = new();
            const ToastDuration duration = ToastDuration.Short;
            const double fontSize = 16;
            const string text = "Đã cập nhật số dư!";
            var toast = Toast.Make(text, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token); //toast a notification about exp deletion

            PageLoaded();
        }
    }

    [RelayCommand]
    public void ShowFilterPopUpPage()
    {
    }
}
