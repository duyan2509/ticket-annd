using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketIndexesFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tickets_company_deleted_createdat",
                table: "tickets");

            migrationBuilder.CreateIndex(
                name: "idx_tickets_company_deleted_createdat",
                table: "tickets",
                columns: new[] { "CompanyId", "IsDeleted", "CreatedAt" },
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tickets_company_deleted_createdat",
                table: "tickets");

            migrationBuilder.CreateIndex(
                name: "idx_tickets_company_deleted_createdat",
                table: "tickets",
                columns: new[] { "CompanyId", "IsDeleted", "CreatedAt" });
        }
    }
}
