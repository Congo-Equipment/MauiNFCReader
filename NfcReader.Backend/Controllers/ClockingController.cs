using Microsoft.AspNetCore.Mvc;
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
    }
}
