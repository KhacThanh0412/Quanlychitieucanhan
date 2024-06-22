using Newtonsoft.Json;
using System.ComponentModel;

namespace Quanlychitieu.Models;

public class ExpendituresModel : INotifyPropertyChanged
{
    double amountSpent;
    [JsonProperty("_id")]
    public string Id { get; set; }
    public DateTime DateSpent { get; set; }
    public double UnitPrice { get; set; }
    public double Quantity { get; set; } = 1;
    public double AmountSpent
    {
        get => amountSpent;
        set
        {
            amountSpent = value;
            OnPropertyChanged(nameof(AmountSpent));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public List<TaxModel>? Taxes { get; set; }
    public string? Reason { get; set; }
    public bool IncludeInReport { get; set; } = true;
    public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDateTime { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Category { get; set; }
    public string? PlatformModel { get; set; }
    public bool IsDeleted { get; set; }
    public string? UserId { get; set; }

    public string Description { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}