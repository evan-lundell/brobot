using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class HotOpPrimaryChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "primary_channel_id",
                schema: "brobot",
                table: "hot_op",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_hot_op_primary_channel_id",
                schema: "brobot",
                table: "hot_op",
                column: "primary_channel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_hot_op_channel_primary_channel_id",
                schema: "brobot",
                table: "hot_op",
                column: "primary_channel_id",
                principalSchema: "brobot",
                principalTable: "channel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_hot_op_channel_primary_channel_id",
                schema: "brobot",
                table: "hot_op");

            migrationBuilder.DropIndex(
                name: "IX_hot_op_primary_channel_id",
                schema: "brobot",
                table: "hot_op");

            migrationBuilder.DropColumn(
                name: "primary_channel_id",
                schema: "brobot",
                table: "hot_op");
        }
    }
}
