using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    public partial class TxHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockchainTxHash",
                schema: "simplex",
                table: "intentions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockchainTxHash",
                schema: "simplex",
                table: "intentions");
        }
    }
}
