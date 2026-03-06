using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaRuleNameFixTicketSlaRuleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_sla_policies_SlaPolicyId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_sla_rules_SlaPolicyId",
                table: "sla_rules");

            migrationBuilder.RenameColumn(
                name: "SlaPolicyId",
                table: "tickets",
                newName: "SlaRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_SlaPolicyId",
                table: "tickets",
                newName: "IX_tickets_SlaRuleId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "sla_rules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_sla_rules_SlaPolicyId_Name",
                table: "sla_rules",
                columns: new[] { "SlaPolicyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_invitations_Email",
                table: "invitations",
                column: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_sla_rules_SlaRuleId",
                table: "tickets",
                column: "SlaRuleId",
                principalTable: "sla_rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_sla_rules_SlaRuleId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_sla_rules_SlaPolicyId_Name",
                table: "sla_rules");

            migrationBuilder.DropIndex(
                name: "IX_invitations_Email",
                table: "invitations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "sla_rules");

            migrationBuilder.RenameColumn(
                name: "SlaRuleId",
                table: "tickets",
                newName: "SlaPolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_SlaRuleId",
                table: "tickets",
                newName: "IX_tickets_SlaPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_sla_rules_SlaPolicyId",
                table: "sla_rules",
                column: "SlaPolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_sla_policies_SlaPolicyId",
                table: "tickets",
                column: "SlaPolicyId",
                principalTable: "sla_policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
