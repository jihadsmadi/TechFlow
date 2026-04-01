using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSprints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SprintNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Goal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActualEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SprintItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SprintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintItems_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SprintItems_SprintId",
                table: "SprintItems",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintItems_SprintId_TaskId",
                table: "SprintItems",
                columns: new[] { "SprintId", "TaskId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SprintItems_TaskId",
                table: "SprintItems",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_CompanyId",
                table: "Sprints",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId",
                table: "Sprints",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId_SprintNumber",
                table: "Sprints",
                columns: new[] { "ProjectId", "SprintNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId_Status",
                table: "Sprints",
                columns: new[] { "ProjectId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SprintItems");

            migrationBuilder.DropTable(
                name: "Sprints");
        }
    }
}
