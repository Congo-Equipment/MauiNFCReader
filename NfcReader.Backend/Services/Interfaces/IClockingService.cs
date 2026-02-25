using NfcReader.Backend.DTOs;
using NfcReader.Backend.Models;
using NfcReader.Shared;

namespace NfcReader.Backend.Services.Interfaces
{
    public interface IClockingService
    {
        Task AddTypeAsync(AddTypeDTO type);
        ValueTask<Response<Recording>> CanClockInAsync(string badgeId);
        Task<bool> DeleteTypeAsync(Guid id);
        ValueTask<Response<EmployeeDTO>> GetInfoFromBadgeAsync(string badgeId);
        ValueTask<Response<Employee>> GetInfoFromStaffIdAsync(string staffId);
        IAsyncEnumerable<ClockingTypeDTO> RawClockings();
        IAsyncEnumerable<Recording> Recordings();
        ValueTask<Response<string>> SaveBadgeIdAsync(Recording recording);
        ValueTask<Response<RawClocking>> SaveClockingAsync(RawClocking clocking);
        ValueTask<Response<IEnumerable<SyncResult>>> SyncBadgesAsync(IEnumerable<Recording> records);
    }
}
