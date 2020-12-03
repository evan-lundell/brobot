using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class ReminderFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reminder_discord_user_owner_id",
                schema: "brobot",
                table: "reminder");

            migrationBuilder.AlterColumn<decimal>(
                name: "owner_id",
                schema: "brobot",
                table: "reminder",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddForeignKey(
                name: "FK_reminder_discord_user_owner_id",
                schema: "brobot",
                table: "reminder",
                column: "owner_id",
                principalSchema: "brobot",
                principalTable: "discord_user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reminder_discord_user_owner_id",
                schema: "brobot",
                table: "reminder");

            migrationBuilder.AlterColumn<decimal>(
                name: "owner_id",
                schema: "brobot",
                table: "reminder",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_reminder_discord_user_owner_id",
                schema: "brobot",
                table: "reminder",
                column: "owner_id",
                principalSchema: "brobot",
                principalTable: "discord_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
