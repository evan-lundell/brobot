using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class SeedEventResponses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO brobot.event_response (discord_event_id, message_text, response_text) VALUES ((SELECT id FROM brobot.discord_event WHERE name = 'MessageReceived'), 'good bot', 'Thanks! :robot:');");
            migrationBuilder.Sql("INSERT INTO brobot.event_response (discord_event_id, message_text, response_text) VALUES ((SELECT id FROM brobot.discord_event WHERE name = 'MessageReceived'), 'bad bot', ':middle_finger:');");
            migrationBuilder.Sql("INSERT INTO brobot.event_response (discord_event_id, response_text) VALUES ((SELECT id FROM brobot.discord_event WHERE name = 'ChannelUpdated'), 'Channel name changed from ''{oldChannelName}'' to ''{newChannelName}''');");
            migrationBuilder.Sql("INSERT INTO brobot.event_response (discord_event_id, response_text) VALUES ((SELECT id FROM brobot.discord_event WHERE name = 'MessageDeleted'), 'I saw that {authorName} :spy:');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM brobot.event_response;");
        }
    }
}
