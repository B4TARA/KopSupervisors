using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveJudgedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentResults_Users_JudgedId",
                table: "AssessmentResults");

            migrationBuilder.DropIndex(
                name: "IX_AssessmentResults_JudgedId",
                table: "AssessmentResults");

            migrationBuilder.DropColumn(
                name: "JudgedId",
                table: "AssessmentResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JudgedId",
                table: "AssessmentResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_JudgedId",
                table: "AssessmentResults",
                column: "JudgedId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentResults_Users_JudgedId",
                table: "AssessmentResults",
                column: "JudgedId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
