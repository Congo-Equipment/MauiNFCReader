using NfcReader.Models;
using NfcReader.Shared;

namespace NfcReader.Services.Interfaces
{
    public interface ICustomApi
    {
        Task<Response<Employee>> GetByIdAsync(string badgeId);
        IAsyncEnumerable<ClockingType?> GetClockingTypesAsync();
        IAsyncEnumerable<Recording?> GetRecordings();
    }
}
