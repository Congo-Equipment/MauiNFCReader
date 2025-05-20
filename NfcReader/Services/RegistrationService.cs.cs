using LiteDB.Async;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using NfcReader.Shared;
using NfcReader.Utils;
using System.Diagnostics;

namespace NfcReader.Services
{
    internal class RegistrationService(IApiService apiService) : IRegistrationService
    {
        public async ValueTask<IReadOnlyCollection<Recording>> GetLocalRecordings()
        {
            try
            {
                using var db = new LiteDatabaseAsync($"Filename={Constants.DB_PATH};Connection=shared");
                var collection = db.GetCollection<Recording>(nameof(Recording));

                // Check if the recording already exists
                var existingRecordings = await collection
                    .Query()
                    .ToListAsync();

                return existingRecordings;
            }
            catch (Exception ex)
            {
                Debug.Print($"Error retreiving data: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return [];
            }
        }

        public ValueTask<bool> HasBeenRecorded(string badgeId, string staffId)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<bool> SaveAndSync(Recording recording)
        {
            try
            {
                using var db = new LiteDatabaseAsync($"Filename={Constants.DB_PATH};Connection=shared");
                var collection = db.GetCollection<Recording>(nameof(Recording));

                // Check if the recording already exists
                var existingRecording = await collection
                    .Query()
                    .Where(x => x.BadgeId == recording.BadgeId && x.StaffId == recording.StaffId)
                    .FirstOrDefaultAsync();
                if (existingRecording != null) return false;

                //check if the record exist using the badgeId
                var byBadgeId = await collection
                    .Query()
                    .Where(x => x.BadgeId == recording.BadgeId)
                    .FirstOrDefaultAsync();
                if (byBadgeId != null) return false;

                // Insert the new recording
                var result = await collection.InsertAsync(recording);

                //TODO: need to call the backend api.
                var response = await apiService.SaveRecording(recording);
                if (response.IsSuccessStatusCode)
                {
                    // If the API call was successful, delete the local recording
                    Debug.Print($"API call failed: {response.ReasonPhrase}");
                }
                else
                {
                    // If the API call failed, keep the local recording for later sync
                    Debug.Print($"API call failed: {response.ReasonPhrase}");
                }

                return true; // Simulate a successful save and sync operation
            }
            catch (Exception ex)
            {
                Debug.Print($"Error saving and syncing recording: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async ValueTask<Response<IEnumerable<SyncResult>>> SaveAndSync()
        {
            try
            {
                using var db = new LiteDatabaseAsync($"Filename={Constants.DB_PATH};Connection=shared");
                var collection = db.GetCollection<Recording>(nameof(Recording));

                var recordings = await collection
                       .Query()
                       .Where(x => x.IsSynced == false)
                       .ToListAsync();

                if (recordings.Any())
                {
                    var apiResult = await apiService.SyncBadges(recordings.ToList());

                    if (!apiResult.IsSuccessStatusCode)
                    {
                        return new Response<IEnumerable<SyncResult>>
                        {
                            Success = false,
                            Message = "Failed to sync with the serveur"
                        };
                    }

                    return new Response<IEnumerable<SyncResult>>
                    {
                        Success = true,
                        Message = "Sync successful"
                    };
                }

                return new Response<IEnumerable<SyncResult>>
                {
                    Success = false,
                    Message = "Nothing to synchronize"
                };
            }
            catch (Exception ex)
            {
                Debug.Print($"Error saving and syncing recording: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return new Response<IEnumerable<SyncResult>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async ValueTask<Response<string>> SaveAndSync(RawClocking clocking)
        {
            try
            {
                using var db = new LiteDatabaseAsync($"Filename={Constants.DB_PATH};Connection=shared");
                var collection = db.GetCollection<RawClocking>(nameof(RawClocking));

                await collection.InsertAsync(clocking);

                var api = await apiService.SaveRawClocking(clocking);
                if (!api.IsSuccessStatusCode)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Failed to sync with the serveur"
                    };
                }

                return new Response<string>
                {
                    Success = true,
                    Message = "Sync successful"
                };
            }
            catch (Exception ex)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

    }
}
