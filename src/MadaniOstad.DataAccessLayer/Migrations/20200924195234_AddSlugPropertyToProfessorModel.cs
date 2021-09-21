using Microsoft.EntityFrameworkCore.Migrations;

namespace MadaniOstad.DataAccessLayer.Migrations
{
    public partial class AddSlugPropertyToProfessorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Professors",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Professors");
        }
    }
}
