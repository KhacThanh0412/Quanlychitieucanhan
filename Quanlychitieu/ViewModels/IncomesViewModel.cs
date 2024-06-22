using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Helpers;
using Quanlychitieu.Models;
using Quanlychitieu.Navigation;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Views;

namespace Quanlychitieu.ViewModels.Incomes;

public partial class IncomesViewModel : BaseViewModel
{
    private readonly IIncomeRepository incomeService;
    private readonly IUsersRepository userService;

    public IncomesViewModel(IIncomeRepository incomeRepository, IUsersRepository usersRepository)
    {
        incomeService = incomeRepository;
        userService = usersRepository;
    }

    [ObservableProperty]
    ObservableCollection<IncomeModel> _incomesList;

    [ObservableProperty]
    double _totalAmount;

    [ObservableProperty]
    double _totalIncomes;

    [ObservableProperty]
    string _userCurrency;

    [ObservableProperty]
    double _userPocketMoney;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    string _incTitle;

    UsersModel ActiveUser;

    public async override Task LoadDataAsync()
    {
        await Task.WhenAll(incomeService.SynchronizeIncomesAsync());
        ActiveUser = await userService.GetUserAsync();
        TotalIncomes = incomeService.CalculateTotalIncome();
        IncomesList = incomeService.IncomesList;
        TotalAmount = incomeService.IncomesList.Count();
        base.LoadDataAsync();
    }

    public override Task InitAsync(object initData)
    {
        return base.InitAsync(initData);
    }

    [RelayCommand]
    async Task AddIncomeAsync()
    {
        if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn thêm mới hóa đơn không?"))
        {
            await NavigationService.PushToPageAsync<AddIncomePage>();
        }
    }

    [RelayCommand]
    async Task EditIncomeAsync(object obj)
    {
        var valueObject = obj as IncomeModel;
        if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn sửa thông tin hóa đơn không?"))
        {
            await NavigationService.PushToPageAsync<AddIncomePage>(valueObject);
        }
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
    public async Task DeleteIncomeBtn(IncomeModel income)
    {
        if(await AlertHelper.ShowConfirmationAlertAsync("Xác nhận xóa đơn?"))
        {
            if(await incomeService.DeleteIncomeAsync(income))
            {
                TotalIncomes = incomeService.CalculateTotalIncome();
                IncomesList = incomeService.IncomesList;
                TotalAmount = incomeService.IncomesList.Count();
            }
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
        }
    }

    [RelayCommand]
    public void ShowFilterPopUpPage()
    {
    }
}
