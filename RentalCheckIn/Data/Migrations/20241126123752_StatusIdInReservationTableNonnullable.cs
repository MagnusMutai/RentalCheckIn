using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatusIdInReservationTableNonnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservation_status_statusId",
                table: "reservation");

            migrationBuilder.AlterColumn<uint>(
                name: "StatusId",
                table: "reservation",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "int unsigned",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_reservation_status_statusId",
                table: "reservation",
                column: "StatusId",
                principalTable: "status",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservation_status_statusId",
                table: "reservation");

            migrationBuilder.AlterColumn<uint>(
                name: "StatusId",
                table: "reservation",
                type: "int unsigned",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.AddForeignKey(
                name: "FK_reservation_status_statusId",
                table: "reservation",
                column: "StatusId",
                principalTable: "status",
                principalColumn: "StatusId");
        }
    }
}
