using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefractorAgent2Team : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ticket_assigns_agents_AgentId",
                table: "ticket_assigns");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_picks_agents_AgentId",
                table: "ticket_picks");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_agents_AgentId",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_user_agents_agents_AgentId",
                table: "user_agents");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "user_agents",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_user_agents_UserId_AgentId",
                table: "user_agents",
                newName: "IX_user_agents_UserId_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_user_agents_AgentId",
                table: "user_agents",
                newName: "IX_user_agents_TeamId");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "tickets",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_AgentId",
                table: "tickets",
                newName: "IX_tickets_TeamId");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "ticket_picks",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_ticket_picks_AgentId",
                table: "ticket_picks",
                newName: "IX_ticket_picks_TeamId");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "ticket_assigns",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_ticket_assigns_AgentId",
                table: "ticket_assigns",
                newName: "IX_ticket_assigns_TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_assigns_agents_TeamId",
                table: "ticket_assigns",
                column: "TeamId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_picks_agents_TeamId",
                table: "ticket_picks",
                column: "TeamId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_agents_TeamId",
                table: "tickets",
                column: "TeamId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_user_agents_agents_TeamId",
                table: "user_agents",
                column: "TeamId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ticket_assigns_agents_TeamId",
                table: "ticket_assigns");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_picks_agents_TeamId",
                table: "ticket_picks");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_agents_TeamId",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_user_agents_agents_TeamId",
                table: "user_agents");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "user_agents",
                newName: "AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_user_agents_UserId_TeamId",
                table: "user_agents",
                newName: "IX_user_agents_UserId_AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_user_agents_TeamId",
                table: "user_agents",
                newName: "IX_user_agents_AgentId");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "tickets",
                newName: "AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_TeamId",
                table: "tickets",
                newName: "IX_tickets_AgentId");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "ticket_picks",
                newName: "AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_ticket_picks_TeamId",
                table: "ticket_picks",
                newName: "IX_ticket_picks_AgentId");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "ticket_assigns",
                newName: "AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_ticket_assigns_TeamId",
                table: "ticket_assigns",
                newName: "IX_ticket_assigns_AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_assigns_agents_AgentId",
                table: "ticket_assigns",
                column: "AgentId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_picks_agents_AgentId",
                table: "ticket_picks",
                column: "AgentId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_agents_AgentId",
                table: "tickets",
                column: "AgentId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_user_agents_agents_AgentId",
                table: "user_agents",
                column: "AgentId",
                principalTable: "agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
