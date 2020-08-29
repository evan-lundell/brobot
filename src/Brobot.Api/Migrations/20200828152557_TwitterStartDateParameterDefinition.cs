using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class TwitterStartDateParameterDefinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE brobot.job_parameter_definition SET name = 'LastCheckDateTimeUtc', description = 'Last time this feed was checked', data_type='datetime' WHERE name = 'LatestTweetId' AND job_definition_id = (SELECT id FROM brobot.job_definition WHERE name = 'Twitter');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE brobot.job_parameter_definition SET name = 'LatestTweetId', description = 'The last tweet that was checked', data_type='string' WHERE name = 'LastCheckDateTimeUtc' AND job_definition_id = (SELECT id FROM brobot.job_definition WHERE name = 'Twitter');");
        }
    }
}
