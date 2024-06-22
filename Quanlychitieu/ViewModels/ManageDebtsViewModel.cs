using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.ViewModels;

public partial class ManageDebtsViewModel : BaseViewModel
{
    private readonly IDebtRepository debtRepo;
    private readonly IUsersRepository usersRepo;

    [ObservableProperty]
    ObservableCollection<DebtModel> _debtsList;
    [ObservableProperty]
    int totalDebts;
    [ObservableProperty]
    int totalLentCount;
    [ObservableProperty]
    double totalLentCompletedAmount;
    [ObservableProperty]
    double totalLentPendingAmount;
    [ObservableProperty]
    int totalBorrowedCount;
    [ObservableProperty]
    double totalBorrowedCompletedAmount;
    [ObservableProperty]
    double totalBorrowedPendingAmount;
    [ObservableProperty]
    UsersModel _activeUser;
    [ObservableProperty]
    DebtModel singleDebtDetails;
    bool IsLoaded;
    [ObservableProperty]
    string userCurrency;
    [ObservableProperty]
    bool isShowCompletedChecked;
    [ObservableProperty]
    bool isShowPendingChecked;
    [ObservableProperty]
    bool isShowBorrowedChecked;
    [ObservableProperty]
    bool isShowLentChecked;
    [ObservableProperty]
    int totalPendingBorrowCount;
    [ObservableProperty]
    int totalPendingLentCount;
    [ObservableProperty]
    int totalCompletedBorrowCount;
    [ObservableProperty]
    int totalCompletedLentCount;
    [ObservableProperty]
    string titleText;
    [ObservableProperty]
    bool showSingleSebt;
    public ManageDebtsViewModel(IDebtRepository debtRepository, IUsersRepository usersRepository)
    {
        debtRepo = debtRepository;
        usersRepo = usersRepository;
    }

    public override async Task LoadDataAsync()
    {
        ActiveUser = await usersRepo.GetUserAsync();
        DebtsList = await debtRepo.GetAllDebtAsync();
        if (DebtsList == null || DebtsList.Count == 0)
        { 
            return;
        }

        ApplyChanges();
        base.LoadDataAsync();
    }

    [RelayCommand]
    async Task GoToManageLendingsPage()
    {
        await NavigationService.PushToPageAsync<DebtLendingPage>(DebtsList);
    }

    [RelayCommand]
    async Task AddNewDebt()
    {
        await NavigationService.PushToPageAsync<AddSertDebtPage>();
    }

    public void RefreshTitleText()
    {
        TitleText = SingleDebtDetails.DebtType == DebtType.Lent
                    ? $"{SingleDebtDetails.PersonOrOrganization.Name} nợ bạn {SingleDebtDetails.AmountDebt} {SingleDebtDetails.Currency}"
                    : $"Bạn nợ {SingleDebtDetails.PersonOrOrganization.Name}, {SingleDebtDetails.AmountDebt} {SingleDebtDetails.Currency}";
    }

    [ObservableProperty]
    IEnumerable<string> listOfPeopleNames;
    public void ApplyChanges(string filterOption= null)
    {
        IEnumerable<DebtModel> filteredDebts = [];
        if (filterOption == "Completed")
        {
            filteredDebts = DebtsList
                .Where(x => !x.IsDeleted && x.IsPaidCompletely);
        }
        else if (filterOption == "Pending")
        {
            filteredDebts = DebtsList
                .Where(x => !x.IsDeleted && !x.IsPaidCompletely);
        }
        else
        {
            filteredDebts = DebtsList
                .Where(x => !x.IsDeleted);
        }

        var sortedDebts = filteredDebts
            .OrderByDescending(x => x.IsPaidCompletely)
            .ThenBy(x => x.UpdateDateTime)
            .ToHashSet()
            .ToList();

        DebtsList = sortedDebts.ToObservableCollection();
        RedoCountsAndAmountsCalculation(filteredDebts);
    }



    private void RedoCountsAndAmountsCalculation(IEnumerable<DebtModel> filteredDebts)
    {
        if (filteredDebts == null || !filteredDebts.Any())
    {
        // Xử lý trường hợp danh sách rỗng hoặc null nếu cần
        return;
    }

    // Tạo một danh sách tạm để chứa tất cả các nợ đã được sắp xếp và lọc
    var sortedFilteredDebts = filteredDebts
        .OrderBy(x => x.DebtType)
        .ThenBy(x => x.IsPaidCompletely)
        .ThenBy(x => x.AddedDateTime)
        .ToList();

    // Cập nhật lại DebtsList với danh sách đã lọc và sắp xếp
    DebtsList = new ObservableCollection<DebtModel>(sortedFilteredDebts);

    // Tính toán số lượng và số tiền theo các tiêu chí khác nhau
    var borrowedCompleted = sortedFilteredDebts
        .Where(x => x.DebtType == DebtType.Borrowed && x.IsPaidCompletely);

    var lentCompleted = sortedFilteredDebts
        .Where(x => x.DebtType == DebtType.Lent && x.IsPaidCompletely);

    var borrowedPending = sortedFilteredDebts
        .Where(x => x.DebtType == DebtType.Borrowed && !x.IsPaidCompletely);

    var lentPending = sortedFilteredDebts
        .Where(x => x.DebtType == DebtType.Lent && !x.IsPaidCompletely);

        TotalPendingBorrowCount = borrowedPending.Count();
        TotalCompletedBorrowCount = borrowedCompleted.Count();
        TotalPendingLentCount = lentPending.Count();
        TotalCompletedLentCount = lentCompleted.Count();

        TotalBorrowedCompletedAmount = borrowedCompleted.Sum(x => x.AmountDebt);
        TotalBorrowedPendingAmount = borrowedPending.Sum(x => x.AmountDebt);
        TotalLentCompletedAmount = lentCompleted.Sum(x => x.AmountDebt);
        TotalLentPendingAmount = lentPending.Sum(x => x.AmountDebt);

        TotalBorrowedCount = TotalPendingBorrowCount + TotalCompletedBorrowCount;
        TotalLentCount = TotalPendingLentCount + TotalCompletedLentCount;
    }
    private async Task AddEditDebt()
    {
        await Task.Delay(1);
    }

    [RelayCommand]
    void SearchBar(string query)
    {        
        try
        {
            var ListOfDebts = DebtsList
            .Where(d => d.PersonOrOrganization?.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
            .Where(d => !d.IsDeleted)
            .ToList();

            DebtsList.Clear();
            DebtsList = ListOfDebts.ToObservableCollection();
            RedoCountsAndAmountsCalculation(ListOfDebts);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception : {ex.Message}");
        }
    }

    [RelayCommand]
    void ApplyFilter(string filterOption)
    {
        ApplyChanges(filterOption);
    }


    [RelayCommand]
    void OpenPhoneDialer()
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
            {
                PhoneDialer.Default.Open(SingleDebtDetails.PersonOrOrganization.PhoneNumber);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception : {ex.Message}");
        }
    }

    private void HandleDebtsListUpdated()
    {
        try
        {
            ApplyChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error "+ ex.Message);
        }
    }
}
