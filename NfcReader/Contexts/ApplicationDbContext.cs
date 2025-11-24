using Microsoft.EntityFrameworkCore;
using NfcReader.Models;

namespace NfcReader.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Clocking> Clockings { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clocking>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Clocking>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Clocking>()
                .Property(x => x.Created)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Clocking>()
                .Property(x => x.StaffId)
                .IsRequired();

            modelBuilder.Entity<Employee>(employee =>
            {
                employee.HasKey(x => x.Id);
                employee.HasIndex(x => x.badgeId).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
