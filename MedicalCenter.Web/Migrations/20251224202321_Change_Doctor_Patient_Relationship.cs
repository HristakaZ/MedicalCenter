using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalCenter.Web.Migrations
{
    /// <inheritdoc />
    public partial class Change_Doctor_Patient_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorsPatients");

            migrationBuilder.AddColumn<int>(
                name: "DoctorID",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorID",
                table: "Patients",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
