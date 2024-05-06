namespace Quanlychitieu.AdditionalResourcefulAPIClasses;

public class CountryAndCurrencyCodes
{
    Dictionary<string, string> CountryAndCurrency;
    readonly List<string> CountryNames = new();

    public Dictionary<string, string> LoadDictionaryWithCountryAndCurrency()
    {
        CountryAndCurrency = new()
        {
            { "Vietnam", "VND" }
        };
        return CountryAndCurrency;
    }

    public List<string> GetCountryNames()
    {
        var dict = LoadDictionaryWithCountryAndCurrency();

        foreach (var item in dict)
        {
            CountryNames.Add(item.Key);
        }
        return CountryNames;
    }
}
