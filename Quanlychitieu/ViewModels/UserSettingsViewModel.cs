using Quanlychitieu.Helpers;

namespace Quanlychitieu.ViewModels;

public partial class UserSettingsViewModel : BaseViewModel
{
    private readonly IExpendituresRepository expenditureRepo;
    private readonly ISettingsServiceRepository settingsService;
    private readonly IUsersRepository userRepo;
    private readonly IIncomeRepository incomeRepo;
    private readonly IDebtRepository debtRepo;

    [ObservableProperty]
    public double _totalIncomeAmount;
    [ObservableProperty]
    private UsersModel _activeUser;
    [ObservableProperty]
    private double _totalDebtRepaid;
    [ObservableProperty]
    private double _totalExpensesAmount;


    [ObservableProperty]
    bool isNotInEditingMode;

    public UserSettingsViewModel(IUsersRepository usersRepository, IIncomeRepository incomeRepo, IExpendituresRepository expendituresRepository, IDebtRepository debtRepository)
    {
        this.userRepo = usersRepository;
        this.incomeRepo = incomeRepo;
        expenditureRepo = expendituresRepository;
        debtRepo = debtRepository;
    }

    public async override Task LoadDataAsync()
    {
        ActiveUser = await userRepo.GetUserAsync();
        await Task.WhenAll(incomeRepo.SynchronizeIncomesAsync());
        await Task.WhenAll(expenditureRepo.SynchronizeExpendituresAsync());
        await Task.WhenAll(debtRepo.SynchronizeDebtsAsync());
        TotalIncomeAmount = incomeRepo.IncomesList.Sum(i => i.AmountReceived);
        TotalExpensesAmount = expenditureRepo.ExpendituresList.Sum(e => e.AmountSpent);
        CalculateTotalDebtRepaid();
        await base.LoadDataAsync();
    }

    private void CalculateTotalDebtRepaid()
    {
        // Tính tổng số tiền đã trả nợ
        TotalDebtRepaid = debtRepo.DebtList.Sum(d => d.AmountDebt);
    }

    [RelayCommand]
    public async Task LogOutUser()
    {
        if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn đăng xuất không?"));
        {
            await userRepo.LogOutUserAsync();
            await (Shell.Current as AppShell)?.RemoveRootAsync();
            App.Current.MainPage = new NavigationPage(new LoginPage(new LoginViewModel(settingsService, userRepo)));
        }
    }
}
