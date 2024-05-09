
using Plugin.Maui.CalendarStore;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;

namespace Quanlychitieu.ViewModels.Debts;

[QueryProperty(nameof(SingleDebtDetails), "SingleDebtDetails")]
public partial class UpSertDebtViewModel( ICalendarStore calendarStore) : ObservableObject
{
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
    }

    [RelayCommand]
    public async Task UpSertDebt()
    {
        await Task.Delay(1);
    }

    [ObservableProperty]
    string selectedCalendarItem;
    private async Task AddDebtAsync(int fontSize, CancellationTokenSource cts, ToastDuration duration)
    {
        await Task.Delay(1);
    }

    private async Task UpdateDebtAsync(int fontSize, CancellationTokenSource cts, ToastDuration duration)
    {
        await Task.Delay(1);
    }

    [RelayCommand]
    public void CancelBtn()
    {
        Debug.WriteLine("Action cancelled by user");
        ThisPopUpResult = PopupResult.Cancel;
        ClosePopUp = true;
    }

    [RelayCommand]
    public async Task UpSertInstallmentPayment()
    {
        await Task.Delay(1);
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
            throw new Exception("No Installment found");
        }
    }
    [RelayCommand]
    public async Task DeleteInstallmentPayment(InstallmentPayments installment)
    {
        await Task.Delay(1);
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
                Debug.WriteLine("Contact not picked");
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