
//This is the view model for the page that shows ALL expenditures

using CommunityToolkit.Maui.Storage;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Platforms.Android.NavigationsMethods;
using Quanlychitieu.Platforms.Android.PDFClasses;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;
using Quanlychitieu.Views;

namespace Quanlychitieu.ViewModels.Expenditures;
public partial class ManageExpendituresViewModel : ObservableObject
{
    readonly IExpendituresRepository expendituresService;
    readonly IUsersRepository userRepo;
    private readonly UpSertExpenditureViewModel upSertExpenditureVM;
    private readonly IFolderPicker folderPickerService;

    public ManageExpendituresViewModel(IExpendituresRepository expendituresRepository, IUsersRepository usersRepository,
        UpSertExpenditureViewModel upSertExpenditureVM, IFolderPicker folderPickerService)
    {
        expendituresService = expendituresRepository;
        userRepo = usersRepository;
        this.upSertExpenditureVM = upSertExpenditureVM;
        this.folderPickerService = folderPickerService;
        ExpendituresCat = ExpenditureCategoryDescriptions.Descriptions;
        expendituresService.ExpendituresListChanged += HandleExpendituresListUpdated;
        userRepo.UserDataChanged += HandleUserDataChanged;
    }

    private void HandleUserDataChanged()
    {
        UserPocketMoney = userRepo.User.PocketMoney;
    }

    [ObservableProperty]
    ObservableCollection<ExpendituresModel> expendituresList;

    [ObservableProperty]
    ObservableCollection<DateGroup> groupedExpenditures;

    [ObservableProperty]
    double totalAmount;

    [ObservableProperty]
    int totalExpenditures;

    [ObservableProperty]
    string userCurrency;

    [ObservableProperty]
    double userPocketMoney;

    [ObservableProperty]
    bool isBusy = true;

    [ObservableProperty]
    string expTitle;

    UsersModel ActiveUser = new();

    [ObservableProperty]
    bool activ;
    [ObservableProperty]
    bool showStatisticBtn;

    [ObservableProperty]
    bool isSyncing;

    [ObservableProperty]
    List<string> expendituresCat;

    public async Task PageloadedAsync()
    {
        UsersModel user = userRepo.User;
        ActiveUser = user;

        UserPocketMoney = ActiveUser.PocketMoney;
        GetAllExp();

    }

    bool IsLoaded;

    [ObservableProperty]
    public int startAction;
    [RelayCommand]
    public void GetAllExp()
    {
        try
        {
            if (!IsLoaded)
            {
                ExpTitle = "Tất cả hóa đơn";
                ApplyChanges();

                IsBusy = false;

                IsLoaded = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");
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
    public async Task ShowAddExpenditurePopUp()
    {
        if (ActiveUser is null)
        {
            await Shell.Current.DisplayAlert("Đợi", "Không thể đi", "Ok");
        }
        else
        {
            var newExpenditure = new ExpendituresModel() { DateSpent = DateTime.Now };
            
            upSertExpenditureVM.SingleExpenditureDetails = newExpenditure;
            upSertExpenditureVM.ClosePopUp = false;
            await AddEditExpediture();
        }
    }

    [RelayCommand]
    public async Task ShowEditExpenditurePopUp(ExpendituresModel expenditure)
    {
        upSertExpenditureVM.SingleExpenditureDetails = expenditure;
        upSertExpenditureVM.ClosePopUp = false;
        await AddEditExpediture();
    }
    private async Task AddEditExpediture()
    {
        var result = (PopUpCloseResult) await Shell.Current.ShowPopupAsync(new UpSertExpendituresPopUp(upSertExpenditureVM));
        if(result.Result == PopupResult.OK)
        {
            var resultingBalance = (double)result.Data;
            UserPocketMoney = resultingBalance;
        }
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
    public async Task DeleteExpenditureBtn(ExpendituresModel expenditure)
    {
        CancellationTokenSource cancellationTokenSource = new();
        const ToastDuration duration = ToastDuration.Short;
        const double fontSize = 14;
        string text;
        bool response = (bool)(await Shell.Current.ShowPopupAsync(new AcceptCancelPopUpAlert("Xác nhận xóa ?")))!;
        if (response)
        {
            IsBusy = true;
            expenditure.UpdatedDateTime = DateTime.UtcNow;
            expenditure.PlatformModel = DeviceInfo.Current.Model;
            var deleteResponse = await expendituresService.DeleteExpenditureAsync(expenditure); //delete the expenditure from db

            if (deleteResponse)
            {
                text = "Đã xóa hóa đơn thành công";
                ActiveUser.TotalExpendituresAmount -= expenditure.AmountSpent;
                ActiveUser.PocketMoney += expenditure.AmountSpent;
                UserPocketMoney += expenditure.AmountSpent;
                await userRepo.UpdateUserAsync(ActiveUser);
            }
            else
            {
                text = "Hóa đơn không bị xóa";
            }
            var toast = Toast.Make(text, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token); //toast a notification about exp deletion
            ApplyChanges();
            IsBusy = false;
        }
    }

    public async Task PrintExpendituresBtn()
    {
        await Task.Delay(1);
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

    [RelayCommand]
    public async Task DropCollection()
    {
        
        await expendituresService.DropExpendituresCollection();
    }
}

public class DateGroup : List<ExpendituresModel>
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