using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsSuspendedToIsDismissed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendation_Grades_GradeId",
                table: "Recommendation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recommendation",
                table: "Recommendation");

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "ValueJudgments");

            migrationBuilder.RenameTable(
                name: "Recommendation",
                newName: "Recommendations");

            migrationBuilder.RenameColumn(
                name: "IsSuspended",
                table: "Users",
                newName: "IsDismissed");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendation_GradeId",
                table: "Recommendations",
                newName: "IX_Recommendations_GradeId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfModification",
                table: "Recommendations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfCreation",
                table: "Recommendations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recommendations",
                table: "Recommendations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Grades_GradeId",
                table: "Recommendations",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Grades_GradeId",
                table: "Recommendations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recommendations",
                table: "Recommendations");

            migrationBuilder.RenameTable(
                name: "Recommendations",
                newName: "Recommendation");

            migrationBuilder.RenameColumn(
                name: "IsDismissed",
                table: "Users",
                newName: "IsSuspended");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendations_GradeId",
                table: "Recommendation",
                newName: "IX_Recommendation_GradeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "ValueJudgments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfModification",
                table: "Recommendation",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfCreation",
                table: "Recommendation",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recommendation",
                table: "Recommendation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendation_Grades_GradeId",
                table: "Recommendation",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
