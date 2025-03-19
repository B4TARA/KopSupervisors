using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUselessDateOfCreationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Subdivisions");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "AssessmentTypes");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "AssessmentMatrixElements");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "AssessmentMatrices");

            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "AssessmentInterpretations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "Subdivisions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "Mails",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "AssessmentTypes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "AssessmentMatrixElements",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "AssessmentMatrices",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfCreation",
                table: "AssessmentInterpretations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
