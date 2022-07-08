using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    public partial class add_indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_intentions_ClientId",
                schema: "simplex",
                table: "intentions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_intentions_CreationTime",
                schema: "simplex",
                table: "intentions",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_intentions_FromCurrency",
                schema: "simplex",
                table: "intentions",
                column: "FromCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_intentions_Status",
                schema: "simplex",
                table: "intentions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_intentions_ToAsset",
                schema: "simplex",
                table: "intentions",
                column: "ToAsset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_intentions_ClientId",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropIndex(
                name: "IX_intentions_CreationTime",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropIndex(
                name: "IX_intentions_FromCurrency",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropIndex(
                name: "IX_intentions_Status",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropIndex(
                name: "IX_intentions_ToAsset",
                schema: "simplex",
                table: "intentions");
        }
    }
}
