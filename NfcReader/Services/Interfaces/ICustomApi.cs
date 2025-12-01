using NfcReader.Models;

namespace NfcReader.Services.Interfaces
{
    public interface ICustomApi
    {
        IAsyncEnumerable<ClockingType?> GetClockingTypesAsync();
        IAsyncEnumerable<Recording?> GetRecordings();
    }
}
