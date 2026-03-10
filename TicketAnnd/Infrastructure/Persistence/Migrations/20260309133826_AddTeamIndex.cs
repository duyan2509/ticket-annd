using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_teams_CompanyId",
                table: "teams");

            migrationBuilder.CreateIndex(
                name: "IX_teams_CompanyId_Name",
                table: "teams",
                columns: new[] { "CompanyId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_teams_CompanyId_Name",
                table: "teams");

            migrationBuilder.CreateIndex(
                name: "IX_teams_CompanyId",
                table: "teams",
                column: "CompanyId");
        }
    }
}
