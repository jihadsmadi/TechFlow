using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListAuditableEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Lists",
                newName: "LastModifiedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Lists",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Lists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Lists",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Lists");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Lists");

            migrationBuilder.RenameColumn(
                name: "LastModifiedUtc",
                table: "Lists",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Lists",
                newName: "CreatedAt");
        }
    }
}
