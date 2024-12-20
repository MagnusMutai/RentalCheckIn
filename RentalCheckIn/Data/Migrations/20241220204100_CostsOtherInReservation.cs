using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class CostsOtherInReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostsOther",
                table: "reservation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostsOther",
                table: "reservation");
        }
    }
}
