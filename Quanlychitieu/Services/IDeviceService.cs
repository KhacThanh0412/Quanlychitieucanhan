namespace Quanlychitieu.Services
{
    public interface IDeviceService
    {
        void HideKeyboard();
        bool IsSoftKeyboardVisible(Android.Views.View view);
    }
}
