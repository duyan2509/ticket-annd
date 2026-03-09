using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefractorLeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "team_members");

            migrationBuilder.AddColumn<Guid>(
                name: "LeaderId",
                table: "teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_LeaderId",
                table: "teams",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_teams_team_members_LeaderId",
                table: "teams",
                column: "LeaderId",
                principalTable: "team_members",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_teams_team_members_LeaderId",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "IX_teams_LeaderId",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "teams");

            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "team_members",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
