using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RestructureQualification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalEducation",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "HigherEducation",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "Speciality",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Qualifications");

            migrationBuilder.CreateTable(
                name: "HigherEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Education = table.Column<string>(type: "text", nullable: false),
                    Speciality = table.Column<string>(type: "text", nullable: false),
                    QualificationName = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    QualificationId = table.Column<int>(type: "integer", nullable: false),
                    DateOfCreation = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HigherEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HigherEducation_Qualifications_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "Qualifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HigherEducation_QualificationId",
                table: "HigherEducation",
                column: "QualificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HigherEducation");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalEducation",
                table: "Qualifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Qualifications",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "HigherEducation",
                table: "Qualifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Qualifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Speciality",
                table: "Qualifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Qualifications",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
