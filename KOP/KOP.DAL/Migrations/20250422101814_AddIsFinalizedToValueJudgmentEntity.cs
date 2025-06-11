using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFinalizedToValueJudgmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HigherEducation");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "ValueJudgments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "HigherEducations",
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
                    table.PrimaryKey("PK_HigherEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HigherEducations_Qualifications_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "Qualifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HigherEducations_QualificationId",
                table: "HigherEducations",
                column: "QualificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HigherEducations");

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "ValueJudgments");

            migrationBuilder.CreateTable(
                name: "HigherEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QualificationId = table.Column<int>(type: "integer", nullable: false),
                    DateOfCreation = table.Column<DateOnly>(type: "date", nullable: false),
                    Education = table.Column<string>(type: "text", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    QualificationName = table.Column<string>(type: "text", nullable: false),
                    Speciality = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false)
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
    }
}
