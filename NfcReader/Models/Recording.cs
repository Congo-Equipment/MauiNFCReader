namespace NfcReader.Models
{
    public class Recording
    {
        public int Id { get; set; }
        public required string BadgeId { get; set; }
        public required DateTime Created { get; set; }
        public required string StaffId { get; set; }
    }
}
