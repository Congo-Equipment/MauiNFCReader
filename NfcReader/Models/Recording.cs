namespace NfcReader.Models
{
    public class Recording
    {
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        public required string BadgeId { get; set; }
        public required DateTime Created { get; set; }
        public required string StaffId { get; set; }
    }
}
