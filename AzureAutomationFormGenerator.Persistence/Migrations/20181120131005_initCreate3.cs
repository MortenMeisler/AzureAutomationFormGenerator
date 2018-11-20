using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureAutomationFormGenerator.Persistence.Migrations
{
    public partial class initCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestName",
                table: "AuditLogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestName",
                table: "AuditLogs");
        }
    }
}
