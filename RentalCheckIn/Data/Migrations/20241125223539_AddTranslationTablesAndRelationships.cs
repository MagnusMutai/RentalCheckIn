using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalCheckIn.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationTablesAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApartmentTranslation",
                columns: table => new
                {
                    ApartmentTranslationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApartmentId = table.Column<uint>(type: "int unsigned", nullable: false),
                    LanguageId = table.Column<uint>(type: "int unsigned", nullable: false),
                    ApartmentName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentTranslation", x => x.ApartmentTranslationId);
                    table.ForeignKey(
                        name: "FK_ApartmentTranslation_apartment_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "apartment",
                        principalColumn: "ApartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApartmentTranslation_language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ReservationTranslation",
                columns: table => new
                {
                    ReservationTranslationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReservationId = table.Column<uint>(type: "int unsigned", nullable: false),
                    LanguageId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CheckInTime = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CheckOutTime = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTranslation", x => x.ReservationTranslationId);
                    table.ForeignKey(
                        name: "FK_ReservationTranslation_language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationTranslation_reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "StatusTranslation",
                columns: table => new
                {
                    StatusTranslationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StatusId = table.Column<uint>(type: "int unsigned", nullable: false),
                    LanguageId = table.Column<uint>(type: "int unsigned", nullable: false),
                    StatusLabel = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTranslation", x => x.StatusTranslationId);
                    table.ForeignKey(
                        name: "FK_StatusTranslation_language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatusTranslation_status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "status",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentTranslation_ApartmentId_LanguageId",
                table: "ApartmentTranslation",
                columns: new[] { "ApartmentId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentTranslation_LanguageId",
                table: "ApartmentTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTranslation_LanguageId",
                table: "ReservationTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTranslation_ReservationId_LanguageId",
                table: "ReservationTranslation",
                columns: new[] { "ReservationId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusTranslation_LanguageId",
                table: "StatusTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusTranslation_StatusId_LanguageId",
                table: "StatusTranslation",
                columns: new[] { "StatusId", "LanguageId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmentTranslation");

            migrationBuilder.DropTable(
                name: "ReservationTranslation");

            migrationBuilder.DropTable(
                name: "StatusTranslation");
        }
    }
}
