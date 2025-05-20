using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NfcReader.Backend.Models;

public class Employee
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? Names { get; set; }
    public string? Surnames { get; set; }
    public string? StaffId { get; set; }
    public string? badgeId { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }

    public override string ToString()
    {
        return $"{Names} {Surnames}";
    }

}

public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("T_EMPLOYE");

        builder.Property(x => x.Id).HasColumnName("OID");

        builder.Property(x => x.Names).HasColumnName("NOMS").HasMaxLength(50);
        builder.Property(x => x.Surnames).HasColumnName("PRENOMS").HasMaxLength(50);
        builder.Property(x => x.StaffId).HasColumnName("MATRICULE").HasMaxLength(50);
        builder.Property(x => x.badgeId).HasColumnName("BADGEID").HasMaxLength(50);
        builder.Property(x => x.Department).HasColumnName("OIDDEPARTEMENT").HasMaxLength(50);
        builder.Property(x => x.Position).HasColumnName("OIDFONCTION").HasMaxLength(50);
    }
}
