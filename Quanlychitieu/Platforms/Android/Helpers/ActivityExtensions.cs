using Android.App;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.Generic;
using AView = Android.Views.View;
using Rect = Android.Graphics.Rect;
using View = Microsoft.Maui.Controls.View;


namespace Quanlychitieu.Helpers
{
    public static class ActivityExtensions
    {
        public static List<EditText> GetVisibleEditTexts(AView currentView)
        {
            List<EditText> visibleEditTexts = new List<EditText>();
            FindVisibleEditTexts(currentView, visibleEditTexts);
            return visibleEditTexts;
        }

        private static void FindVisibleEditTexts(AView view, List<EditText> visibleEditTexts)
        {
            if (view is EditText editText)
            {
                visibleEditTexts.Add(editText);
            }

            if (view is ViewGroup viewGroup)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    AView child = viewGroup.GetChildAt(i);
                    FindVisibleEditTexts(child, visibleEditTexts);
                }
            }
        }
    }
}
