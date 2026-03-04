using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "companies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_companies_OwnerId",
                table: "companies",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_companies_users_OwnerId",
                table: "companies",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_companies_users_OwnerId",
                table: "companies");

            migrationBuilder.DropIndex(
                name: "IX_companies_OwnerId",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "companies");
        }
    }
}
