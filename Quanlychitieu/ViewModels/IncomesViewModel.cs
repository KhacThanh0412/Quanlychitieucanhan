using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Helpers;
using Quanlychitieu.Models;
using Quanlychitieu.Navigation;
using Quanlychitieu.Views;

namespace Quanlychitieu.ViewModels;

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
    public void ShowFilterPopUpPage()
    {
    }
}
