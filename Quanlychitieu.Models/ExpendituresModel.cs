using LiteDB;
using System.ComponentModel;

namespace Quanlychitieu.Models;

public class ExpendituresModel : INotifyPropertyChanged
{
    double amountSpent;
    [BsonId]
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
    public ExpenditureCategory Category { get; set; } = ExpenditureCategory.None;
    public string? PlatformModel { get; set; }
    public bool IsDeleted { get; set; }
    public string? UserId { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public enum ExpenditureCategory
{
    [Description("Học tập")]
    HocTap = 0,

    [Description("Giải trí")]
    GiaiTri = 1,

    [Description("Ăn uống")]
    AnUong = 2,

    [Description("Sức khỏe")]
    SucKhoe = 3,

    [Description("Không có")]
    None = 4,
}

public static class ExpenditureCategoryDescriptions
{
    public static List<string> Descriptions { get; }
    static ExpenditureCategoryDescriptions() => Descriptions = GetEnumDescriptions<ExpenditureCategory>();

    private static List<string>? GetEnumDescriptions<T>()
    {
        Type type = typeof(T);
        if (!type.IsEnum)
        {
            throw new ArgumentException("Type must be an enum");
        }

        var descriptions = new List<string>();
        foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            if (Attribute.IsDefined(field, typeof(DescriptionAttribute)))
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                descriptions.Add(attribute?.Description ?? string.Empty);
            }
        }
        return descriptions;
    }
}