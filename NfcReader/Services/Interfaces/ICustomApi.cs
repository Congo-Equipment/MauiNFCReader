using NfcReader.Models;

namespace NfcReader.Services.Interfaces
{
    public interface ICustomApi
    {
        IAsyncEnumerable<Recording?> GetRecordings();
    }
}
