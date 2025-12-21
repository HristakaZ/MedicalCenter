using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MedicalCenter.Web.Dtos.MedicalExamination;

namespace MedicalCenter.Web.Models
{
    public class MedicalCenterDbContext : DbContext
    {

        public MedicalCenterDbContext(DbContextOptions<MedicalCenterDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TPT for Users, Doctors, and Patients
            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<Doctor>()
                .ToTable("Doctors");

            modelBuilder.Entity<Patient>()
                .ToTable("Patients");
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<MedicalExamination> MedicalExaminations { get; set; }
        public DbSet<MedicalCenter.Web.Dtos.MedicalExamination.EditMedicalExaminationDto> EditMedicalExaminationDto { get; set; } = default!;
    }
}