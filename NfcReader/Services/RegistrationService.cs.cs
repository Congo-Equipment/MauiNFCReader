using NfcReader.Models;
using NfcReader.Services.Interfaces;
using System.Diagnostics;

namespace NfcReader.Services
{
    internal class RegistrationService : IRegistrationService
    {
        public ValueTask<IReadOnlyCollection<Recording>> GetLocalRecordings()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> HasBeenRecorded(string badgeId, string staffId)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<bool> SaveAndSync(Recording recording)
        {
            try
            {
                return true; // Simulate a successful save and sync operation
            }
            catch (Exception ex)
            {
                Debug.Print($"Error saving and syncing recording: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public ValueTask<bool> SaveAndSync(RawClocking clocking)
        {
            throw new NotImplementedException();
        }
    }
}
