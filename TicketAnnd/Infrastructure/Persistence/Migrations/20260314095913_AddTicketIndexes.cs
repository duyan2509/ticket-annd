using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_tickets_company_deleted_category",
                table: "tickets",
                columns: new[] { "CompanyId", "IsDeleted", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "idx_tickets_company_deleted_status",
                table: "tickets",
                columns: new[] { "CompanyId", "IsDeleted", "Status" });

            migrationBuilder.CreateIndex(
                name: "idx_tickets_company_deleted_team_createdat",
                table: "tickets",
                columns: new[] { "CompanyId", "IsDeleted", "TeamId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tickets_company_deleted_category",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "idx_tickets_company_deleted_status",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "idx_tickets_company_deleted_team_createdat",
                table: "tickets");
        }
    }
}
