using NfcReader.Models.Enums;

namespace NfcReader.Models;

public class Clocking
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string? BadgeId { get; set; }
    public string? StaffId { get; set; }
    public DateTime ClockingTime { get; set; }
    public DateTime Created { get; set; }
    public ClockingKind ClockingKind { get; set; }
}
