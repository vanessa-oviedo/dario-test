using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wheelzy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBuyerAndBuyerZipCoverage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------------------------
            // BUYERS
            // ---------------------------
            migrationBuilder.InsertData(
                table: "Buyer",
                columns: new[] { "BuyerId", "Name" },
                values: new object[,]
                {
                    { 1, "AutoNation Inc." },
                    { 2, "CarMax LLC" },
                    { 3, "BestCars Buyers" }
                });

            // ---------------------------
            // BUYER ZIP COVERAGE
            //   Nota: ZipCode ya existe en el seed anterior
            // ---------------------------
            migrationBuilder.InsertData(
                table: "BuyerZipCoverage",
                columns: new[] { "BuyerZipCoverageId", "BuyerId", "ZipCode", "DefaultQuoteAmount" },
                values: new object[,]
                {
                    { 1, 1, "10001", 5000m },
                    { 2, 1, "10002", 5100m },
                    { 3, 1, "10003", 5200m },

                    { 4, 2, "02139", 6000m },
                    { 5, 2, "15213", 6100m },
                    { 6, 2, "33101", 6200m },

                    { 7, 3, "60601", 7000m },
                    { 8, 3, "73301", 7100m },
                    { 9, 3, "90001", 7200m },
                    { 10, 3, "94105", 7300m }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // BuyerZipCoverage
            for (int id = 1; id <= 10; id++)
                migrationBuilder.DeleteData(table: "BuyerZipCoverage", keyColumn: "BuyerZipCoverageId", keyValue: id);

            // Buyers
            for (int id = 1; id <= 3; id++)
                migrationBuilder.DeleteData(table: "Buyer", keyColumn: "BuyerId", keyValue: id);

        }
    }
}
