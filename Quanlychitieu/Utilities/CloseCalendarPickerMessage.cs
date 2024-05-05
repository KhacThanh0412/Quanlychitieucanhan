using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Quanlychitieu.Utilities;

public class CloseCalendarPickerMessage : ValueChangedMessage<string>
{
    public CloseCalendarPickerMessage(string value) : base(value) { }
}
