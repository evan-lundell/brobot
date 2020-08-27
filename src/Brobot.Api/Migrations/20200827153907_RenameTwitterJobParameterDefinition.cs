using Microsoft.EntityFrameworkCore.Migrations;

namespace Brobot.Api.Migrations
{
    public partial class RenameTwitterJobParameterDefinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE brobot.job_parameter_definition SET name = 'TwitterHandle', description = 'The handle of the Twitter feed to monitor (without @)' WHERE name = 'TwitterUrl' AND job_definition_id = (SELECT id FROM job_definition WHERE name = 'Twitter');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE brobot.job_parameter_definition SET name = 'TwitterUrl', description = 'The URL of the Twitter feed to monitor' WHERE name = 'TwitterHandle' AND job_definition_id = (SELECT id FROM job_definition WHERE name = 'Twitter');");
        }
    }
}
