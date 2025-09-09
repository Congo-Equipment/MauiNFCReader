﻿using Microsoft.EntityFrameworkCore;
using NfcReader.Backend.Contexts;
using NfcReader.Backend.Models;
using NfcReader.Backend.Services.Interfaces;
using NfcReader.Shared;
using System.Collections;

namespace NfcReader.Backend.Services
{
    internal class ClockingService(ApplicationDbContext dbContext) : IClockingService
    {
        public async ValueTask<Response<Employee>> GetInfoFromBadgeAsync(string badgeId)
        {
            try
            {
                var emp = await dbContext.Employees.FirstOrDefaultAsync(x => x.StaffId == badgeId);
                if (emp is null)
                {
                    return new Response<Employee>
                    {
                        Success = true,
                        Message = "Employee not found or registred with this badge id"
                    };
                }

                return new Response<Employee>
                {
                    Success = true,
                    Data = emp
                };
            }
            catch (Exception ex)
            {
                return new Response<Employee>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }


        public async ValueTask<Response<Employee>> GetInfoFromStaffIdAsync(string staffId)
        {
            try
            {
                var emp = await dbContext.Employees.FirstOrDefaultAsync(x => x.StaffId == staffId);
                if (emp is null)
                {
                    return new Response<Employee>
                    {
                        Success = true,
                        Message = "Employee not found or registred with this badge id"
                    };
                }

                return new Response<Employee>
                {
                    Success = true,
                    Data = emp
                };
            }
            catch (Exception ex)
            {
                return new Response<Employee>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async ValueTask<Response<string>> SaveBadgeIdAsync(Recording recording)
        {
            try
            {
                //var result = await dbContext.Employees.Where(x => x.StaffId == recording.StaffId)
                //     .ExecuteUpdateAsync(x => x.SetProperty(b => b.badgeId, recording.BadgeId));

                //if (result > 0)
                //{

                // Save the recording to the database
                await dbContext.Recordings.AddAsync(recording);
                await dbContext.SaveChangesAsync();

                return new Response<string>
                {
                    Success = true,
                    Message = $"Badge id updated successfully for {recording.StaffId}"
                };
                //}

                //return new Response<string>
                //{
                //    Success = false,
                //    Message = $"Failed to update badge id for {recording.StaffId}"
                //};
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

        public async ValueTask<Response<IEnumerable<SyncResult>>> SyncBadgesAsync(IEnumerable<Recording> records)
        {
            try
            {
                List<SyncResult> syncResults = [];
                foreach (var record in records)
                {
                    //var result = await dbContext.Employees.Where(x => x.StaffId == record.StaffId)
                    //.ExecuteUpdateAsync(x => x.SetProperty(b => b.badgeId, record.BadgeId));


                    //var exist = await dbContext.Recordings.AnyAsync(x => x.Id == record.Id);
                    if (!await dbContext.Recordings.AnyAsync(x => x.Id == record.Id))
                    {
                        var r = await dbContext.Recordings.AddAsync(record);
                        await dbContext.SaveChangesAsync();

                    }

                    syncResults.Add(new()
                    {
                        StaffId = record.StaffId,
                        BadgeId = record.BadgeId,
                        Synced = true
                    });
                }

                //await dbContext.Recordings.AddRangeAsync(records);
                //await dbContext.SaveChangesAsync();

                return new Response<IEnumerable<SyncResult>>
                {
                    Success = true,
                    Message = $"{syncResults.Count} records synced successfully!",
                    Data = syncResults
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<SyncResult>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async ValueTask<Response<RawClocking>> SaveClockingAsync(RawClocking clocking)
        {
            try
            {
                clocking.Created = DateTime.UtcNow;
                var entry = await dbContext.RawClockings.AddAsync(clocking);

                await dbContext.SaveChangesAsync();

                var hasParsed = int.TryParse(clocking.StaffId, out int toInt);

                var employee = hasParsed ? await dbContext.Employees.FirstOrDefaultAsync(x => x.StaffId == toInt.ToString())
                    : await dbContext.Employees.FirstOrDefaultAsync(x => x.StaffId == clocking.StaffId);

                return new Response<RawClocking>
                {
                    Success = true,
                    Message = $"{clocking.StaffId} - {employee} Clocked successfully!",
                    Data = entry.Entity
                };
            }
            catch (Exception ex)
            {
                return new Response<RawClocking>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async IAsyncEnumerable<Recording> Recordings()
        {
            await foreach (var record in dbContext.Recordings.AsAsyncEnumerable())
            {
                yield return record;
            }
        }
    }
}
