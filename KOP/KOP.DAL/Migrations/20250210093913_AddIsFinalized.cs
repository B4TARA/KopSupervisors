using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFinalized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKpisFinalized",
                table: "Grades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMarksFinalized",
                table: "Grades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProjectsFinalized",
                table: "Grades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQualificationFinalized",
                table: "Grades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStrategicTasksFinalized",
                table: "Grades",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValueJudgmentFinalized",
                table: "Grades",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKpisFinalized",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "IsMarksFinalized",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "IsProjectsFinalized",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "IsQualificationFinalized",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "IsStrategicTasksFinalized",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "IsValueJudgmentFinalized",
                table: "Grades");
        }
    }
}
