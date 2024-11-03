using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmailVerProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "lhost",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVTokenExpiresAt",
                table: "lhost",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "lhost",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "lhost");

            migrationBuilder.DropColumn(
                name: "EmailVTokenExpiresAt",
                table: "lhost");

            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "lhost");
        }
    }
}
