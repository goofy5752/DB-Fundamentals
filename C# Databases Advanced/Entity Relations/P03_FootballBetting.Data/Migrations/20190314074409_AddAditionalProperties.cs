using Microsoft.EntityFrameworkCore.Migrations;

namespace P03_FootballBetting.Data.Migrations
{
    public partial class AddAditionalProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Teams",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Teams");
        }
    }
}
