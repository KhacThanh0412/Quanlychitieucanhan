using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                var vietnameseMonthNames = new[]
                {
                    "tháng Một", "tháng Hai", "tháng Ba", "tháng Tư", "tháng Năm", "tháng Sáu",
                    "tháng Bảy", "tháng Tám", "tháng Chín", "tháng Mười", "tháng Mười Một", "tháng Mười Hai"
                };
                var monthName = vietnameseMonthNames[date.Month - 1];
                return $"{date.Day} {monthName} {date.Year}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
