using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations
{
    public partial class SecretSanta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "brobot_admin",
                schema: "brobot",
                table: "discord_user",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "secret_santa_group",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    check_past_year_pairings = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secret_santa_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "secret_santa_event",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    year = table.Column<int>(nullable: false),
                    created_date_utc = table.Column<DateTime>(nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    created_by_id = table.Column<decimal>(nullable: true),
                    secret_santa_group_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secret_santa_event", x => x.id);
                    table.ForeignKey(
                        name: "FK_secret_santa_event_discord_user_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_secret_santa_event_secret_santa_group_secret_santa_group_id",
                        column: x => x.secret_santa_group_id,
                        principalSchema: "brobot",
                        principalTable: "secret_santa_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "secret_santa_group_discord_user",
                schema: "brobot",
                columns: table => new
                {
                    secret_santa_group_id = table.Column<int>(nullable: false),
                    discord_user_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secret_santa_group_discord_user", x => new { x.secret_santa_group_id, x.discord_user_id });
                    table.ForeignKey(
                        name: "FK_secret_santa_group_discord_user_discord_user_discord_user_id",
                        column: x => x.discord_user_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_secret_santa_group_discord_user_secret_santa_group_secret_s~",
                        column: x => x.secret_santa_group_id,
                        principalSchema: "brobot",
                        principalTable: "secret_santa_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "secret_santa_pairing",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    secret_santa_event_id = table.Column<int>(nullable: false),
                    giver_id = table.Column<decimal>(nullable: false),
                    recipient_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secret_santa_pairing", x => x.id);
                    table.ForeignKey(
                        name: "FK_secret_santa_pairing_discord_user_giver_id",
                        column: x => x.giver_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_secret_santa_pairing_discord_user_recipient_id",
                        column: x => x.recipient_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_secret_santa_pairing_secret_santa_event_secret_santa_event_~",
                        column: x => x.secret_santa_event_id,
                        principalTable: "secret_santa_event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_secret_santa_event_created_by_id",
                table: "secret_santa_event",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_secret_santa_event_secret_santa_group_id",
                table: "secret_santa_event",
                column: "secret_santa_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_secret_santa_pairing_giver_id",
                table: "secret_santa_pairing",
                column: "giver_id");

            migrationBuilder.CreateIndex(
                name: "IX_secret_santa_pairing_recipient_id",
                table: "secret_santa_pairing",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_secret_santa_pairing_secret_santa_event_id",
                table: "secret_santa_pairing",
                column: "secret_santa_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_secret_santa_group_discord_user_discord_user_id",
                schema: "brobot",
                table: "secret_santa_group_discord_user",
                column: "discord_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "secret_santa_pairing");

            migrationBuilder.DropTable(
                name: "secret_santa_group_discord_user",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "secret_santa_event");

            migrationBuilder.DropTable(
                name: "secret_santa_group",
                schema: "brobot");

            migrationBuilder.DropColumn(
                name: "brobot_admin",
                schema: "brobot",
                table: "discord_user");
        }
    }
}
