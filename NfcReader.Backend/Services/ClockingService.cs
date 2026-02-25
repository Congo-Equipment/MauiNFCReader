using Microsoft.EntityFrameworkCore;
using NfcReader.Backend.Contexts;
using NfcReader.Backend.DTOs;
using NfcReader.Backend.Models;
using NfcReader.Backend.Services.Interfaces;
using NfcReader.Shared;
using System.Collections;

namespace NfcReader.Backend.Services
{
    internal class ClockingService(ApplicationDbContext dbContext) : IClockingService
    {
        public async ValueTask<Response<EmployeeDTO>> GetInfoFromBadgeAsync(string badgeId)
        {
            try
            {
                var badgeInfo = await dbContext.Recordings.FirstOrDefaultAsync(x => x.BadgeId == badgeId);
                if (badgeInfo is null)
                {
                    return new Response<EmployeeDTO>
                    {
                        Success = false,
                        Message = "Badge information not found or registred"
                    };
                }

                var emp = await dbContext.Employees.FirstOrDefaultAsync(x => x.StaffId == badgeInfo.StaffId);
                if (emp is null)
                {
                    return new Response<EmployeeDTO>
                    {
                        Success = false,
                        Message = "Employee not found or registred with this badge id"
                    };
                }

                var mapped = new EmployeeDTO
                {
                    Id = emp.Id,
                    Name = emp.Names,
                    Surname = emp.Surnames,
                    StaffId = emp.StaffId,
                    Department = emp.Department,
                    Position = emp.Position,
                    badgeId = badgeInfo.BadgeId,
                };

                return new Response<EmployeeDTO>
                {
                    Success = true,
                    Data = mapped
                };
            }
            catch (Exception ex)
            {
                return new Response<EmployeeDTO>
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

                var doesExist = await dbContext.Employees.AnyAsync(x => x.StaffId == recording.StaffId);
                if (!doesExist)
                {
                    return new Response<string>
                    {
                        Success = false,
                        Message = $"Failed to update badge id for {recording.StaffId}, employee not found!"
                    };
                }

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

                    ///Save only if the employee exist, otherwise skip and add to the result as not synced

                    var doesExist = await dbContext.Employees.AnyAsync(x => x.StaffId == record.StaffId);
                    if (doesExist)
                    {

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
                    else
                    {
                        syncResults.Add(new()
                        {
                            StaffId = record.StaffId,
                            BadgeId = record.BadgeId,
                            Synced = false
                        });
                    }
                }

                //await dbContext.Recordings.AddRangeAsync(records);
                //await dbContext.SaveChangesAsync();

                return new Response<IEnumerable<SyncResult>>
                {
                    Success = true,
                    Message = $"{syncResults.Count(x => x.Synced)} records synced successfully!",
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

                if (employee is null)
                {
                    entry.State = EntityState.Deleted;

                    await dbContext.SaveChangesAsync();

                    return new Response<RawClocking>
                    {
                        Success = false,
                        Message = "Employee not found, can't clock today!"
                    };
                }

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
            await foreach (var record in dbContext.Recordings.OrderByDescending(x => x.Created).AsAsyncEnumerable())
            {
                yield return record;
            }
        }

        public async ValueTask<Response<Recording>> CanClockInAsync(string badgeId)
        {
            try
            {
                var recording = await dbContext.Recordings.FirstOrDefaultAsync(x => x.BadgeId == badgeId);
                if (recording is null)
                {
                    return new Response<Recording>()
                    {
                        Success = false,
                        Message = "Badge information not found or registred"
                    };
                }
                var emp = await dbContext.Employees.FirstOrDefaultAsync(x => x.StaffId == recording.StaffId);
                if (emp is null)
                {
                    return new Response<Recording>()
                    {
                        Success = false,
                        Message = "Employee not found or registred with this badge id"
                    };
                }
                return new Response<Recording>()
                {
                    Success = true,
                    Data = recording
                };
            }
            catch (Exception ex)
            {
                return new Response<Recording>()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Return list of clocking types
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<ClockingTypeDTO> RawClockings()
        {
            await foreach (var type in dbContext.ClockingTypes.OrderByDescending(x => x.Created).AsAsyncEnumerable())
            {
                yield return new ClockingTypeDTO
                {
                    Id = type.Id,
                    Name = type.Name,
                    Description = type.Description,
                    Created = type.Created,
                    Updated = type.Updated
                };
            }
        }

        public async Task AddTypeAsync(AddTypeDTO type)
        {
            var clockingType = new ClockingType
            {
                Name = type.Name,
                Description = type.Description,
                Created = DateTime.UtcNow
            };
            await dbContext.ClockingTypes.AddAsync(clockingType);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteTypeAsync(Guid id)
        {
            var deleted = await dbContext.ClockingTypes.Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            return deleted > 0;
        }
    }
}
