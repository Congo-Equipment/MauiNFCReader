using NfcReader.Backend.Models;
using NfcReader.Shared;

namespace NfcReader.Backend.Services.Interfaces
{
    public interface IClockingService
    {
        ValueTask<Response<Employee>> GetInfoFromBadgeAsync(string badgeId);
        ValueTask<Response<Employee>> GetInfoFromStaffIdAsync(string staffId);
        ValueTask<Response<string>> SaveBadgeIdAsync(Recording recording);
        ValueTask<Response<RawClocking>> SaveClockingAsync(RawClocking clocking);
    }
}
