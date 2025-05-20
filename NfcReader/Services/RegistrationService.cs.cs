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


        public async ValueTask<IReadOnlyCollection<RawClocking>> GetLocalClockings()
        {
            try
            {
                using var db = new LiteDatabaseAsync($"Filename={Constants.DB_PATH};Connection=shared");
                var collection = db.GetCollection<RawClocking>(nameof(RawClocking));

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
                        var content = apiResult.Content;
                        return new Response<IEnumerable<SyncResult>>
                        {
                            Success = false,
                            Message = "Failed to sync with the serveur"
                        };
                    }

                    foreach (var update in apiResult.Content.Data)
                    {
                        var recording = await collection
                            .Query()
                            .Where(x => x.BadgeId == update.BadgeId && x.StaffId == update.StaffId)
                            .FirstOrDefaultAsync();
                        if (recording is not null)
                        {
                            recording.IsSynced = true;
                            await collection.UpdateAsync(recording);
                        }
                    }

                    return new Response<IEnumerable<SyncResult>>
                    {
                        Success = true,
                        Message = apiResult.Content.Message
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
                var recordings = db.GetCollection<Recording>(nameof(Recording));

                var hasClocked = await collection
                    .Query()
                    .Where(x => x.BadgeId == clocking.BadgeId && x.Created.Date == DateTime.Now.Date)
                    .CountAsync();
                if (hasClocked > 0)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Already clocked today"
                    };
                }

                var get = await recordings
                    .Query()
                    .Where(x => x.BadgeId == clocking.BadgeId)
                    .FirstOrDefaultAsync();


                if (get is null) return new Response<string>
                {
                    Success = false,
                    Message = "Does not exist"
                };

                clocking.StaffId = get?.StaffId;

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

                var result = api.Content;

                clocking.Created = result?.Data?.Created ?? DateTime.Now;

                var resut = await collection.UpdateAsync(clocking);
                return new Response<string>
                {
                    Success = true,
                    Message = result?.Message
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

        public async ValueTask<Response<string>> ClearData()
        {
            try
            {
                using var db = new LiteDatabaseAsync($"Filename={Constants.DB_PATH};Connection=shared");
                var recordings = db.GetCollection<Recording>(nameof(Recording));
                var rawClockings = db.GetCollection<RawClocking>(nameof(RawClocking));

                // Clear all data from the database
                await recordings.DeleteAllAsync();
                await rawClockings.DeleteAllAsync();
                return new Response<string>
                {
                    Success = true,
                    Message = "Data cleared successfully"
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
