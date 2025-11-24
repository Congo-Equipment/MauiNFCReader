using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NfcReader.Backend.Models
{
    public class Recording
    {
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        public required string BadgeId { get; set; }
        public required DateTime Created { get; set; }
        public required string StaffId { get; set; }
    }

    public class RecordingEntityTypeConfiguration : IEntityTypeConfiguration<Recording>
    {
        public void Configure(EntityTypeBuilder<Recording> builder)
        {
            builder.ToTable("T_MOBILE_BADGE_INFO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("OID").ValueGeneratedOnAdd();

            builder.Property(x => x.BadgeId).HasColumnName("BADGE_ID").IsRequired();

            builder.Property(x => x.Created).HasColumnName("CREATED").IsRequired();

            builder.Property(x => x.StaffId).HasColumnName("STAFF_ID").IsRequired();
        }
    }
}
