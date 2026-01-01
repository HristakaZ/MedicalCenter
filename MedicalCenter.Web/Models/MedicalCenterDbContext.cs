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
                    new Specialty { ID = 1, Description = "Офталмолог" },
                    new Specialty { ID = 2, Description = "Кардиолог" },
                    new Specialty { ID = 3, Description = "Невролог" },
                    new Specialty { ID = 4, Description = "Дерматолог" },
                    new Specialty { ID = 5, Description = "Педиатър" }
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
                    Password = "/iLgcY4bH4mID2rF4jeG0ZtIBUW84oWOuGfF7GqZzrCvGHGq1R/AswPwFwIrHnLX+XbzWcX+rbhYj5CDuzP+xw==", // raw Test123/
                    Name = "Иван",
                    Surname = "Иванов",
                    RoleID = 1
                },
                new User
                {
                    ID = 2,
                    Email = "Doctor456@gmail.com",
                    Password = "x0bPjSO2rxdVaDzOG9C2fSygAYtFlPekH/UKNgJ1WsMVfD+dzTwYev2/sKA7oNQZ9sh4m0D6m8sS+cF/Qdorww==", // raw Test456!
                    Name = "Георги",
                    Surname = "Георгиев",
                    RoleID = 2
                },
                new User
                {
                    ID = 3,
                    Email = "Administrator789@gmail.com",
                    Password = "63QCHXTudjyoCjWA6wXjgkcowt2MCwNDnZD2AFf3+dY8N6qXVulIxW4p7Ucyrwh3otzPRSJLJMhZtt2VWYda2w==", // raw Test789?
                    Name = "Димитър",
                    Surname = "Димитров",
                    RoleID = 3
                },
                new User
                {
                    ID = 4,
                    Email = "DoctorBG@gmail.com",
                    Password = "ws0tN4IGRQWBBZSU87WEt1RiQCKmkxLOet6pimScLK5YU07QfGLouqMc8mhlIZe4KRD3TUAQnvlFjRXxXWTCbA==", // raw MedDoc123!
                    Name = "Стефан",
                    Surname = "Петров",
                    RoleID = 2
                },
                new User
                {
                    ID = 5,
                    Email = "PatientBG@gmail.com",
                    Password = "xNrdNp5NsCzus3j/shDrfVEui3/PXjlW/bVwi2OqOvPW1YVIKw0iD+MQVc5De+pq+f0f3MD2cWkN2lEqZrjI/A==", // raw MedPat456?
                    Name = "Мария",
                    Surname = "Николова",
                    RoleID = 1
                },
                new User
                {
                    ID = 6,
                    Email = "AdminIvan@gmail.com",
                    Password = "p3ln9clcsffFCYXmkm9XfR2mmyH6XSptR777LC3XinRHPdVMKi0/1ttIZcr+rUC69TrpB3RVuC/MPAbLhnV+xw==", // raw Admin123!
                    Name = "Ивайло",
                    Surname = "Костов",
                    RoleID = 3
                },
                new User
                {
                    ID = 7,
                    Email = "AdminPetar@gmail.com",
                    Password = "F+1NS4CnMKWqh/Ytwpbz+baQ7Zqj8SEZs+LJn/sZ33OaHovTKGwSDJlweC97FnaT96Noh2A/eZAmVZa3TinYFA==", // raw Admin456?
                    Name = "Петър",
                    Surname = "Симеонов",
                    RoleID = 3
                },
                new User
                {
                    ID = 8,
                    Email = "DoctorNew@gmail.com",
                    Password = "6qTCotEgwhIbnnofPaLuUmm0EQ62ibGuMrhOzPUqaSCviEwXEOCmCRhyapQrmswkp5T8LMO92soeMkLvzrgUvQ==", // raw Doc789!
                    Name = "Александър",
                    Surname = "Тодоров",
                    RoleID = 2
                },
                new User
                {
                    ID = 9,
                    Email = "PatientNew@gmail.com",
                    Password = "spb9btmd9VCXtDCwZKzomAhM21kJ3rm3on4b3iIcAnHdWN1y+Jopy/m+mOduu3LPClphnK30AwxdOqFJpZTHKg==", // raw Pat789?
                    Name = "Елена",
                    Surname = "Маринова",
                    RoleID = 1
                }
            );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(Doctor)))
            {
                modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    ID = 2,
                    Room = 101,
                    SpecialtyID = 1
                },
                new Doctor
                {
                    ID = 4,
                    Room = 202,
                    SpecialtyID = 2
                },
                new Doctor
                {
                    ID = 8,
                    Room = 303,
                    SpecialtyID = 3
                }
            );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(Doctor)))
            {
                modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    ID = 1,
                    SSN = 0725191800,
                    PhoneNumber = "+359891234567",
                    DoctorID = 2
                },
                new Patient
                {
                    ID = 5,
                    SSN = 0725191801,
                    PhoneNumber = "+359888765432",
                    DoctorID = 4
                },
                new Patient
                {
                    ID = 9,
                    SSN = 0725191802,
                    PhoneNumber = "+359888111222",
                    DoctorID = 8
                }
            );
            }

            if (!modelBuilder.Model.GetEntityTypes().Any(e => e.ClrType == typeof(MedicalExamination)))
            {
                modelBuilder.Entity<MedicalExamination>().HasData(

                    // Doctor 2 (IDs 1–8) — DECEMBER 2025
                    new MedicalExamination { ID = 1, Diagnosis = "Главоболие", Recommendation = "Почивка и хидратация", StartTime = new DateTime(2025, 12, 1, 9, 0, 0), EndTime = new DateTime(2025, 12, 1, 9, 30, 0), PatientID = 1, DoctorID = 2 },
                    new MedicalExamination { ID = 2, Diagnosis = "Замъглено зрение", Recommendation = "Очни капки", StartTime = new DateTime(2025, 12, 1, 10, 0, 0), EndTime = new DateTime(2025, 12, 1, 10, 30, 0), PatientID = 1, DoctorID = 2 },
                    new MedicalExamination { ID = 3, Diagnosis = "Синузит", Recommendation = "Антибиотик", StartTime = new DateTime(2025, 12, 1, 11, 0, 0), EndTime = new DateTime(2025, 12, 1, 11, 45, 0), PatientID = 1, DoctorID = 2 },

                    new MedicalExamination { ID = 4, Diagnosis = "Алергична реакция", Recommendation = "Антихистамини", StartTime = new DateTime(2025, 12, 2, 9, 0, 0), EndTime = new DateTime(2025, 12, 2, 9, 30, 0), PatientID = 1, DoctorID = 2 },
                    new MedicalExamination { ID = 5, Diagnosis = "Мигрена", Recommendation = "Обезболяващи", StartTime = new DateTime(2025, 12, 2, 10, 0, 0), EndTime = new DateTime(2025, 12, 2, 10, 45, 0), PatientID = 1, DoctorID = 2 },

                    new MedicalExamination { ID = 6, Diagnosis = "Кашлица", Recommendation = "Сироп за кашлица", StartTime = new DateTime(2025, 12, 3, 9, 0, 0), EndTime = new DateTime(2025, 12, 3, 9, 20, 0), PatientID = 1, DoctorID = 2 },
                    new MedicalExamination { ID = 7, Diagnosis = "Болки в корема", Recommendation = "Диета", StartTime = new DateTime(2025, 12, 3, 10, 0, 0), EndTime = new DateTime(2025, 12, 3, 10, 30, 0), PatientID = 1, DoctorID = 2 },
                    new MedicalExamination { ID = 8, Diagnosis = "Болки в гърба", Recommendation = "Физиотерапия", StartTime = new DateTime(2025, 12, 3, 11, 0, 0), EndTime = new DateTime(2025, 12, 3, 11, 45, 0), PatientID = 1, DoctorID = 2 },

                    // Doctor 4 (IDs 9–15) — JANUARY 2026
                    new MedicalExamination { ID = 9, Diagnosis = "Сърцебиене", Recommendation = "ЕКГ изследване", StartTime = new DateTime(2026, 1, 4, 9, 0, 0), EndTime = new DateTime(2026, 1, 4, 9, 30, 0), PatientID = 5, DoctorID = 4 },
                    new MedicalExamination { ID = 10, Diagnosis = "Високо кръвно", Recommendation = "Промяна в диетата", StartTime = new DateTime(2026, 1, 4, 10, 0, 0), EndTime = new DateTime(2026, 1, 4, 10, 30, 0), PatientID = 5, DoctorID = 4 },
                    new MedicalExamination { ID = 11, Diagnosis = "Болки в гърдите", Recommendation = "Кардиологичен преглед", StartTime = new DateTime(2026, 1, 4, 11, 0, 0), EndTime = new DateTime(2026, 1, 4, 11, 30, 0), PatientID = 5, DoctorID = 4 },

                    new MedicalExamination { ID = 12, Diagnosis = "Температура", Recommendation = "Парацетамол", StartTime = new DateTime(2026, 1, 5, 9, 0, 0), EndTime = new DateTime(2026, 1, 5, 9, 30, 0), PatientID = 5, DoctorID = 4 },
                    new MedicalExamination { ID = 13, Diagnosis = "Замайване", Recommendation = "Повече течности", StartTime = new DateTime(2026, 1, 5, 10, 0, 0), EndTime = new DateTime(2026, 1, 5, 10, 30, 0), PatientID = 5, DoctorID = 4 },

                    new MedicalExamination { ID = 14, Diagnosis = "Анемия", Recommendation = "Железни добавки", StartTime = new DateTime(2026, 1, 6, 9, 0, 0), EndTime = new DateTime(2026, 1, 6, 9, 30, 0), PatientID = 5, DoctorID = 4 },
                    new MedicalExamination { ID = 15, Diagnosis = "Настинка", Recommendation = "Почивка", StartTime = new DateTime(2026, 1, 6, 10, 0, 0), EndTime = new DateTime(2026, 1, 6, 10, 20, 0), PatientID = 5, DoctorID = 4 }
                );
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalExamination> MedicalExaminations { get; set; }
    }
}