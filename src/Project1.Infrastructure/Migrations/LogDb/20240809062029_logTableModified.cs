using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project1.Infrastructure.Migrations.LogDb
{
    /// <inheritdoc />
    public partial class logTableModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttrRouteInfo",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExceptionMessage",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExceptionStackTrace",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ExecutionDuration",
                table: "AuditLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsException",
                table: "AuditLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttrRouteInfo",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ExceptionMessage",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ExceptionStackTrace",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ExecutionDuration",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsException",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "AuditLogs");
        }
    }
}
