using Microsoft.EntityFrameworkCore.Migrations;

namespace KodoomOstad.DataAccessLayer.Migrations
{
    public partial class AddAverageRatePropertyToProfessorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AverageRate",
                table: "Professors",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRate",
                table: "Professors");
        }
    }
}
