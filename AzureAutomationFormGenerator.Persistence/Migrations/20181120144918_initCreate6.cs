using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureAutomationFormGenerator.Persistence.Migrations
{
    public partial class initCreate6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestInput2",
                table: "AuditLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestInput2",
                table: "AuditLogs");
        }
    }
}
