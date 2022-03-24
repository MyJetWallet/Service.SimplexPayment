using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    public partial class FeeAndReceivedAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BlockchainFee",
                schema: "simplex",
                table: "intentions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivedAmount",
                schema: "simplex",
                table: "intentions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_intentions_BlockchainTxHash",
                schema: "simplex",
                table: "intentions",
                column: "BlockchainTxHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_intentions_BlockchainTxHash",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropColumn(
                name: "BlockchainFee",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropColumn(
                name: "ReceivedAmount",
                schema: "simplex",
                table: "intentions");
        }
    }
}
