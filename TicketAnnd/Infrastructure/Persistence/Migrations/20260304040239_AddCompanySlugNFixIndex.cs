using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAnnd.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanySlugNFixIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_companies_Name",
                table: "companies");

            // Add Slug column as nullable 
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE companies
SET ""Slug"" = regexp_replace(lower(""Name""), '[^a-z0-9]+','-','g');
UPDATE companies
SET ""Slug"" = trim(both '-' from ""Slug"");
");

            // For any duplicate slugs, append a short id suffix to ensure uniqueness
            migrationBuilder.Sql(@"
WITH dup AS (
  SELECT ""Slug"" FROM companies GROUP BY ""Slug"" HAVING COUNT(*) > 1
)
UPDATE companies
SET ""Slug"" = ""Slug"" || '-' || substring(CAST(""Id"" AS text) from 1 for 8)
WHERE ""Slug"" IN (SELECT ""Slug"" FROM dup);
");

            // Make Slug NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_Slug",
                table: "companies",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_companies_Slug",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "companies");

            migrationBuilder.CreateIndex(
                name: "IX_companies_Name",
                table: "companies",
                column: "Name",
                unique: true);
        }
    }
}
