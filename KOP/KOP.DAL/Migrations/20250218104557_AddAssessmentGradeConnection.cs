using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAssessmentGradeConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assessments_AssessmentTypeId",
                table: "Assessments");

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Assessments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_AssessmentTypeId_GradeId",
                table: "Assessments",
                columns: new[] { "AssessmentTypeId", "GradeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_GradeId",
                table: "Assessments",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Grades_GradeId",
                table: "Assessments",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Grades_GradeId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_AssessmentTypeId_GradeId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_GradeId",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Assessments");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_AssessmentTypeId",
                table: "Assessments",
                column: "AssessmentTypeId");
        }
    }
}
