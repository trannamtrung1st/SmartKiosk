using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Data.Migrations
{
    public partial class AddForeignKeyResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Resource",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FloorId",
                table: "Resource",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "FloorId",
                table: "Resource");
        }
    }
}
