using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    public partial class Indecies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_intentions_ClientIdHash",
                schema: "simplex",
                table: "intentions",
                column: "ClientIdHash");

            migrationBuilder.CreateIndex(
                name: "IX_intentions_PaymentId",
                schema: "simplex",
                table: "intentions",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_intentions_ClientIdHash",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropIndex(
                name: "IX_intentions_PaymentId",
                schema: "simplex",
                table: "intentions");
        }
    }
}
