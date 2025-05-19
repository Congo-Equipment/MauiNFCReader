using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NfcReader.Models.Messages
{
    public class OpenBottomSheetMessage : ValueChangedMessage<bool>
    {
        public OpenBottomSheetMessage(bool value) : base(value)
        {
        }
    }
}
