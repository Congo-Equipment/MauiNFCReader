using Microsoft.AspNetCore.Mvc;
using NfcReader.Backend.DTOs;
using NfcReader.Backend.Models;
using NfcReader.Backend.Services.Interfaces;

namespace NfcReader.Backend.Controllers
{
    [Route("api/clocking")]
    [ApiController]
    public class ClockingController(IClockingService clockingService) : ControllerBase
    {

        [HttpPost("save-raw")]
        public async Task<IActionResult> SaveClockingAsync([FromBody] RawClocking clocking)
            => Ok(await clockingService.SaveClockingAsync(clocking));

        [HttpPost("save-recording")]
        public async Task<IActionResult> SaveRecordingAsync([FromBody] Recording recording)
            => Ok(await clockingService.SaveBadgeIdAsync(recording));

        [HttpGet("employee-info/{badgeId}")]
        public async Task<IActionResult> GetEmployeeInfo(string badgeId)
        {
            var result = await clockingService.GetInfoFromBadgeAsync(badgeId);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("sync-badges")]
        public async Task<IActionResult> SyncBadgesAsync([FromBody] IEnumerable<Recording> records)
        {
            var result = await clockingService.SyncBadgesAsync(records);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("recordings")]
        public async IAsyncEnumerable<Recording> GetRecordings()
        {
            await foreach (var item in clockingService.Recordings())
            {
                yield return item;
            }
        }

        [HttpGet("clocking-types")]
        public async IAsyncEnumerable<ClockingTypeDTO> GetClockingTypes()
        {
            await foreach (var item in clockingService.RawClockings())
            {
                yield return item;
            }
        }

        [HttpPost("add-clock-type")]
        public async Task<IActionResult> AddClockingTypeAsync([FromBody] AddTypeDTO clockingType)
        {
            await clockingService.AddTypeAsync(clockingType);
            return Ok(new { Success = true, Message = "Clocking type added successfully." });
        }

        [HttpDelete("delete-clock-type/{id}")]
        public async Task<IActionResult> DeleteClockingTypeAsync(Guid id)
        {
            var success = await clockingService.DeleteTypeAsync(id);
            if (success)
                return Ok(new { Success = true, Message = "Clocking type deleted successfully." });

            return BadRequest(new { Success = false, Message = "Failed to delete clocking type." });
        }
    }
}
