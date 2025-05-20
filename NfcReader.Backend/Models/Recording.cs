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

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.BadgeId).IsRequired();

            builder.Property(x => x.Created).IsRequired();

            builder.Property(x => x.StaffId).IsRequired();
        }
    }
}
