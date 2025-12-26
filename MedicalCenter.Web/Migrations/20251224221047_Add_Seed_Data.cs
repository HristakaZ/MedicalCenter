using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalCenter.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_Seed_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed data for Specialties table
            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "ID", "Description" },
                values: new object[] { 1, "Ophthalmologist" }
            );

            // Seed data for Roles table
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

            // Seed data for Users table
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "Email", "Password", "Name", "Surname", "RoleID" },
                values: new object[,]
                {
                    { 1, "Patient123@gmail.com", "/iLgcY4bH4mID2rF4jeG0ZtIBUW84oWOuGfF7GqZzrCvGHGq1R/AswPwFwIrHnLX+XbzWcX+rbhYj5CDuzP+xw==", "John", "Smith", 1 },
                    { 2, "Doctor456@gmail.com", "x0bPjSO2rxdVaDzOG9C2fSygAYtFlPekH/UKNgJ1WsMVfD+dzTwYev2/sKA7oNQZ9sh4m0D6m8sS+cF/Qdorww==", "Joe", "Green", 2 },
                    { 3, "Administrator789@gmail.com", "63QCHXTudjyoCjWA6wXjgkcowt2MCwNDnZD2AFf3+dY8N6qXVulIxW4p7Ucyrwh3otzPRSJLJMhZtt2VWYda2w==", "Jay", "Blue", 3 }
                }
            );

            // Seed data for Doctors table
            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "ID", "Room", "SpecialtyID" },
                values: new object[] { 2, 101, 1 }
            );

            // Seed data for Patients table
            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "ID", "SSN", "PhoneNumber", "DoctorID" },
                values: new object[] { 1, 0725191800, "+359891234567", 2 }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
