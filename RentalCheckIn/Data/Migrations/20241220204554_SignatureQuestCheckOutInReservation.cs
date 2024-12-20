using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class SignatureQuestCheckOutInReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignatureQuestCheckOut",
                table: "reservation",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureQuestCheckOut",
                table: "reservation");
        }
    }
}
