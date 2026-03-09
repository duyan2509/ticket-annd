using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTicketSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use existing SlaPolicyId column (mapped in the domain model)
            migrationBuilder.CreateIndex(
                name: "IX_sla_rules_SlaPolicyId",
                table: "sla_rules",
                column: "SlaPolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_sla_rules_sla_policies_SlaPolicyId",
                table: "sla_rules",
                column: "SlaPolicyId",
                principalTable: "sla_policies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sla_rules_sla_policies_SlaPolicyId",
                table: "sla_rules");

            migrationBuilder.DropIndex(
                name: "IX_sla_rules_SlaPolicyId",
                table: "sla_rules");
        }
    }
}
