using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.SimplexPayment.Postgres.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "simplex");

            migrationBuilder.CreateTable(
                name: "intentions",
                schema: "simplex",
                columns: table => new
                {
                    QuoteId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    ClientIdHash = table.Column<string>(type: "text", nullable: true),
                    FromAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FromCurrency = table.Column<string>(type: "text", nullable: true),
                    ToAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ToAsset = table.Column<string>(type: "text", nullable: true),
                    ClientIp = table.Column<string>(type: "text", nullable: true),
                    PaymentId = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    ErrorText = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_intentions", x => x.QuoteId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "intentions",
                schema: "simplex");
        }
    }
}
