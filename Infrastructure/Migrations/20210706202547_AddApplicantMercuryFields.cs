using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddApplicantMercuryFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MercuryLogin",
                schema: "mis",
                table: "Applicants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MercuryPassword",
                schema: "mis",
                table: "Applicants",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MercuryLogin",
                schema: "mis",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "MercuryPassword",
                schema: "mis",
                table: "Applicants");
        }
    }
}
