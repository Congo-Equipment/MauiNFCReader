using CommunityToolkit.Maui.ApplicationModel;
using Microsoft.EntityFrameworkCore;
using NfcReader.Contexts;
using NfcReader.Models;
using NfcReader.Models.Enums;
using NfcReader.Services.Interfaces;
using NfcReader.Shared;
using System.Diagnostics;

namespace NfcReader.Services
{
    internal class ClockingService(ApplicationDbContext context, IApiService apiService) : IClockingService
    {
        public async Task<Response<string>> SaveClockingAsync(string badgeId, string staffId, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await apiService.GetEmployeeInfo(badgeId);
                if (!employee.IsSuccessful)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Employee not found."
                    };
                }


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

        public async Task<Response<string>> SaveClockingAsync(Clocking clocking, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await apiService.GetEmployeeInfo(clocking.BadgeId);
                if (!employee.IsSuccessful)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "Employee not found."
                    };
                }


                var hasPunchedIn = await context
                    .Clockings
                    .AnyAsync(c => c.StaffId == clocking.StaffId &&
                    c.ClockingKind == ClockingKind.Meeting &&
                    c.ClockingTime.Date == DateTime.UtcNow.Date &&
                    c.ClockingTypeId == clocking.ClockingTypeId);
                if (hasPunchedIn)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = "You have already clocked in for a meeting today.",
                    };
                }

                var en = await context.Clockings.AddAsync(clocking, cancellationToken);

                var raw = new RawClocking
                {
                    BadgeId = clocking.BadgeId,
                    ClockingTime = clocking.ClockingTime,
                    StaffId = clocking.StaffId,
                    ClockingTypeId = clocking.ClockingTypeId
                };

                var api = await apiService.SaveRawClocking(raw);
                if (!api.IsSuccessStatusCode)
                {


                    return new Response<string>
                    {
                        Success = false,
                        Message = "Failed to sync with the serveur"
                    };
                }

                var result = api.Content;

                en.Entity.Created = result?.Data?.Created ?? DateTime.Now;

                //var resut = await collection.UpdateAsync(clocking);s
                return new Response<string>
                {
                    Success = true,
                    Message = result?.Message
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
            try
            {
                return await context
                       .Clockings
                       .AnyAsync(c => c.BadgeId == badgeId &&
                                   c.ClockingKind == ClockingKind.Meeting &&
                                   c.ClockingTime.Date == DateTime.UtcNow.Date);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "MEETING-CLOCKING:");
                return false;
            }
        }

        public async Task<Response<Employee>> GetOrFetchEmployeeInfoAsync(string badgeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var localEmployee = await context.Employees.FirstOrDefaultAsync(e => e.StaffId == badgeId, cancellationToken);
                if (localEmployee != null)
                {
                    return new Response<Employee>
                    {
                        Success = true,
                        Message = "Success",
                        Data = localEmployee
                    };
                }

                var employee = await apiService.GetEmployeeInfo(badgeId);
                if (!employee.IsSuccessful)
                {
                    return new Response<Employee>
                    {
                        Success = false,
                        Message = "Employee not found."
                    };
                }

                await context.Employees.AddAsync(employee.Content, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                return new Response<Employee>
                {
                    Success = true,
                    Message = "Success",
                    Data = employee.Content
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new Response<Employee>
                {
                    Success = false,
                    Message = "An Error occured while processing the entry",
                };
            }
        }
    }
}
