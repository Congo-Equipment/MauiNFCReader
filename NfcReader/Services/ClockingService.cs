using Microsoft.EntityFrameworkCore;
using NfcReader.Contexts;
using NfcReader.Models;
using NfcReader.Models.Enums;
using NfcReader.Services.Interfaces;
using NfcReader.Shared;
using System.Diagnostics;

namespace NfcReader.Services
{
    internal class ClockingService(ApplicationDbContext context) : IClockingService
    {
        public async Task<Response<string>> SaveClockingAsync(string badgeId, string staffId, CancellationToken cancellationToken = default)
        {
            try
            {
                var hasPunchedIn = await context
                    .Clockings
                    .AnyAsync(c => c.StaffId == staffId && c.ClockingKind == ClockingKind.Meeting && c.ClockingTime.Date == DateTime.UtcNow.Date);
                if (hasPunchedIn)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "You have already clocked in for a meeting today.",
                    };
                }

                var meetingPunching = new Clocking
                {
                    ClockingKind = ClockingKind.Meeting,
                    BadgeId = badgeId,
                    StaffId = staffId
                };

                await context.Clockings.AddAsync(meetingPunching);
                await context.SaveChangesAsync(cancellationToken);

                //TODO: Sync to server

                return new Response<string>
                {
                    Success = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new Response<string>
                {
                    Success = false,
                    Message = "An Error occured while processing the entry",
                };
            }
        }

        public async Task<int> TodayClockingAsync(CancellationToken cancellationToken = default)
        {
            return await context.Clockings.CountAsync(c => c.ClockingKind == ClockingKind.Meeting && c.ClockingTime.Date == DateTime.UtcNow.Date);
        }

        public async Task<bool> HasClockedInAsync(string badgeId, CancellationToken cancellationToken = default)
        {
            return await context
                .Clockings
                .AnyAsync(c => c.BadgeId == badgeId &&
                            c.ClockingKind == ClockingKind.Meeting &&
                            c.ClockingTime.Date == DateTime.UtcNow.Date);
        }
    }
}
