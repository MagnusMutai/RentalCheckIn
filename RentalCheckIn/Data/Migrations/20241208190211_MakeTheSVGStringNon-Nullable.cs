using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeTheSVGStringNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "language",
                keyColumn: "svg",
                keyValue: null,
                column: "svg",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "svg",
                table: "language",
                type: "text",
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldCollation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "svg",
                table: "language",
                type: "text",
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3");
        }
    }
}
