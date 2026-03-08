using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefractorAgent2TeamTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_agents_companies_CompanyId",
                table: "agents");

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

            migrationBuilder.DropForeignKey(
                name: "FK_user_agents_users_UserId",
                table: "user_agents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_agents",
                table: "user_agents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_agents",
                table: "agents");

            migrationBuilder.RenameTable(
                name: "user_agents",
                newName: "team_members");

            migrationBuilder.RenameTable(
                name: "agents",
                newName: "teams");

            migrationBuilder.RenameIndex(
                name: "IX_user_agents_UserId_TeamId",
                table: "team_members",
                newName: "IX_team_members_UserId_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_user_agents_TeamId",
                table: "team_members",
                newName: "IX_team_members_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_agents_CompanyId",
                table: "teams",
                newName: "IX_teams_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_team_members",
                table: "team_members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_teams",
                table: "teams",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_team_members_teams_TeamId",
                table: "team_members",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_members_users_UserId",
                table: "team_members",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_companies_CompanyId",
                table: "teams",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_assigns_teams_TeamId",
                table: "ticket_assigns",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_picks_teams_TeamId",
                table: "ticket_picks",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_teams_TeamId",
                table: "tickets",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_team_members_teams_TeamId",
                table: "team_members");

            migrationBuilder.DropForeignKey(
                name: "FK_team_members_users_UserId",
                table: "team_members");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_companies_CompanyId",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_assigns_teams_TeamId",
                table: "ticket_assigns");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_picks_teams_TeamId",
                table: "ticket_picks");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_teams_TeamId",
                table: "tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teams",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_team_members",
                table: "team_members");

            migrationBuilder.RenameTable(
                name: "teams",
                newName: "agents");

            migrationBuilder.RenameTable(
                name: "team_members",
                newName: "user_agents");

            migrationBuilder.RenameIndex(
                name: "IX_teams_CompanyId",
                table: "agents",
                newName: "IX_agents_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_UserId_TeamId",
                table: "user_agents",
                newName: "IX_user_agents_UserId_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_TeamId",
                table: "user_agents",
                newName: "IX_user_agents_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_agents",
                table: "agents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_agents",
                table: "user_agents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_agents_companies_CompanyId",
                table: "agents",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_user_agents_users_UserId",
                table: "user_agents",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
