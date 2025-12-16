using NfcReader.Models;
using NfcReader.Shared;

namespace NfcReader.Services.Interfaces
{
    public interface IClockingService
    {
        Task<Response<Employee>> GetOrFetchEmployeeInfoAsync(string badgeId, CancellationToken cancellationToken = default);
        Task<bool> HasClockedInAsync(string badgeId, CancellationToken cancellationToken = default);
        Task<Response<string>> SaveClockingAsync(string badgeId, string staffId, CancellationToken cancellationToken = default);
        Task<Response<string>> SaveClockingAsync(Clocking clocking, CancellationToken cancellationToken = default);
        Task<int> TodayClockingAsync(CancellationToken cancellationToken = default);
    }
}
