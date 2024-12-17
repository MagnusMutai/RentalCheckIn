using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthenticatorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "AuthenticatorId",
                table: "lhost",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "Authenticator",
                columns: table => new
                {
                    AuthenticatorId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authenticator", x => x.AuthenticatorId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_lhost_AuthenticatorId",
                table: "lhost",
                column: "AuthenticatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_lhost_Authenticator_AuthenticatorId",
                table: "lhost",
                column: "AuthenticatorId",
                principalTable: "Authenticator",
                principalColumn: "AuthenticatorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lhost_Authenticator_AuthenticatorId",
                table: "lhost");

            migrationBuilder.DropTable(
                name: "Authenticator");

            migrationBuilder.DropIndex(
                name: "IX_lhost_AuthenticatorId",
                table: "lhost");

            migrationBuilder.DropColumn(
                name: "AuthenticatorId",
                table: "lhost");
        }
    }
}
