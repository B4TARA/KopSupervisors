using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemAssessmentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SystemAssessmentType",
                table: "AssessmentTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemAssessmentType",
                table: "AssessmentTypes");
        }
    }
}
