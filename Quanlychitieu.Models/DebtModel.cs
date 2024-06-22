using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Quanlychitieu.Models;

public partial class DebtModel : ObservableObject
{
    private string? displayText;
    [JsonProperty("_id")]
    public string Id { get; set; }
    public double AmountDebt { get; set; }
    public DebtType DebtType { get; set; } = DebtType.Lent;
    public required PersonOrOrganizationModel PersonOrOrganization { get; set; }
    public DateTime? Deadline { get; set; } 
    public double AmountPaid { get; set; }
    public string? Currency { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;
    public string? PhoneAddress { get; set; } = string.Empty;
    public DateTime? DatePaidCompletely { get; set; }
    public DateTime AddedDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public bool IsDeleted { get; set; }
    public string? UserId { get; set; } = string.Empty;
    public string? PlatformModel { get; set; } = string.Empty;
    private ObservableCollection<InstallmentPayments>? paymentAdvances { get; set; } = new ObservableCollection<InstallmentPayments>();

    public ObservableCollection<InstallmentPayments> PaymentAdvances
    {
        get => paymentAdvances;
        set
        {
            if (paymentAdvances != value)
            {
                paymentAdvances = value;
                OnPropertyChanged(nameof(PaymentAdvances));
            }
        }
    }

    public string DisplayText
    {
        get => displayText;
        set
        {
            if (displayText != value)
            {
                displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }
    public bool IsPaidCompletely { get; set; }
}
public enum DebtType
{
    Borrowed, // User has borrowed money (owes money)
    Lent      // User has lent money (is owed money)
}
public class InstallmentPayments
{
    public string Id { get; set; }
    public required double AmountPaid { get; set; }
    public string? ReasonForOptionalPayment { get; set; }
    public required DateTime DatePaid { get; set; } = DateTime.Now;
}
public class PersonOrOrganizationModel
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}