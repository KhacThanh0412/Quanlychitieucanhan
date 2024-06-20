
using Plugin.Maui.CalendarStore;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.ViewModels.Debts;

[QueryProperty(nameof(SingleDebtDetails), "SingleDebtDetails")]
public partial class UpSertDebtViewModel(IDebtRepository debtRepository, IUsersRepository usersRepository, ICalendarStore calendarStore) : ObservableObject
{
    readonly IDebtRepository debtRepo = debtRepository;
    readonly IUsersRepository userRepo = usersRepository;
    private readonly ICalendarStore calendarStoreRepo = calendarStore;
    [ObservableProperty]
    DebtModel singleDebtDetails;

    [ObservableProperty]
    InstallmentPayments singleInstallmentPayment;

    [ObservableProperty]
    string pageTitle;
    [ObservableProperty]
    bool hasDeadLine;
    [ObservableProperty]
    bool isBottomSheetOpened;
    [ObservableProperty]
    PopupResult thisPopUpResult;

    [ObservableProperty]
    bool closePopUp;
    bool isLent;
    bool isBorrow;

    [ObservableProperty]
    bool isUpSertInstallmentBSheetPresent;
    public bool IsLent
    {
        get => isLent;
        set
        {
            if (SetProperty(ref isLent, value) && value)
            {
                SetProperty(ref isBorrow, false, nameof(IsBorrow));
            }
        }
    }
    public bool IsBorrow
    {
        get => isBorrow;
        set
        {
            if (SetProperty(ref isBorrow, value) && value)
            {
                SetProperty(ref isLent, false, nameof(IsLent));
            }
        }
    }

    public DebtType DebtType
    {
        get => IsLent ? DebtType.Lent : DebtType.Borrowed;
        set
        {
            IsLent = value == DebtType.Lent;
        }
    }


    [ObservableProperty]
    List<PersonOrOrganizationModel> listOfPersons;
    [ObservableProperty]
    List<string> listOfPersonsNames;

    public double selectedInstallmentInitialAmount;
    public void PageLoaded()
    {
        DebtType = SingleDebtDetails.DebtType;

        IsLent = DebtType == DebtType.Lent;
        HasDeadLine = SingleDebtDetails.Deadline is not null;
        ListOfPersons = debtRepo.DebtList.Select(x => x.PersonOrOrganization)
            .Distinct()
            .ToList();
        ListOfPersonsNames = ListOfPersons.Select(x => x.Name)
            .Distinct()
            .ToList();
    }

    [RelayCommand]
    public async Task UpSertDebt()
    {
        if (string.IsNullOrEmpty(SingleDebtDetails.Notes) || string.IsNullOrWhiteSpace(SingleDebtDetails.Notes))
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Vui lòng thêm ghi chú!"));
            return;
        }
        CancellationTokenSource cts = new();
        const ToastDuration duration = ToastDuration.Short;

        SingleDebtDetails.UpdateDateTime = DateTime.UtcNow;
        SingleDebtDetails.PlatformModel = DeviceInfo.Current.Model;
        SingleDebtDetails.DebtType = DebtType;
        if (HasDeadLine is false)
        {
            SingleDebtDetails.DisplayText = "Đang chờ xử lý Không có thời hạn";
        }
        else
        {
            var diff = SingleDebtDetails.Deadline.Value.Date - DateTime.Now.Date;
            if (diff.TotalDays == 1)
            {
                SingleDebtDetails.DisplayText = "Đang chờ xử lý sau 1 ngày";
            }
            else if (diff.TotalDays == 0)
            {
                SingleDebtDetails.DisplayText = "Đang chờ xử lý hôm nay!";
            }
            else if (diff.TotalDays > 1)
            {
                SingleDebtDetails.DisplayText = $"Đang chờ đến hạn vào {diff.TotalDays} ngày";
            }
            else if (diff.TotalDays < 0)
            {
                SingleDebtDetails.DisplayText = $"Đang chờ xử lý {diff.TotalDays} ngày";
            }

        }
        if (SingleDebtDetails.Id is not null)
        {
            await UpdateDebtAsync(14, cts, duration);
        }
        else
        {
            await AddDebtAsync(14, cts, duration);
        }

        ThisPopUpResult = PopupResult.OK;
        ClosePopUp = true;
    }

    [ObservableProperty]
    string selectedCalendarItem;
    private async Task AddDebtAsync(int fontSize, CancellationTokenSource cts, ToastDuration duration)
    {
        SingleDebtDetails.Id = Guid.NewGuid().ToString();
        SingleDebtDetails.AddedDateTime = DateTime.UtcNow;
        if (HasDeadLine is not true)
        {
            SingleDebtDetails.Deadline = null;
            SingleDebtDetails.DatePaidCompletely = null;
        }
        //this saves the debt to db and online
        if (!await debtRepo.AddDebtAsync(SingleDebtDetails))
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Không thể thêm"));
            return;
        }

        if (HasDeadLine is true && SingleDebtDetails.Deadline is not null)
        {
            var calendarStatusRead = await CheckAndRequestReadCalendarPermission();
            if (calendarStatusRead != PermissionStatus.Granted)
            {
                return;
            }
            var calendarStatusWrite = await CheckAndRequestWriteCalendarPermission();
            if (calendarStatusWrite != PermissionStatus.Granted)
            {
                return;
            }

            var calendarsAccountsProfiles = await calendarStoreRepo.GetCalendars();


            if (calendarsAccountsProfiles is null || calendarsAccountsProfiles.Count() == 0)
            {
                await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Không tìm thấy tài khoản trên thiết bị"));
                const string toastNotifMessageError = "Không được thêm vào";
                var toasts = Toast.Make(toastNotifMessageError, duration, fontSize);
                await toasts.Show(cts.Token);
                IsBottomSheetOpened = false;
                return;

            }

            var selectedProfile = await Shell.Current.DisplayActionSheet("Chọn lịch", "Cancel", null, calendarsAccountsProfiles.Select(x => x.Name).ToArray());
            if (selectedProfile == "Cancel")
            {
                return;
            }
            var calendarProfileID = calendarsAccountsProfiles
                .Where(x => x.Name == selectedProfile)
                .Select(x => x.Id)
                .FirstOrDefault();

            DateTimeOffset deadlineOffsetStart = new DateTimeOffset(SingleDebtDetails.Deadline.Value).AddHours(12);
            DateTimeOffset deadlineOffsetEnd = deadlineOffsetStart.AddMinutes(30);

            var eventID = await calendarStoreRepo.CreateEventWithReminder(calendarProfileID, "Thời gian chờ !",
                $"{(SingleDebtDetails.DebtType == DebtType.Lent ? $"{SingleDebtDetails.PersonOrOrganization.Name} Nợ bạn" : $"Bạn nợ {SingleDebtDetails.PersonOrOrganization.Name}")} {SingleDebtDetails.Amount} {SingleDebtDetails.Currency} {Environment.NewLine}{SingleDebtDetails.PhoneAddress}",
                "Ứng dụng quản lý chi tiêu", deadlineOffsetStart, deadlineOffsetEnd, 30);
        }

        const string toastNotifMessage = "Thêm";
        var toast = Toast.Make(toastNotifMessage, duration, fontSize);
        await toast.Show(cts.Token);
        IsBottomSheetOpened = false;

    }

    private async Task UpdateDebtAsync(int fontSize, CancellationTokenSource cts, ToastDuration duration)
    {
        if (!await debtRepo.UpdateDebtAsync(SingleDebtDetails))
        {
            await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Không thể cập nhật"));
            return;
        }

        //maybe i'll need to update user idk
        const string toastNotifMessage = "Đã cập nhật thành công";
        var toast = Toast.Make(toastNotifMessage, duration, fontSize);
        await toast.Show(cts.Token);
        IsBottomSheetOpened = false;
    }

    [RelayCommand]
    public void CancelBtn()
    {
        ThisPopUpResult = PopupResult.Cancel;
        ClosePopUp = true;
    }

    [RelayCommand]
    public async Task UpSertInstallmentPayment()
    {
        bool ProcessInstallmentPayment()
        {
            if (SingleInstallmentPayment.Id is null)
            {
                return AddInstallmentPayment();
            }
            else
            {
                return EditInstallmentPayment();
            }
        }

        if (ProcessInstallmentPayment())
        {
            await debtRepo.UpdateDebtAsync(SingleDebtDetails);
        }
        ThisPopUpResult = PopupResult.OK;
        ClosePopUp = true;
        IsUpSertInstallmentBSheetPresent = false;
    }
    [RelayCommand]
    void CloseInstallmentsPopup()
    {
        ThisPopUpResult = PopupResult.Cancel;
        ClosePopUp = true;
        IsUpSertInstallmentBSheetPresent = false;
        
    }
    private bool AddInstallmentPayment()
    {
        if (SingleInstallmentPayment.AmountPaid < 1)
        {
            return false;
        }
        SingleInstallmentPayment.Id = Guid.NewGuid().ToString();

        if (SingleDebtDetails.PaymentAdvances is null)
        {
            SingleDebtDetails.PaymentAdvances = [SingleInstallmentPayment];
        }
        else
        {
            SingleDebtDetails.PaymentAdvances.Add(SingleInstallmentPayment);
        }
        SingleDebtDetails.Amount -= SingleInstallmentPayment.AmountPaid;
        return true;
    }


    private bool EditInstallmentPayment()
    {
                
        if (SingleInstallmentPayment.AmountPaid < 0)
        {
            return false;
        }

        int index = SingleDebtDetails.PaymentAdvances
            .Select((item, idx) => new { Item = item, Index = idx })
            .FirstOrDefault(x => x.Item.Id == SingleInstallmentPayment.Id)?.Index ?? -1;

        if (index != -1)
        {
            // Replace the found item
            SingleDebtDetails.PaymentAdvances[index] = SingleInstallmentPayment;

            double amountDifference = selectedInstallmentInitialAmount - SingleInstallmentPayment.AmountPaid;
            if ((SingleInstallmentPayment.AmountPaid < selectedInstallmentInitialAmount))
            {
                SingleDebtDetails.Amount += (double)amountDifference;
            }
            else
            {
                SingleDebtDetails.Amount -= (double)-amountDifference;
            }

            return true;

        }
        else
        {
            return false;
        }
    }
    [RelayCommand]
    public async Task DeleteInstallmentPayment(InstallmentPayments installment)
    {
        SingleDebtDetails.PaymentAdvances.Remove(installment);
        SingleDebtDetails.Amount += installment.AmountPaid;
        await debtRepo.UpdateDebtAsync(SingleDebtDetails);

        IsUpSertInstallmentBSheetPresent = false;
    }
    [RelayCommand]
    async Task ContactDetailsPicker()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            }

            var PickedContact = await Contacts.Default.PickContactAsync();
            if (PickedContact is null)
            {
                
            }
            SingleDebtDetails.PersonOrOrganization = new()
            {
                Name = PickedContact.DisplayName,
                PhoneNumber = PickedContact.Phones.FirstOrDefault()?.PhoneNumber,
                Email = PickedContact.Emails.FirstOrDefault()?.EmailAddress
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Permission denied " + ex.Message);
        }
    }

    public static async Task<PermissionStatus> CheckAndRequestReadCalendarPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();

        if (status == PermissionStatus.Granted)
        {
            return status;
        }

        if (status == PermissionStatus.Denied)
        {
            status = await Permissions.RequestAsync<Permissions.CalendarRead>();
            return status;
        }

        status = await Permissions.RequestAsync<Permissions.CalendarRead>();

        return status;
    }

    /// <summary>
    /// CheckAndRequestWriteCalendarPermission
    /// </summary>
    /// <returns></returns>
    public static async Task<PermissionStatus> CheckAndRequestWriteCalendarPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();

        if (status == PermissionStatus.Granted)
        {
            return status;
        }

        if (status == PermissionStatus.Denied)
        {
            status = await Permissions.RequestAsync<Permissions.CalendarWrite>();
            return status;
        }

        status = await Permissions.RequestAsync<Permissions.CalendarWrite>();

        return status;
    }
}