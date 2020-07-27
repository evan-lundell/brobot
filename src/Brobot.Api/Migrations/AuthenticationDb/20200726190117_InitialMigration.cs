using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations.AuthenticationDb
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "api_key",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner = table.Column<string>(maxLength: 50, nullable: false),
                    key = table.Column<string>(maxLength: 36, nullable: false),
                    created_date_utc = table.Column<DateTime>(nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_key", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "api_role",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "api_key_role",
                schema: "auth",
                columns: table => new
                {
                    api_key_id = table.Column<int>(nullable: false),
                    api_role_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_key_role", x => new { x.api_key_id, x.api_role_id });
                    table.ForeignKey(
                        name: "FK_api_key_role_api_key_api_key_id",
                        column: x => x.api_key_id,
                        principalSchema: "auth",
                        principalTable: "api_key",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_api_key_role_api_role_api_role_id",
                        column: x => x.api_role_id,
                        principalSchema: "auth",
                        principalTable: "api_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_api_key_role_api_role_id",
                schema: "auth",
                table: "api_key_role",
                column: "api_role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_key_role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "api_key",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "api_role",
                schema: "auth");
        }
    }
}
