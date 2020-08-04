using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class SeedJobDefinitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // job definitions
            migrationBuilder.Sql("INSERT INTO brobot.job_definition (name, description) VALUES ('Stats', 'Counts each user''s total messages sent for a given time period, and optionally creates a word cloud');");
            migrationBuilder.Sql("INSERT INTO brobot.job_definition (name, description) VALUES ('Twitter', 'Connects to a Twitter feed and posts tweets matching a given string');");
            migrationBuilder.Sql("INSERT INTO brobot.job_definition (name, description) VALUES ('Birthdays', 'Checks for birthdays and wishes the user a happy birthday');");
            migrationBuilder.Sql("INSERT INTO brobot.job_definition (name, description) VALUES ('Reminders', 'Checks for and posts saved reminders');");

            // job parameter definitions
            migrationBuilder.Sql("INSERT INTO brobot.job_parameter_definition (name, description, is_required, user_configurable, data_type, job_definition_id) VALUES ('Period', 'The period to gather stats for. Available options: Day, Month, Year', true, true, 'string', (SELECT id FROM brobot.job_definition WHERE name = 'Stats'));");
            migrationBuilder.Sql("INSERT INTO brobot.job_parameter_definition (name, description, is_required, user_configurable, data_type, job_definition_id) VALUES ('GenerateWordCloud', 'Value indicating whether or not a word cloud should be generated', true, true, 'boolean', (SELECT id FROM brobot.job_definition WHERE name = 'Stats'));");

            migrationBuilder.Sql("INSERT INTO brobot.job_parameter_definition (name, description, is_required, user_configurable, data_type, job_definition_id) VALUES ('TwitterUrl', 'The URL of the Twitter feed to monitor', true, true, 'string', (SELECT id FROM brobot.job_definition WHERE name = 'Twitter'));");
            migrationBuilder.Sql("INSERT INTO brobot.job_parameter_definition (name, description, is_required, user_configurable, data_type, job_definition_id) VALUES ('LatestTweetId', 'The last tweet that was checked', true, false, 'string', (SELECT id FROM brobot.job_definition WHERE name = 'Twitter'));");
            migrationBuilder.Sql("INSERT INTO brobot.job_parameter_definition (name, description, is_required, user_configurable, data_type, job_definition_id) VALUES ('Contains', 'String the tweet must contain to be posted', false, true, 'string', (SELECT id FROM brobot.job_definition WHERE name = 'Twitter'));");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM brobot.job_definition;");
        }
    }
}
