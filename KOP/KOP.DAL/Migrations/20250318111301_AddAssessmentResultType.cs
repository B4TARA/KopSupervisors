using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAssessmentResultType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AssessmentResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "AssessmentResults");
        }
    }
}
