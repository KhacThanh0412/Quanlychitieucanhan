using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Views
{
    public class PageAttachedProperties
    {
        public static readonly BindableProperty IsDisableBackButtonProperty = BindableProperty.CreateAttached("IsDisableBackButton", typeof(bool), typeof(Page), false);

        public static bool GetIsDisableBackButton(BindableObject page)
        {
            if (page == null)
                return false;
            return (bool)page.GetValue(IsDisableBackButtonProperty);
        }

        public static void SetIsDisableBackButton(BindableObject page, bool value)
        {
            page.SetValue(IsDisableBackButtonProperty, value);
        }
    }
}
