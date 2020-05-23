using Microsoft.EntityFrameworkCore.Migrations;

namespace Semestro_projektas.Migrations
{
    public partial class fff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "ChannelUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ChannelUsers");
        }
    }
}
