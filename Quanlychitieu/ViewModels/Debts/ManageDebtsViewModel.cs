
using Plugin.Maui.CalendarStore;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;
using Quanlychitieu.Views.Debts;

namespace Quanlychitieu.ViewModels.Debts;

public partial class ManageDebtsViewModel : ObservableObject
{
    private readonly IDebtRepository debtRepo;
    private readonly IUsersRepository usersRepo;
    private readonly UpSertDebtViewModel upSertDebtVM;

    public ManageDebtsViewModel(IDebtRepository debtRepository, IUsersRepository usersRepository,
        UpSertDebtViewModel upSertDebtViewModel)
    {
        debtRepo = debtRepository;
        usersRepo = usersRepository;
        upSertDebtVM = upSertDebtViewModel;
        debtRepo.DebtListChanged += HandleDebtsListUpdated;
    }
    [ObservableProperty]
    ObservableCollection<DebtModel> debtsList;
    [ObservableProperty]
    ObservableCollection<DebtModel> borrowedCompletedList;
    [ObservableProperty]
    ObservableCollection<DebtModel> lentCompletedList;
    [ObservableProperty]
    ObservableCollection<DebtModel> borrowedPendingList;
    [ObservableProperty]
    ObservableCollection<DebtModel> lentPendingList;


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
    UsersModel activeUser;
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


    public void PageLoaded()
    {
        try
        {
            if (!IsLoaded)
            {
                ApplyChanges();
                IsLoaded = true;
                ActiveUser = usersRepo.User;
                UserCurrency = ActiveUser.UserCurrency;
                SingleDebtDetails = new DebtModel()
                {
                    Amount = 0,
                    PersonOrOrganization = new()
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception when loading all debts MESSAGE : {ex.Message}");
        }
    }


    [RelayCommand]
    public async Task ShowDebtDetails(DebtModel debt)
    {
        SingleDebtDetails = debt;
        await Shell.Current.GoToAsync(nameof(SingleDebtDetailsPage), true);
    }

    public void RefreshTitleText()
    {
        TitleText = SingleDebtDetails.DebtType == DebtType.Lent
                    ? $"{SingleDebtDetails.PersonOrOrganization.Name} nợ bạn {SingleDebtDetails.Amount} {SingleDebtDetails.Currency}"
                    : $"Bạn nợ {SingleDebtDetails.PersonOrOrganization.Name}, {SingleDebtDetails.Amount} {SingleDebtDetails.Currency}";
    }

    [ObservableProperty]
    IEnumerable<string> listOfPeopleNames;
    public void ApplyChanges(string filterOption=null)
    {
        IEnumerable<DebtModel> filteredDebts = [];
        if (filterOption == "Completed")
        {
            filteredDebts = debtRepo.DebtList
                .Where(x => !x.IsDeleted && x.IsPaidCompletely);
        }
        else if (filterOption == "Pending")
        {
            filteredDebts = debtRepo.DebtList
                .Where(x => !x.IsDeleted && !x.IsPaidCompletely);
        }
        else
        {
            filteredDebts = debtRepo.DebtList
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

        var borrowedCompleted = filteredDebts
        .Where(x => x.DebtType == DebtType.Borrowed && x.IsPaidCompletely)
        .OrderBy(x => x.AddedDateTime);

        var lentCompleted = filteredDebts
            .Where(x => x.DebtType == DebtType.Lent && x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime);

        var borrowedPending = filteredDebts
            .Where(x => x.DebtType == DebtType.Borrowed && !x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime);

        var lentPending = filteredDebts
            .Where(x => x.DebtType == DebtType.Lent && !x.IsPaidCompletely)
            .OrderBy(x => x.AddedDateTime);

        BorrowedCompletedList = new ObservableCollection<DebtModel>(borrowedCompleted);
        LentCompletedList = new ObservableCollection<DebtModel>(lentCompleted);
        BorrowedPendingList = new ObservableCollection<DebtModel>(borrowedPending);
        LentPendingList = new ObservableCollection<DebtModel>(lentPending);

        TotalPendingBorrowCount = BorrowedPendingList.Count;
        TotalCompletedBorrowCount = BorrowedCompletedList.Count;
        TotalPendingLentCount = LentPendingList.Count;
        TotalCompletedLentCount = LentCompletedList.Count;

        TotalBorrowedCompletedAmount = BorrowedCompletedList.Sum(x => x.Amount);
        TotalBorrowedPendingAmount = BorrowedPendingList.Sum(x => x.Amount);
        TotalLentCompletedAmount = LentCompletedList.Sum(x => x.Amount);
        TotalLentPendingAmount = LentPendingList.Sum(x => x.Amount);

        TotalBorrowedCount = TotalPendingBorrowCount + TotalCompletedBorrowCount;
        TotalLentCount = TotalPendingLentCount + TotalCompletedLentCount;
    }

    [RelayCommand]
    public async Task ShowAddDebtPopUp()
    {
        if (ActiveUser is null)
        {
            Debug.WriteLine("Can't Open Add Debt PopUp because User is null");
            await Shell.Current.DisplayAlert("Đợi", "Không thể đi", "Ok");
        }
        else
        {

            var newDebt = new DebtModel
            {
                Amount = 1,
                PersonOrOrganization = new PersonOrOrganizationModel(),
                Currency = ActiveUser.UserCurrency
            };
            upSertDebtVM.SingleDebtDetails = newDebt;
            upSertDebtVM.IsLent = true;
            await AddEditDebt();
        }
    }

    [RelayCommand]
    public async Task ShowEditDebtPopUp(DebtModel debt)
    {
        upSertDebtVM.SingleDebtDetails = debt;
        upSertDebtVM.HasDeadLine = debt.Deadline is not null;
        upSertDebtVM.IsLent = debt.DebtType == DebtType.Lent;
        await AddEditDebt();
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
            var ListOfDebts = debtRepo.DebtList
            .Where(d => d.PersonOrOrganization?.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
            .Where(d => !d.IsDeleted)
            .ToList();

            DebtsList.Clear();
            DebtsList = ListOfDebts.ToObservableCollection();
            RedoCountsAndAmountsCalculation(ListOfDebts);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FLOW HOLD EXCEPTION : {ex.Message}");
        }
    }

   
   
    [RelayCommand]
    async Task DeleteDebtAsync(DebtModel debt)
    {
        CancellationTokenSource cancellationTokenSource = new();
        const ToastDuration duration = ToastDuration.Short;
        const double fontSize = 14;
        string text;
        try
        {
            var response = await Shell.Current.DisplayAlert("Xác nhận", "Xóa ?", "Có", "Không");
            if (response)
            {
                debt.PlatformModel = DeviceInfo.Current.Model;
                if (await debtRepo.DeleteDebtAsync(debt))
                {
                    text = "Xóa thành công";
                }
                else
                {
                    text = "Xóa thất bại";
                }
                var toast = Toast.Make(text, duration, fontSize);
                await toast.Show(cancellationTokenSource.Token);
                ApplyChanges();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception when deleting debt MESSAGE : {ex.Message}");
        }
    }

    [RelayCommand]
    async Task UpSertInstallmentPaymentPopUp(InstallmentPayments selectedInstallment = null)
    {
        upSertDebtVM.SingleDebtDetails = SingleDebtDetails;
        upSertDebtVM.SingleInstallmentPayment = selectedInstallment is null ? new() { AmountPaid = 0, DatePaid = DateTime.Now } : selectedInstallment;
        upSertDebtVM.selectedInstallmentInitialAmount = selectedInstallment is null ? 0 : selectedInstallment.AmountPaid;
        
        var result = (PopUpCloseResult)await Shell.Current.ShowPopupAsync(new UpSertInstallmentPayment(upSertDebtVM));
        {
            RefreshTitleText();
            Debug.WriteLine($"Installments Popup Closed {result.Result}");
        }
    }
    [RelayCommand]
    async Task DeleteInstallment(InstallmentPayments selectedInstallment)
    {
        var deletedResult = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Xác nhận xóa?"));
        if (deletedResult)
        {
            upSertDebtVM.SingleDebtDetails = SingleDebtDetails;
            upSertDebtVM.SingleInstallmentPayment = selectedInstallment;

            await upSertDebtVM.DeleteInstallmentPayment(selectedInstallment);
            RefreshTitleText();           

        }
    }

    [RelayCommand]
    async Task ToggleDebtCompletionStatus(object s)
    {
        await Task.Delay(1);
        if (s.GetType() == typeof(DebtModel))
        {
            var debt = (DebtModel)s;
            CancellationTokenSource cancellationTokenSource = new();
            const ToastDuration duration = ToastDuration.Short;
            const double fontSize = 14;
            string text;
            try
            {
                string message = debt.IsPaidCompletely ? "Đánh dấu đã hoàn thành" : "Đánh dấu là đang chờ xử lý";
                var response = (bool)await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert(message));
                if (response)
                {
                    bool completedSwapper = !debt.IsPaidCompletely;

                    if (!debt.IsPaidCompletely) // to mark as pending
                    {
                        debt.IsPaidCompletely = completedSwapper; // to unpaid completely  
                        debt.DatePaidCompletely = null;
                        if (debt.Deadline.HasValue)
                        {
                            var diff = DateTime.Now.Date - debt.Deadline.Value.Date;
                            if (diff.TotalDays == 1)
                            {
                                debt.DisplayText = $"Đến hạn {-diff.TotalDays} ngày";
                            }
                            if (diff.TotalDays > 1)
                            {
                                debt.DisplayText = $"Quá hạn {diff.TotalDays} ngày!";
                            }
                            else if (diff.TotalDays < 0)
                            {
                                debt.DisplayText = $"Đến hạn {-diff.TotalDays} ngày";
                            }
                            else
                            {
                                debt.DisplayText = "Hạn hôm nay";
                            }

                        }
                        else
                        {
                            debt.DisplayText = "Đang chờ xử lý Không có thời hạn";
                        }
                        text = "Được đánh dấu đang chờ xử lý";
                    }
                    else
                    {
                        debt.IsPaidCompletely = completedSwapper;
                        debt.DatePaidCompletely = DateTime.Now;

                        if (debt.Deadline.HasValue)
                        {
                            var DatePaidDiff = DateTime.Now.Date - debt.DatePaidCompletely?.Date;
                            if (DatePaidDiff.Value.TotalDays == 1)
                            {
                                debt.DisplayText = "Đã thanh toán 1 ngày trước";
                            }
                            else if (DatePaidDiff.Value.TotalDays == 0)
                            {
                                debt.DisplayText = "Đã thanh toán hôm nay";
                            }
                            else
                            {
                                debt.DisplayText = $"Trả {DatePaidDiff.Value.TotalDays} ngày trước";
                            }
                        }
                        else
                        {
                            debt.DisplayText = "Được thanh toán hôm nay";
                        }
                        text = "Đánh dấu đã hoàn thành";
                    }
                    await debtRepo.UpdateDebtAsync(debt);

                    var toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token); //toast a notification about exp deletion
                    ApplyChanges();
                }
                else
                {
                    debt.IsPaidCompletely = !debt.IsPaidCompletely;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception when Marking as completed debt MESSAGE : {ex.Message}");
            }
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
            Debug.WriteLine($"Exception when opening phone dialer MESSAGE : {ex.Message}");
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
            Debug.WriteLine("Error when added debts "+ ex.Message);
        }
    }
}
