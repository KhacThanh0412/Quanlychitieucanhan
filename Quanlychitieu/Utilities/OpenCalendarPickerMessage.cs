using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Quanlychitieu.Utilities;

public class OpenCalendarPickerMessage : ValueChangedMessage<string>
{
    public OpenCalendarPickerMessage(string value) : base(value) { }
}
