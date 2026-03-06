using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaRuleNamePolicyIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sla_rules_SlaPolicyId_Name",
                table: "sla_rules");

            migrationBuilder.CreateIndex(
                name: "IX_sla_rules_SlaPolicyId_Name",
                table: "sla_rules",
                columns: new[] { "SlaPolicyId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sla_rules_SlaPolicyId_Name",
                table: "sla_rules");

            migrationBuilder.CreateIndex(
                name: "IX_sla_rules_SlaPolicyId_Name",
                table: "sla_rules",
                columns: new[] { "SlaPolicyId", "Name" });
        }
    }
}
