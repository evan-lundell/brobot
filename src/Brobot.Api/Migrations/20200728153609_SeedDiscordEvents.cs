using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class SeedDiscordEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO brobot.discord_event (name) VALUES ('MessageReceived'), ('MessageDeleted'), ('ChannelUpdated');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM brobot.discord_event;");
        }
    }
}
