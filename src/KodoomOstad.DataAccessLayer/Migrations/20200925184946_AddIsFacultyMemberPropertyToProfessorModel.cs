using Microsoft.EntityFrameworkCore.Migrations;

namespace KodoomOstad.DataAccessLayer.Migrations
{
    public partial class AddIsFacultyMemberPropertyToProfessorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFacultyMember",
                table: "Professors",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFacultyMember",
                table: "Professors");
        }
    }
}
