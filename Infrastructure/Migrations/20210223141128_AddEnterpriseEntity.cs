using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddEnterpriseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Enterprises",
                schema: "mis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MercuryId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enterprises_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "mis",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enterprises_UserId",
                schema: "mis",
                table: "Enterprises",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enterprises",
                schema: "mis");

            migrationBuilder.AddColumn<string>(
                name: "EnterpriseId",
                schema: "mis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
