using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations
{
    public partial class Reminders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reminder",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_id = table.Column<decimal>(nullable: false),
                    channel_id = table.Column<decimal>(nullable: false),
                    message = table.Column<string>(maxLength: 1024, nullable: false),
                    created_date_utc = table.Column<DateTime>(nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    reminder_date_utc = table.Column<DateTime>(nullable: false),
                    sent_date_utc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminder", x => x.id);
                    table.ForeignKey(
                        name: "FK_reminder_channel_channel_id",
                        column: x => x.channel_id,
                        principalSchema: "brobot",
                        principalTable: "channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reminder_discord_user_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reminder_channel_id",
                schema: "brobot",
                table: "reminder",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminder_owner_id",
                schema: "brobot",
                table: "reminder",
                column: "owner_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reminder",
                schema: "brobot");
        }
    }
}
