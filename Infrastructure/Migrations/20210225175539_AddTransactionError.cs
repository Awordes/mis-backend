using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddTransactionError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                schema: "mis",
                table: "VsdProcessTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OperationId",
                schema: "mis",
                table: "VsdProcessTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VsdProcessTransactions_OperationId",
                schema: "mis",
                table: "VsdProcessTransactions",
                column: "OperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_VsdProcessTransactions_Operations_OperationId",
                schema: "mis",
                table: "VsdProcessTransactions",
                column: "OperationId",
                principalSchema: "mis",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VsdProcessTransactions_Operations_OperationId",
                schema: "mis",
                table: "VsdProcessTransactions");

            migrationBuilder.DropIndex(
                name: "IX_VsdProcessTransactions_OperationId",
                schema: "mis",
                table: "VsdProcessTransactions");

            migrationBuilder.DropColumn(
                name: "Error",
                schema: "mis",
                table: "VsdProcessTransactions");

            migrationBuilder.DropColumn(
                name: "OperationId",
                schema: "mis",
                table: "VsdProcessTransactions");
        }
    }
}
