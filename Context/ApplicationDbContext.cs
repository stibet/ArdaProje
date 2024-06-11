using Microsoft.EntityFrameworkCore;
using ArdaProje.Models;

namespace ArdaProje.Context
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }  
        
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PT> PTs { get; set; }
        public DbSet<ClosedSlots> ClosedSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.AppointmentDateTime)
                      .HasColumnType("datetime2"); // MSSQL için datetime2 olarak tanımla
            });

            // Diğer modeller için konfigürasyonlar
        }
    }
}
