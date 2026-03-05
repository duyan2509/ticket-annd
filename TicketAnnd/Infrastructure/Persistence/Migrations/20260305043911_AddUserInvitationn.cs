using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInvitationn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseAt",
                table: "invitations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "invitations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_invitations_UserId",
                table: "invitations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_invitations_users_UserId",
                table: "invitations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invitations_users_UserId",
                table: "invitations");

            migrationBuilder.DropIndex(
                name: "IX_invitations_UserId",
                table: "invitations");

            migrationBuilder.DropColumn(
                name: "ResponseAt",
                table: "invitations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "invitations");
        }
    }
}
