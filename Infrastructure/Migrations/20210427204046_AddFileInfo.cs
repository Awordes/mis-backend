using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddFileInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VetisStatementId",
                schema: "mis",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MercuryFileInfo",
                schema: "mis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MercuryFileInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VetisStatementId",
                schema: "mis",
                table: "AspNetUsers",
                column: "VetisStatementId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MercuryFileInfo_VetisStatementId",
                schema: "mis",
                table: "AspNetUsers",
                column: "VetisStatementId",
                principalSchema: "mis",
                principalTable: "MercuryFileInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MercuryFileInfo_VetisStatementId",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MercuryFileInfo",
                schema: "mis");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VetisStatementId",
                schema: "mis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VetisStatementId",
                schema: "mis",
                table: "AspNetUsers");
        }
    }
}
