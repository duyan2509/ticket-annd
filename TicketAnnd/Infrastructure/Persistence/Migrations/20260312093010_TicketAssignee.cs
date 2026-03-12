using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TicketAssignee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tickets_AssigneeId",
                table: "tickets",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_users_AssigneeId",
                table: "tickets",
                column: "AssigneeId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_users_AssigneeId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_tickets_AssigneeId",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "tickets");
        }
    }
}
