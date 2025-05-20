using NfcReader.Models;
using NfcReader.Shared;

namespace NfcReader.Services.Interfaces
{
    public interface IRegistrationService
    {

        /// <summary>
        /// Check if the cardId has been recorded for the given staffId.
        /// </summary>
        /// <param name="badgeId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        ValueTask<bool> HasBeenRecorded(string badgeId, string staffId);

        /// <summary>
        /// Save the recording to the database and sync it with the server.
        /// this concernt only the cardId
        /// </summary>
        /// <param name="recording"></param>
        /// <returns></returns>
        ValueTask<bool> SaveAndSync(Recording recording);

        /// <summary>
        /// Record the clocking and sync it with the server.
        /// </summary>
        /// <param name="clocking"></param>
        /// <returns></returns>
        ValueTask<Response<string>> SaveAndSync(RawClocking clocking);

        /// <summary>
        /// Return the list of all recorded badgeId(serial Number) and staffId.
        /// </summary>
        /// <returns></returns>
        ValueTask<IReadOnlyCollection<Recording>> GetLocalRecordings();
        ValueTask<Response<IEnumerable<SyncResult>>> SaveAndSync();
    }
}
