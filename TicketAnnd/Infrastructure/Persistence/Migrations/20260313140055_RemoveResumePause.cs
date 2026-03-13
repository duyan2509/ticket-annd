using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResumePause : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeAt",
                table: "ticket_pauses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResumeAt",
                table: "ticket_pauses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
