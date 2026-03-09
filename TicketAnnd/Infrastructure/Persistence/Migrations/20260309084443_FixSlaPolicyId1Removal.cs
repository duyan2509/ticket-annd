using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSlaPolicyId1Removal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sla_rules_sla_policies_SlaPolicyId1",
                table: "sla_rules");

            migrationBuilder.DropIndex(
                name: "IX_sla_rules_SlaPolicyId1",
                table: "sla_rules");

            migrationBuilder.DropColumn(
                name: "SlaPolicyId1",
                table: "sla_rules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SlaPolicyId1",
                table: "sla_rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sla_rules_SlaPolicyId1",
                table: "sla_rules",
                column: "SlaPolicyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_sla_rules_sla_policies_SlaPolicyId1",
                table: "sla_rules",
                column: "SlaPolicyId1",
                principalTable: "sla_policies",
                principalColumn: "Id");
        }
    }
}
