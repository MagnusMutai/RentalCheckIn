using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCheckedInAtCheckedOutAtReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                table: "reservation",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedOutAt",
                table: "reservation",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "CheckedOutAt",
                table: "reservation");
        }
    }
}
