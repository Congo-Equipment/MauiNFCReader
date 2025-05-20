using NfcReader.Models;
using NfcReader.Shared;
using Refit;

namespace NfcReader.Services.Interfaces;

public interface IApiService
{
    [Post("/clocking/save-raw")]
    Task<ApiResponse<Response<RawClocking>>> SaveRawClocking([Body] RawClocking clocking);

    [Post("/clocking/save-recording")]
    Task<ApiResponse<string>> SaveRecording([Body] Recording recording);

    [Post("/clocking/sync-badges")]
    Task<ApiResponse<bool>> SyncBadges([Body] List<Recording> badges);

    [Get("/clocking/employee-info/{badgeId}")]
    Task<ApiResponse<Employee>> GetEmployeeInfo(string badgeId);
}
