using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Data.Migrations
{
    public partial class AddContrainstScheduleIdOfSD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDetail_Schedule",
                table: "ScheduleDetail");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "ScheduleDetail",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDetail_Schedule",
                table: "ScheduleDetail",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDetail_Schedule",
                table: "ScheduleDetail");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "ScheduleDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDetail_Schedule",
                table: "ScheduleDetail",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
