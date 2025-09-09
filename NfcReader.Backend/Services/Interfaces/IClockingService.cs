using NfcReader.Backend.Models;
using NfcReader.Shared;

namespace NfcReader.Backend.Services.Interfaces
{
    public interface IClockingService
    {
        ValueTask<Response<Employee>> GetInfoFromBadgeAsync(string badgeId);
        ValueTask<Response<Employee>> GetInfoFromStaffIdAsync(string staffId);
        IAsyncEnumerable<Recording> Recordings();
        ValueTask<Response<string>> SaveBadgeIdAsync(Recording recording);
        ValueTask<Response<RawClocking>> SaveClockingAsync(RawClocking clocking);
        ValueTask<Response<IEnumerable<SyncResult>>> SyncBadgesAsync(IEnumerable<Recording> records);
    }
}
