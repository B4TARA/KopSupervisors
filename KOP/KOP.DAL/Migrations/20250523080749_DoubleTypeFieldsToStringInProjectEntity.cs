using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DoubleTypeFieldsToStringInProjectEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SuccessRate",
                table: "Projects",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "SP",
                table: "Projects",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "AverageKpi",
                table: "Projects",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SuccessRate",
                table: "Projects",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "SP",
                table: "Projects",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "AverageKpi",
                table: "Projects",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
