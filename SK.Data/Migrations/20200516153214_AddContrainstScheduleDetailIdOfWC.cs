using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Data.Migrations
{
    public partial class AddContrainstScheduleDetailIdOfWC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ScheduleDetailId",
                table: "ScheduleWeekConfig",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ScheduleDetailId",
                table: "ScheduleWeekConfig",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
