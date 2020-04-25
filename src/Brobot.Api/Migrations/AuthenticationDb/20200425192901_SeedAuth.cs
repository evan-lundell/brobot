using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations.AuthenticationDb
{
    public partial class SeedAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO auth.ApiKey ([Owner], [Key]) VALUES ('SyncService', '881fd42b-e64e-46ce-a1e9-7d6614e80acf');");
            migrationBuilder.Sql("INSERT INTO auth.ApiRole ([Name]) VALUES ('InternalService'), ('Sync');");
            migrationBuilder.Sql("INSERT INTO auth.ApiKeyRole ([ApiKeyId], [ApiRoleId]) SELECT k.ApiKeyId, r.ApiRoleId FROM auth.ApiKey k CROSS JOIN auth.ApiRole r WHERE k.[Owner] = 'SyncService' AND r.[Name] IN ('InternalService', 'Sync');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM auth.ApiKeyRole;");
            migrationBuilder.Sql("DELETE FROM auth.ApiRole;");
            migrationBuilder.Sql("DELETE FROM auth.ApiKey;");
        }
    }
}
