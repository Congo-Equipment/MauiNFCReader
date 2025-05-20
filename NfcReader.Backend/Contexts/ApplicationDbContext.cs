using Microsoft.EntityFrameworkCore;
using NfcReader.Backend.Models;

namespace NfcReader.Backend.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        //public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<RawClocking> RawClockings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        }

    }
}
