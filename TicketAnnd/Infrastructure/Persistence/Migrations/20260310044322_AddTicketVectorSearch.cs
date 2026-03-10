using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketVectorSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "tickets",
                type: "tsvector",
                nullable: false,
                computedColumnSql: "to_tsvector('english', \r\n            coalesce(\"Subject\", '') || ' ' || \r\n            coalesce(\"Body\", '')\r\n        )",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "idx_ticket_search",
                table: "tickets",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_ticket_search",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "tickets");
        }
    }
}
