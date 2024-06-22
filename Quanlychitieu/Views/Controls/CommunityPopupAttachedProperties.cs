using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Views.Controls
{
    public class CommunityPopupAttachedProperties
    {
        public static readonly BindableProperty FillScreenPercentProperty = BindableProperty.CreateAttached("FillScreenPercent", typeof(Size), typeof(Popup), new Size(0, 0));

        public static Size GetFillScreenPercent(BindableObject popup)
        {
            return (Size)popup.GetValue(FillScreenPercentProperty);
        }

        public static void SetFillScreenPercent(BindableObject popup, Size value)
        {
            popup.SetValue(FillScreenPercentProperty, value);
        }
    }
}
