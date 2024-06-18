using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public class AlertHelper
    {
        public static Task<bool> ShowConfirmationAlertAsync(string message, string title = null)
        {
            return ShowConfirmationAlertAsync(new AlertConfigure
            {
                Message = message,
                Title = title
            });
        }

        public static Task<bool> ShowConfirmationAlertAsync(AlertConfigure alertConfigure)
        {
            return App.Current.MainPage.DisplayAlert(
                           alertConfigure.Title ?? "Xác nhận",
                           alertConfigure.Message,
                           alertConfigure.OK,
                           alertConfigure.Cancel);
        }

        public static Task ShowErrorAlertAsync(string message, string title = null)
        {
            return ShowErrorAlertAsync(new AlertConfigure
            {
                Title = title,
                Message = message
            });
        }

        public static Task<bool> ShowConfirmationAlertFromMopupAsync(AlertConfigure alertConfigure)
        {
            return App.Current.MainPage.DisplayAlert(
                alertConfigure.Title ?? "Xác nhận",
                alertConfigure.Message,
                alertConfigure.OK,
                alertConfigure.Cancel);
        }

        public static Task ShowErrorAlertAsync(AlertConfigure alertConfigure)
        {
            return App.Current.MainPage.DisplayAlert(alertConfigure.Title ?? "Thông báo lỗi", alertConfigure.Message, alertConfigure.OK);
        }

        public static Task ShowWarningAlertAsync(string message, string title = null)
        {
            return ShowWarningAlertAsync(new AlertConfigure
            {
                Message = message,
                Title = title
            });
        }

        public static Task ShowWarningAlertAsync(AlertConfigure alertConfigure)
        {
            return App.Current.MainPage.DisplayAlert(alertConfigure.Title ?? "Cảnh báo", alertConfigure.Message, alertConfigure.OK);
        }

        public static Task ShowInformationAlertAsync(string message, string title = null)
        {
            return ShowInformationAlertAsync(new AlertConfigure
            {
                Message = message,
                Title = title
            });
        }

        public static Task ShowInformationAlertAsync(AlertConfigure alertConfigure)
        {
            return App.Current.MainPage.DisplayAlert(alertConfigure.Title ?? "Xác nhận", alertConfigure.Message, alertConfigure.OK);
        }
    }

    public class AlertConfigure
    {
        public string OK { get; set; } = "OK";
        public string Cancel { get; set; } = "Thoát";
        private string _title;
        public string Title
        {
            get
            {
                if (_title != null)
                    _title = _title.TrimEnd('.');
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string Message { get; set; }
    }
}
