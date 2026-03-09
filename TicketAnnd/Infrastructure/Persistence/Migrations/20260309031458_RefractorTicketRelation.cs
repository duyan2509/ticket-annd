using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefractorTicketRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_ticket_assigns_TicketAssignId",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_users_CustomerId",
                table: "tickets");

            migrationBuilder.DropTable(
                name: "ticket_assigns");

            migrationBuilder.DropTable(
                name: "ticket_picks");

            migrationBuilder.DropIndex(
                name: "IX_tickets_TicketAssignId",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "FirstResponseDueAt",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "ResolutionDueAt",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "TicketAssignId",
                table: "tickets");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "tickets",
                newName: "RaiserId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_CustomerId",
                table: "tickets",
                newName: "IX_tickets_RaiserId");

            migrationBuilder.AddColumn<int>(
                name: "SlaFirstResponseMinutes",
                table: "tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SlaResolutionMinutes",
                table: "tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "team_members",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_users_RaiserId",
                table: "tickets",
                column: "RaiserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_users_RaiserId",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "SlaFirstResponseMinutes",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "SlaResolutionMinutes",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "team_members");

            migrationBuilder.RenameColumn(
                name: "RaiserId",
                table: "tickets",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_RaiserId",
                table: "tickets",
                newName: "IX_tickets_CustomerId");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstResponseDueAt",
                table: "tickets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolutionDueAt",
                table: "tickets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "TicketAssignId",
                table: "tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ticket_assigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_assigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ticket_assigns_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_assigns_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ticket_picks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_picks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ticket_picks_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_picks_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_TicketAssignId",
                table: "tickets",
                column: "TicketAssignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_assigns_TeamId",
                table: "ticket_assigns",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_assigns_TicketId",
                table: "ticket_assigns",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_picks_TeamId",
                table: "ticket_picks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_picks_TicketId",
                table: "ticket_picks",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_ticket_assigns_TicketAssignId",
                table: "tickets",
                column: "TicketAssignId",
                principalTable: "ticket_assigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_users_CustomerId",
                table: "tickets",
                column: "CustomerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
