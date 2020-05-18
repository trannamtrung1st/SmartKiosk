using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Data.Migrations
{
    public partial class AddArchivedResType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "ResourceType",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "ResourceType");
        }
    }
}
