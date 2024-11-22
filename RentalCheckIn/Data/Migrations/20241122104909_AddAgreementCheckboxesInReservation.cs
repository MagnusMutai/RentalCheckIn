using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAgreementCheckboxesInReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AgreeEnergyConsumption",
                table: "reservation",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AgreeTerms",
                table: "reservation",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReceivedKeys",
                table: "reservation",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreeEnergyConsumption",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "AgreeTerms",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "ReceivedKeys",
                table: "reservation");
        }
    }
}
