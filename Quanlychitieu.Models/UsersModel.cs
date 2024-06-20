using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Quanlychitieu.Models;

public class UsersModel : INotifyPropertyChanged
{
    private string id;
    private string username;
    private string email;
    private string password;
    private double savings;
    private double pocketMoney;
    private double totalExpendituresAmount;
    private double totalIncomeAmount;
    private double totalInDebtAmount;
    private double totalOutDebtAmount;
    private DateTime dateTimeOfPocketMoneyUpdate;

    public UsersModel(string username, string email, string password)
    {
        this.username = username;
        this.email = email;
        this.password = password;
    }

    [JsonProperty("_id")]
    public string Id
    {
        get => id;
        set => SetProperty(ref id, value);
    }

    public string Username
    {
        get => username;
        set => SetProperty(ref username, value);
    }

    public string Email
    {
        get => email;
        set => SetProperty(ref email, value);
    }

    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    public double Savings
    {
        get => savings;
        set => SetProperty(ref savings, value);
    }

    public double PocketMoney
    {
        get => pocketMoney;
        set => SetProperty(ref pocketMoney, value);
    }

    public double TotalExpendituresAmount
    {
        get => totalExpendituresAmount;
        set => SetProperty(ref totalExpendituresAmount, value);
    }

    public double TotalIncomeAmount
    {
        get => totalIncomeAmount;
        set => SetProperty(ref totalIncomeAmount, value);
    }

    public double TotalInDebtAmount
    {
        get => totalInDebtAmount;
        set => SetProperty(ref totalInDebtAmount, value);
    }

    public double TotalOutDebtAmount
    {
        get => totalOutDebtAmount;
        set => SetProperty(ref totalOutDebtAmount, value);
    }

    public DateTime DateTimeOfPocketMoneyUpdate
    {
        get => dateTimeOfPocketMoneyUpdate;
        set => SetProperty(ref dateTimeOfPocketMoneyUpdate, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}