using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    public partial class Fee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseFiatAmount",
                schema: "simplex",
                table: "intentions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                schema: "simplex",
                table: "intentions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFiatAmount",
                schema: "simplex",
                table: "intentions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseFiatAmount",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropColumn(
                name: "Fee",
                schema: "simplex",
                table: "intentions");

            migrationBuilder.DropColumn(
                name: "TotalFiatAmount",
                schema: "simplex",
                table: "intentions");
        }
    }
}
