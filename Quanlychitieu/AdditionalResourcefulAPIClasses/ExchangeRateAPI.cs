using Newtonsoft.Json;

namespace Quanlychitieu.AdditionalResourcefulApiClasses;

public class ExchangeRateAPI
{
    public ConvertedRate GetConvertedRate(string UserCurrency, string DestinationCurrency)
    {
        
        try
        {            
            string url_str = $@"https://v6.exchangerate-api.com/v6/f0bfc8e59756851a938ce5cc/pair/{UserCurrency}/{DestinationCurrency}";
            using var webClient = new HttpClient();
            var json = webClient.GetStringAsync(url_str).Result;
            ConvertedRate JsonObject = JsonConvert.DeserializeObject<ConvertedRate>(json);

            return JsonObject;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }

    public class ConvertedRate
    {
        public string success { get; set; }
        public string result { get; set; }
        public float conversion_rate { get; set; }

        private string time_last_update_utc;
        private string _formattedTime;

        public string TimeLastUpdateUtc
        {
            get => _formattedTime;
            set
            {
                time_last_update_utc = value;
                _formattedTime = ConvertToFormattedDateTime(value);
            }
        }
        
        private string ConvertToFormattedDateTime(string utcDateTime)
        {
            if(DateTime.TryParseExact(utcDateTime, "ddd, dd MMM yyyy HH:mm:ss '+0000'",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.AdjustToUniversal,
                                   out DateTime parsedDateTime))
            {
                return parsedDateTime.ToLocalTime().ToString("ddd, dd MM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }

            return "Invalid Date";
        }

        
    }


    public class ConversionRate
    {
        public double VND { get; set; } // Vietnamese Dong
    }
}