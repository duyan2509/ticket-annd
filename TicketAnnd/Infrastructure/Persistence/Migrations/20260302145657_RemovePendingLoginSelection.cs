using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePendingLoginSelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pending_login_selections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pending_login_selections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_login_selections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pending_login_selections_Token",
                table: "pending_login_selections",
                column: "Token",
                unique: true);
        }
    }
}
