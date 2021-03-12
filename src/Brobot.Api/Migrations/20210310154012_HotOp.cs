using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations
{
    public partial class HotOp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hot_op",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    start_datetime_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_datetime_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hot_op", x => x.id);
                    table.ForeignKey(
                        name: "FK_hot_op_discord_user_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voice_channel",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    server_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voice_channel", x => x.id);
                    table.ForeignKey(
                        name: "FK_voice_channel_server_server_id",
                        column: x => x.server_id,
                        principalSchema: "brobot",
                        principalTable: "server",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hot_op_session",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discord_user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    voice_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    hot_op_id = table.Column<int>(type: "integer", nullable: false),
                    start_datetime_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_datetime_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hot_op_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_hot_op_session_discord_user_discord_user_id",
                        column: x => x.discord_user_id,
                        principalSchema: "brobot",
                        principalTable: "discord_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hot_op_session_hot_op_hot_op_id",
                        column: x => x.hot_op_id,
                        principalSchema: "brobot",
                        principalTable: "hot_op",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hot_op_session_voice_channel_voice_channel_id",
                        column: x => x.voice_channel_id,
                        principalSchema: "brobot",
                        principalTable: "voice_channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hot_op_owner_id",
                schema: "brobot",
                table: "hot_op",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_hot_op_session_discord_user_id",
                schema: "brobot",
                table: "hot_op_session",
                column: "discord_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_hot_op_session_hot_op_id",
                schema: "brobot",
                table: "hot_op_session",
                column: "hot_op_id");

            migrationBuilder.CreateIndex(
                name: "IX_hot_op_session_voice_channel_id",
                schema: "brobot",
                table: "hot_op_session",
                column: "voice_channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_voice_channel_server_id",
                schema: "brobot",
                table: "voice_channel",
                column: "server_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hot_op_session",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "hot_op",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "voice_channel",
                schema: "brobot");
        }
    }
}
