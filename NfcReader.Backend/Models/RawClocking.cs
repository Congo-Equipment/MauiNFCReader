using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NfcReader.Backend.Models
{
    public class RawClocking
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string? BadgeId { get; set; }
        public string? StaffId { get; set; }
        public DateTime ClockingTime { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class RawClockingEntityTypeConfiguration : IEntityTypeConfiguration<RawClocking>
    {
        public void Configure(EntityTypeBuilder<RawClocking> builder)
        {
            builder.ToTable("T_POINTAGE_TRAV_MOBILE");

            builder.HasKey(x => x.Id);


            builder.Property(x => x.Id).HasColumnName("OID");
            builder.Property(x => x.BadgeId).HasColumnName("BADGEID").HasMaxLength(50);
            builder.Property(x => x.StaffId).HasColumnName("OID_EMPLOYE").HasMaxLength(50);
            builder.Property(x => x.ClockingTime).HasColumnName("DATE_HEURE_POINTAGE").HasMaxLength(50);
            builder.Property(x => x.Created).HasColumnName("CREATED").HasMaxLength(50);
        }
    }
}
