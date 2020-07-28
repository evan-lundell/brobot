using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations
{
    public partial class Responses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "discord_event",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discord_event", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "event_response",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discord_event_id = table.Column<int>(nullable: false),
                    channel_id = table.Column<decimal>(nullable: true),
                    message_text = table.Column<string>(maxLength: 1024, nullable: true),
                    response_text = table.Column<string>(maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_response", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_response_channel_channel_id",
                        column: x => x.channel_id,
                        principalSchema: "brobot",
                        principalTable: "channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_event_response_discord_event_discord_event_id",
                        column: x => x.discord_event_id,
                        principalSchema: "brobot",
                        principalTable: "discord_event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_response_channel_id",
                schema: "brobot",
                table: "event_response",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_response_discord_event_id",
                schema: "brobot",
                table: "event_response",
                column: "discord_event_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_response",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "discord_event",
                schema: "brobot");
        }
    }
}
