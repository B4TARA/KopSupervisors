using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RestructProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStatusDate",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "PlanStages",
                table: "Projects",
                newName: "SuccessRate");

            migrationBuilder.RenameColumn(
                name: "FactStages",
                table: "Projects",
                newName: "AverageKpi");

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Qn2",
                table: "Grades",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Qn2",
                table: "Grades");

            migrationBuilder.RenameColumn(
                name: "SuccessRate",
                table: "Projects",
                newName: "PlanStages");

            migrationBuilder.RenameColumn(
                name: "AverageKpi",
                table: "Projects",
                newName: "FactStages");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CurrentStatusDate",
                table: "Projects",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
