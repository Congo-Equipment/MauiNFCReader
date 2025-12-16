namespace NfcReader.Backend.DTOs
{
    public class EmployeeDTO
    {
        public  required string Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? badgeId { get; set; }
        public string? StaffId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
    }
}
