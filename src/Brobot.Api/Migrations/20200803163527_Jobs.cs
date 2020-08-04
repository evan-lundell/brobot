using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations
{
    public partial class Jobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_response_channel_channel_id",
                schema: "brobot",
                table: "event_response");

            migrationBuilder.CreateTable(
                name: "job_definition",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 32, nullable: false),
                    description = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_definition", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 64, nullable: false),
                    description = table.Column<string>(maxLength: 1024, nullable: true),
                    cron_trigger = table.Column<string>(maxLength: 16, nullable: false),
                    created_date_utc = table.Column<DateTime>(nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    modified_date_utc = table.Column<DateTime>(nullable: true),
                    job_definition_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_job_definition_job_definition_id",
                        column: x => x.job_definition_id,
                        principalSchema: "brobot",
                        principalTable: "job_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_parameter_definition",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 32, nullable: false),
                    description = table.Column<string>(maxLength: 1024, nullable: true),
                    is_required = table.Column<bool>(nullable: false),
                    user_configurable = table.Column<bool>(nullable: false),
                    data_type = table.Column<string>(maxLength: 16, nullable: false),
                    job_definition_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_parameter_definition", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_parameter_definition_job_definition_job_definition_id",
                        column: x => x.job_definition_id,
                        principalSchema: "brobot",
                        principalTable: "job_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_channel",
                schema: "brobot",
                columns: table => new
                {
                    job_id = table.Column<int>(nullable: false),
                    channel_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_channel", x => new { x.channel_id, x.job_id });
                    table.ForeignKey(
                        name: "FK_job_channel_channel_channel_id",
                        column: x => x.channel_id,
                        principalSchema: "brobot",
                        principalTable: "channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_job_channel_job_job_id",
                        column: x => x.job_id,
                        principalSchema: "brobot",
                        principalTable: "job",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_parameter",
                schema: "brobot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<string>(maxLength: 1024, nullable: false),
                    job_id = table.Column<int>(nullable: false),
                    job_parameter_definition_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_parameter", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_parameter_job_job_id",
                        column: x => x.job_id,
                        principalSchema: "brobot",
                        principalTable: "job",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_job_parameter_job_parameter_definition_job_parameter_defini~",
                        column: x => x.job_parameter_definition_id,
                        principalSchema: "brobot",
                        principalTable: "job_parameter_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_job_definition_id",
                schema: "brobot",
                table: "job",
                column: "job_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_channel_job_id",
                schema: "brobot",
                table: "job_channel",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_parameter_job_id",
                schema: "brobot",
                table: "job_parameter",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_parameter_job_parameter_definition_id",
                schema: "brobot",
                table: "job_parameter",
                column: "job_parameter_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_parameter_definition_job_definition_id",
                schema: "brobot",
                table: "job_parameter_definition",
                column: "job_definition_id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_response_channel_channel_id",
                schema: "brobot",
                table: "event_response",
                column: "channel_id",
                principalSchema: "brobot",
                principalTable: "channel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_response_channel_channel_id",
                schema: "brobot",
                table: "event_response");

            migrationBuilder.DropTable(
                name: "job_channel",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "job_parameter",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "job",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "job_parameter_definition",
                schema: "brobot");

            migrationBuilder.DropTable(
                name: "job_definition",
                schema: "brobot");

            migrationBuilder.AddForeignKey(
                name: "FK_event_response_channel_channel_id",
                schema: "brobot",
                table: "event_response",
                column: "channel_id",
                principalSchema: "brobot",
                principalTable: "channel",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
