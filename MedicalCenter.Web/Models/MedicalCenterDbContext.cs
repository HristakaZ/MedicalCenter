using Microsoft.EntityFrameworkCore;

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

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyID);

            modelBuilder.Entity<Patient>()
                .HasOne(d => d.Doctor)
                .WithMany(p => p.Patients)
                .HasForeignKey(p => p.DoctorID);

            modelBuilder.Entity<MedicalExamination>()
                .HasOne(me => me.Patient)
                .WithMany(p => p.MedicalExaminations)
                .HasForeignKey(me => me.PatientID);

            modelBuilder.Entity<MedicalExamination>()
                .HasOne(me => me.Doctor)
                .WithMany(p => p.MedicalExaminations)
                .HasForeignKey(me => me.DoctorID);

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(Specialty)))
            {
                modelBuilder.Entity<Specialty>().HasData(
                    new Specialty { ID = 1, Description = "Ophthalmologist" }
                );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(Role)))
            {
                modelBuilder.Entity<Role>().HasData(
                    new Role { ID = 1, Description = "Patient" },
                    new Role { ID = 2, Description = "Doctor" },
                    new Role { ID = 3, Description = "Administrator" }
                );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(User)))
            {
                modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        ID = 1,
                        Email = "Patient123@gmail.com",
                        Password = "/iLgcY4bH4mID2rF4jeG0ZtIBUW84oWOuGfF7GqZzrCvGHGq1R/AswPwFwIrHnLX+XbzWcX+rbhYj5CDuzP+xw==", //raw Test123/
                        Name = "John",
                        Surname = "Smith",
                        RoleID = 1
                    },
                    new User
                    {
                        ID = 2,
                        Email = "Doctor456@gmail.com",
                        Password = "x0bPjSO2rxdVaDzOG9C2fSygAYtFlPekH/UKNgJ1WsMVfD+dzTwYev2/sKA7oNQZ9sh4m0D6m8sS+cF/Qdorww==", //raw Test456!
                        Name = "Joe",
                        Surname = "Green",
                        RoleID = 2
                    },
                    new User
                    {
                        ID = 3,
                        Email = "Administrator789@gmail.com",
                        Password = "63QCHXTudjyoCjWA6wXjgkcowt2MCwNDnZD2AFf3+dY8N6qXVulIxW4p7Ucyrwh3otzPRSJLJMhZtt2VWYda2w==", //raw Test789?
                        Name = "Jay",
                        Surname = "Blue",
                        RoleID = 3
                    }
                );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(Doctor)))
            {
                modelBuilder.Entity<Doctor>().HasData(
                    new Doctor
                    {
                        ID = 2, // Matches the User ID for the doctor
                        Email = "Doctor456@gmail.com",
                        Password = "x0bPjSO2rxdVaDzOG9C2fSygAYtFlPekH/UKNgJ1WsMVfD+dzTwYev2/sKA7oNQZ9sh4m0D6m8sS+cF/Qdorww==",
                        Name = "Joe",
                        Surname = "Green",
                        RoleID = 2,
                        Room = 101,
                        SpecialtyID = 1
                    }
                );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(Patient)))
            {
                modelBuilder.Entity<Patient>().HasData(
                    new Patient
                    {
                        ID = 1, // Matches the User ID for the patient
                        SSN = 0725191800,
                        PhoneNumber = "+359891234567",
                        DoctorID = 1,
                        Email = "Patient123@gmail.com",
                        Password = "/iLgcY4bH4mID2rF4jeG0ZtIBUW84oWOuGfF7GqZzrCvGHGq1R/AswPwFwIrHnLX+XbzWcX+rbhYj5CDuzP+xw==",
                        Name = "John",
                        Surname = "Smith",
                        RoleID = 1
                    }
                );
            }
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