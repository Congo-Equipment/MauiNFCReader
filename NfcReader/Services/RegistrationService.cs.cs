using NfcReader.Models;
using NfcReader.Services.Interfaces;

namespace NfcReader.Services
{
    internal class RegistrationService : IRegistrationService
    {
        public ValueTask<bool> HasBeenRecorded(string badgeId, string staffId)
        {
            throw new NotImplementedException();
        }

        public ValueTask SaveAndSync(Recording recording)
        {
            throw new NotImplementedException();
        }

        public ValueTask SaveAndSync(RawClocking clocking)
        {
            throw new NotImplementedException();
        }
    }
}
