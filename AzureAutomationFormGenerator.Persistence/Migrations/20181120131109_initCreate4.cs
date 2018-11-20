using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureAutomationFormGenerator.Persistence.Migrations
{
    public partial class initCreate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestName",
                table: "AuditLogs",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RequestName",
                table: "AuditLogs",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
