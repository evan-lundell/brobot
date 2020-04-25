using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations.AuthenticationDb
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "ApiKey",
                schema: "auth",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Owner = table.Column<string>(maxLength: 50, nullable: false),
                    Key = table.Column<string>(maxLength: 36, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKey", x => x.ApiKeyId);
                });

            migrationBuilder.CreateTable(
                name: "ApiRole",
                schema: "auth",
                columns: table => new
                {
                    ApiRoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiRole", x => x.ApiRoleId);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyRole",
                schema: "auth",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(nullable: false),
                    ApiRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyRole", x => new { x.ApiKeyId, x.ApiRoleId });
                    table.ForeignKey(
                        name: "FK_ApiKeyRole_ApiKey_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "auth",
                        principalTable: "ApiKey",
                        principalColumn: "ApiKeyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiKeyRole_ApiRole_ApiRoleId",
                        column: x => x.ApiRoleId,
                        principalSchema: "auth",
                        principalTable: "ApiRole",
                        principalColumn: "ApiRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRole_ApiRoleId",
                schema: "auth",
                table: "ApiKeyRole",
                column: "ApiRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ApiKey",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ApiRole",
                schema: "auth");
        }
    }
}
