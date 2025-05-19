namespace NfcReader.Models;

public class Employee
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? badgeId { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }

}
