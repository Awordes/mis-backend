using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddUserCustomFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiLogin",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiPassword",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                schema: "mis",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditAllow",
                schema: "mis",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Inn",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuerId",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MercuryLogin",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MercuryPassword",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApiLogin",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApiPassword",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Contact",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Deleted",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EditAllow",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Inn",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IssuerId",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MercuryLogin",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MercuryPassword",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "mis",
                table: "AspNetUsers");
        }
    }
}
