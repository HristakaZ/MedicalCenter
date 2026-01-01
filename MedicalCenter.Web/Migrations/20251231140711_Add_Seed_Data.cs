using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalCenter.Web.Migrations
{
    public partial class Add_Seed_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SPECIALTIES
            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "ID", "Description" },
                values: new object[,]
                {
                    { 1, "Офталмолог" },
                    { 2, "Кардиолог" },
                    { 3, "Невролог" },
                    { 4, "Дерматолог" },
                    { 5, "Педиатър" }
                }
            );

            // ROLES
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Description" },
                values: new object[,]
                {
                    { 1, "Patient" },
                    { 2, "Doctor" },
                    { 3, "Administrator" }
                }
            );

            // USERS
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "Email", "Password", "Name", "Surname", "RoleID" },
                values: new object[,]
                {
                    { 1, "Patient123@gmail.com", "/iLgcY4bH4mID2rF4jeG0ZtIBUW84oWOuGfF7GqZzrCvGHGq1R/AswPwFwIrHnLX+XbzWcX+rbhYj5CDuzP+xw==", "Иван", "Иванов", 1 }, // raw password - Test123/       
                    { 2, "Doctor456@gmail.com", "x0bPjSO2rxdVaDzOG9C2fSygAYtFlPekH/UKNgJ1WsMVfD+dzTwYev2/sKA7oNQZ9sh4m0D6m8sS+cF/Qdorww==", "Георги", "Георгиев", 2 }, // raw password - Test456!
                    { 3, "Administrator789@gmail.com", "63QCHXTudjyoCjWA6wXjgkcowt2MCwNDnZD2AFf3+dY8N6qXVulIxW4p7Ucyrwh3otzPRSJLJMhZtt2VWYda2w==", "Димитър", "Димитров", 3 }, // raw password - Test789?
                    { 4, "DoctorBG@gmail.com", "ws0tN4IGRQWBBZSU87WEt1RiQCKmkxLOet6pimScLK5YU07QfGLouqMc8mhlIZe4KRD3TUAQnvlFjRXxXWTCbA==", "Стефан", "Петров", 2 }, // raw password - MedDoc123!
                    { 5, "PatientBG@gmail.com", "xNrdNp5NsCzus3j/shDrfVEui3/PXjlW/bVwi2OqOvPW1YVIKw0iD+MQVc5De+pq+f0f3MD2cWkN2lEqZrjI/A==", "Мария", "Николова", 1 }, // raw password - MedPat456?
                    { 6, "AdminIvan@gmail.com", "p3ln9clcsffFCYXmkm9XfR2mmyH6XSptR777LC3XinRHPdVMKi0/1ttIZcr+rUC69TrpB3RVuC/MPAbLhnV+xw==", "Ивайло", "Костов", 3 }, // raw password - Admin123!
                    { 7, "AdminPetar@gmail.com", "F+1NS4CnMKWqh/Ytwpbz+baQ7Zqj8SEZs+LJn/sZ33OaHovTKGwSDJlweC97FnaT96Noh2A/eZAmVZa3TinYFA==", "Петър", "Симеонов", 3 }, // raw password - Admin456?
                    { 8, "DoctorNew@gmail.com", "6qTCotEgwhIbnnofPaLuUmm0EQ62ibGuMrhOzPUqaSCviEwXEOCmCRhyapQrmswkp5T8LMO92soeMkLvzrgUvQ==", "Александър", "Тодоров", 2 }, // raw password - Doc789!
                    { 9, "PatientNew@gmail.com", "spb9btmd9VCXtDCwZKzomAhM21kJ3rm3on4b3iIcAnHdWN1y+Jopy/m+mOduu3LPClphnK30AwxdOqFJpZTHKg==", "Елена", "Маринова", 1 } // raw password - Pat789?
                }
            );

            // DOCTORS
            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "ID", "Room", "SpecialtyID" },
                values: new object[,]
                {
                    { 2, 101, 1 },
                    { 4, 202, 2 },
                    { 8, 303, 3 }
                }
            );

            // PATIENTS
            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "ID", "SSN", "PhoneNumber", "DoctorID" },
                values: new object[,]
                {
                    { 1, 0725191800L, "+359891234567", 2 },
                    { 5, 0725191801L, "+359888765432", 4 },
                    { 9, 0725191802L, "+359888111222", 8 },
                }
            );

            // MEDICAL EXAMINATIONS
            migrationBuilder.InsertData(
                table: "MedicalExaminations",
                columns: new[] { "ID", "Diagnosis", "Recommendation", "StartTime", "EndTime", "PatientID", "DoctorID" },
                values: new object[,]
                {
                    // Doctor 2 — December 2025
                    { 1, "Главоболие", "Почивка и хидратация", new DateTime(2025,12,1,9,0,0), new DateTime(2025,12,1,9,30,0), 1, 2 },
                    { 2, "Замъглено зрение", "Очни капки", new DateTime(2025,12,1,10,0,0), new DateTime(2025,12,1,10,30,0), 1, 2 },
                    { 3, "Синузит", "Антибиотик", new DateTime(2025,12,1,11,0,0), new DateTime(2025,12,1,11,45,0), 1, 2 },
                    { 4, "Алергична реакция", "Антихистамини", new DateTime(2025,12,2,9,0,0), new DateTime(2025,12,2,9,30,0), 1, 2 },
                    { 5, "Мигрена", "Обезболяващи", new DateTime(2025,12,2,10,0,0), new DateTime(2025,12,2,10,45,0), 1, 2 },
                    { 6, "Кашлица", "Сироп за кашлица", new DateTime(2025,12,3,9,0,0), new DateTime(2025,12,3,9,20,0), 1, 2 },
                    { 7, "Болки в корема", "Диета", new DateTime(2025,12,3,10,0,0), new DateTime(2025,12,3,10,30,0), 1, 2 },
                    { 8, "Болки в гърба", "Физиотерапия", new DateTime(2025,12,3,11,0,0), new DateTime(2025,12,3,11,45,0), 1, 2 },

                    // Doctor 4 — January 2026
                    { 9, "Сърцебиене", "ЕКГ изследване", new DateTime(2026,1,4,9,0,0), new DateTime(2026,1,4,9,30,0), 5, 4 },
                    { 10, "Високо кръвно", "Промяна в диетата", new DateTime(2026,1,4,10,0,0), new DateTime(2026,1,4,10,30,0), 5, 4 },
                    { 11, "Болки в гърдите", "Кардиологичен преглед", new DateTime(2026,1,4,11,0,0), new DateTime(2026,1,4,11,30,0), 5, 4 },
                    { 12, "Температура", "Парацетамол", new DateTime(2026,1,5,9,0,0), new DateTime(2026,1,5,9,30,0), 5, 4 },
                    { 13, "Замайване", "Повече течности", new DateTime(2026,1,5,10,0,0), new DateTime(2026,1,5,10,30,0), 5, 4 },
                    { 14, "Анемия", "Железни добавки", new DateTime(2026,1,6,9,0,0), new DateTime(2026,1,6,9,30,0), 5, 4 },
                    { 15, "Настинка", "Почивка", new DateTime(2026,1,6,10,0,0), new DateTime(2026,1,6,10,20,0), 5, 4 }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("MedicalExaminations", "ID", new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
            migrationBuilder.DeleteData("Patients", "ID", new object[] { 1, 5 });
            migrationBuilder.DeleteData("Doctors", "ID", new object[] { 2, 4 });
            migrationBuilder.DeleteData("Users", "ID", new object[] { 1, 2, 3, 4, 5 });
            migrationBuilder.DeleteData("Roles", "ID", new object[] { 1, 2, 3 });
            migrationBuilder.DeleteData("Specialties", "ID", new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
