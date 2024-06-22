using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Platforms.Android.NavigationsMethods;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.ViewModels.Incomes;

public partial class UpSertIncomeViewModel : BaseViewModel
{
    private readonly IIncomeRepository incomeService;
    private readonly IUsersRepository userService;
    private readonly ManageIncomesNavs NavFunctions = new();

    public UpSertIncomeViewModel(IIncomeRepository incomeRepository, IUsersRepository usersRepository, IncomeModel singleIncomeDetails, string pageTitle, bool isAdd, UsersModel activeUser)
    {
        incomeService = incomeRepository;
        userService = usersRepository;
        SingleIncomeDetails = singleIncomeDetails;
        PageTitle = pageTitle;
        IsAdd = isAdd;
        ActiveUser = activeUser;
    }

    [ObservableProperty]
    IncomeModel _singleIncomeDetails;

    [ObservableProperty]
    string pageTitle;

    [ObservableProperty]
    UsersModel _activeUser;
    [ObservableProperty]
    double resultingBalance;
    [ObservableProperty]
    public bool closePopUp;

    public PopupResult ThisPopUpResult;

    double InitialUserPockerMoney;
    double InitialIncomeAmout;
    double _initialTotalIncAmount;

    public bool IsAdd { get; }

    [RelayCommand]
    public void PageLoaded()
    {
        InitialUserPockerMoney = ActiveUser.PocketMoney;
        InitialIncomeAmout = SingleIncomeDetails.AmountReceived;
        _initialTotalIncAmount = ActiveUser.TotalIncomeAmount;
        ResultingBalance = ActiveUser.PocketMoney;
    }
    [RelayCommand]
    public async Task UpSertIncome()
    {
        CancellationTokenSource cancellationTokenSource = new();
        const ToastDuration duration = ToastDuration.Short;
        const double fontsize = 14;
        if (SingleIncomeDetails.Id is null)
        {
            await AddIncomeAsync(duration, fontsize, cancellationTokenSource);
        }
        else
        {
            await UpdateIncomeAsync(duration, fontsize, cancellationTokenSource);
        }
        ThisPopUpResult = PopupResult.OK;
        ClosePopUp = true;
    }

    public override async Task LoadDataAsync()
    {
        ActiveUser = await userService.GetUserAsync();
        base.LoadDataAsync();
    }

    async Task UpdateIncomeAsync(ToastDuration duration, double fontSize, CancellationTokenSource tokenSource)
    {
        double difference = SingleIncomeDetails.AmountReceived - InitialIncomeAmout;

        double FinalTotalInc = _initialTotalIncAmount + difference;
        double FinalPocketMoney = InitialUserPockerMoney + difference;
        SingleIncomeDetails.UpdatedDateTime = DateTime.UtcNow;
        if (FinalPocketMoney < 0)
        {
            // Chưa xử lý
        }
        else if (await incomeService.UpdateIncomeAsync(SingleIncomeDetails))
        {
            ActiveUser.TotalIncomeAmount += FinalTotalInc;
            ActiveUser.PocketMoney = FinalPocketMoney;
            ActiveUser.DateTimeOfPocketMoneyUpdate = DateTime.UtcNow;
            await userService.UpdateUserAsync(ActiveUser);

            const string toastNotifMessage = "Đang cập nhật";
            var toastObj = Toast.Make(toastNotifMessage, duration, fontSize);
            await toastObj.Show(tokenSource.Token);

            await ManageIncomesNavs.ReturnOnce();
        }
    }

    async Task AddIncomeAsync(ToastDuration duration, double fontSize, CancellationTokenSource tokenSource)
    {
        if (SingleIncomeDetails.AmountReceived <= 0)
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Số tiền không thể nhỏ hơn 0"));
            return;
        }
        else
        {
            SingleIncomeDetails.Id = Guid.NewGuid().ToString();
            SingleIncomeDetails.UserId = ActiveUser.Id;
            if (await incomeService.AddIncomeAsync(SingleIncomeDetails))
            {
                ActiveUser.TotalIncomeAmount += SingleIncomeDetails.AmountReceived;
                double FinalPocketMoney = InitialUserPockerMoney + SingleIncomeDetails.AmountReceived;
                ActiveUser.PocketMoney = FinalPocketMoney;
                ActiveUser.DateTimeOfPocketMoneyUpdate = DateTime.UtcNow;

                // await userService.UpdateUserAsync(ActiveUser);

                const string toastNotifMessage = "Đã thêm";
                var toast = Toast.Make(toastNotifMessage, duration, fontSize);
                await toast.Show(tokenSource.Token);

                await ManageIncomesNavs.ReturnOnce();
            }
        }
    }
    public void AmountReceivedChanged()
    {
        ResultingBalance = InitialUserPockerMoney - InitialIncomeAmout + SingleIncomeDetails.AmountReceived;
    }

    [RelayCommand]
    public void CancelBtn()
    {
        ThisPopUpResult = PopupResult.Cancel;
        ClosePopUp = true;
    }
}
