using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "brobot");

            migrationBuilder.CreateTable(
                name: "discord_user",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false),
                    username = table.Column<string>(maxLength: 128, nullable: false),
                    birthdate = table.Column<DateTime>(nullable: true),
                    timezone = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discord_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "server",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "channel",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: false),
                    server_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channel", x => x.id);
                    table.ForeignKey(
                        name: "FK_channel_server_server_id",
                        column: x => x.server_id,
                        principalSchema: "brobot",
                        principalTable: "server",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discord_user_channel",
                schema: "brobot",
                columns: table => new
                {
                    channel_id = table.Column<decimal>(nullable: false),
                    discord_user_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discord_user_channel", x => new { x.discord_user_id, x.channel_id });
                    table.ForeignKey(
                        name: "FK_discord_user_channel_channel_channel_id",
                        column: x => x.channel_id,
                        principalSchema: "brobot",
                        principalTable: "channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discord_user_channel_discord_user_discord_user_id",
                        column: x => x.discord_user_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_channel_server_id",
                schema: "brobot",
                table: "channel",
                column: "server_id");

            migrationBuilder.CreateIndex(
                name: "IX_discord_user_channel_channel_id",
                schema: "brobot",
                table: "discord_user_channel",
                column: "channel_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "discord_user_channel",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "channel",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "discord_user",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "server",
                schema: "brobot");
        }
    }
}
