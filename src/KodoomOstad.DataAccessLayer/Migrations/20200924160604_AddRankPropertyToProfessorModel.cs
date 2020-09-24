using Microsoft.EntityFrameworkCore.Migrations;

namespace KodoomOstad.DataAccessLayer.Migrations
{
    public partial class AddRankPropertyToProfessorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Rank",
                table: "Professors",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Professors");
        }
    }
}
