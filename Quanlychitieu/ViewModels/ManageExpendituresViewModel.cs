using CommunityToolkit.Maui.Storage;
using Quanlychitieu.Helpers;

namespace Quanlychitieu.ViewModels;
public partial class ManageExpendituresViewModel : BaseViewModel
{
    readonly IExpendituresRepository expendituresService;

    public ManageExpendituresViewModel(IExpendituresRepository expendituresRepository, IUsersRepository usersRepository)
    {
        expendituresService = expendituresRepository;
    }

    [ObservableProperty]
    ObservableCollection<ExpendituresModel> expendituresList;

    [ObservableProperty]
    ObservableCollection<DateGroup> groupedExpenditures;

    [ObservableProperty]
    double totalAmount;

    [ObservableProperty]
    double totalExpenditures;

    [ObservableProperty]
    string userCurrency;

    [ObservableProperty]
    double userPocketMoney;

    [ObservableProperty]
    bool isBusy = true;

    [ObservableProperty]
    string expTitle;

    UsersModel ActiveUser;

    [ObservableProperty]
    bool activ;
    [ObservableProperty]
    bool showStatisticBtn;

    [ObservableProperty]
    bool isSyncing;

    [ObservableProperty]
    List<string> expendituresCat;
    bool IsLoaded;
    [ObservableProperty]
    public int startAction;

    public override async Task LoadDataAsync()
    {
        await Task.WhenAll(expendituresService.SynchronizeExpendituresAsync());
        TotalExpenditures = expendituresService.CalculateTotalExpends();
        ExpendituresList = expendituresService.ExpendituresList;
        TotalAmount = expendituresService.ExpendituresList.Count();
        base.LoadDataAsync();
    }

    [RelayCommand]
    async Task OpenAddExpenditure()
    {
        if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn thêm mới hóa đơn không?"))
        {
            await NavigationService.PushToPageAsync<AddExpendituresPage>();
        }
    }

    [RelayCommand]
    public async Task OpenEditExpenditure(object obj)
    {
        var valueObject = obj as ExpendituresModel;
        if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn sửa thông tin hóa đơn không?"))
        {
            await NavigationService.PushToPageAsync<AddExpendituresPage>(valueObject);
        }
    }



    [RelayCommand]
    public async Task DeleteExpenditureBtn(ExpendituresModel expenditure)
    {
        if (await AlertHelper.ShowConfirmationAlertAsync("Xác nhận xóa đơn?"))
        {
            if (await expendituresService.DeleteExpenditureAsync(expenditure))
            {
                await AlertHelper.ShowInformationAlertAsync("Xóa đơn thành công");
                TotalExpenditures = expendituresService.CalculateTotalExpends();
                ExpendituresList = expendituresService.ExpendituresList;
                TotalAmount = expendituresService.ExpendituresList.Count();
            }
        }
    }

    private async void HandleExpendituresListUpdated()
    {
        try
        {
            ApplyChanges();
        }
        catch (Exception ex)
        {
           await Shell.Current.DisplayAlert("Lỗi Exp", ex.Message, "OK");
        }
    }

    private void ApplyChanges()
    {
        // Update expList
        var expList = expendituresService.ExpendituresList
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.DateSpent).ToList();

        // Update groupedData
        var groupedData = expList.GroupBy(e => e.DateSpent.Date)
            .Select(g => new DateGroup(g.Key, g.ToList()))
            .ToList();

        // Update GroupedExpenditures
        GroupedExpenditures = new ObservableCollection<DateGroup>(groupedData);
        OnPropertyChanged(nameof(GroupedExpenditures));

        // Update TotalAmount
        TotalAmount = expList.AsParallel().Sum(x => x.AmountSpent);

        // Update TotalExpenditures
        TotalExpenditures = expList.Count;

        // Update ShowStatisticBtn
        ShowStatisticBtn = expList.Count >= 3;
    }

    [RelayCommand]
    public async Task GoToSpecificStatsPage()
    {
        if (GroupedExpenditures is null)
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Không có dữ liệu"));
            return;
        }

        var navParam = new Dictionary<string, object>
        {
            { "GroupedExpList", GroupedExpenditures }
        };

        await ManageExpendituresNavs.FromManageExpToSingleMonthStats(navParam);
    }

    [RelayCommand]
    public async Task CopyToClipboard(ExpendituresModel singlExp)
    {
        CancellationTokenSource cancellationTokenSource = new();
        const ToastDuration duration = ToastDuration.Short;
        const double fontSize = 14;
        const string text = "Chi tiết hóa đơn được sao chép";
        var toast = Toast.Make(text, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token); //toast a notification about exp being copied to clipboard
    }

    [RelayCommand]
    public async Task DropCollection()
    {
        await Task.Delay(1);
    }
}

public class DateGroup : ObservableCollection<ExpendituresModel>
{
    public DateTime Date { get; set; }
    public double TotalAmount { get; set; }
    public int TotalCount { get; set; }
    public string Currency { get; }
    public DateGroup(DateTime date, List<ExpendituresModel> expenditures) : base(expenditures)
    {
        Date = date;
        TotalAmount = expenditures.Sum(x => x.AmountSpent);
        TotalCount = expenditures.Count;
        Currency = expenditures[0].Currency;
    }
}