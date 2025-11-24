using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NfcReader.Backend.Models
{
    public class ClockingType
    {
        public ClockingType()
        {
            RawClockings = new HashSet<RawClocking>();
        }

        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RawClocking> RawClockings { get; set; }
    }

    public class ClockingTypeEntityTypeConfiguration : IEntityTypeConfiguration<ClockingType>
    {
        public void Configure(EntityTypeBuilder<ClockingType> builder)
        {
            builder.ToTable("T_CLOCKING_TYPE");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Id)
                .HasColumnName("OID")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("NAME")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasColumnName("DESCRIPTION")
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Created)
                .HasColumnName("CREATED")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("UPDATED")
                .IsRequired();
        }
    }
}
