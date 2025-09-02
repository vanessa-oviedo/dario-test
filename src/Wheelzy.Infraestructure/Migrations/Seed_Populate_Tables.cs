using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace Wheelzy.Infrastructure.Migrations
{
    public partial class Seed_Populate_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------------------------
            // ZIP CODES
            // ---------------------------
            migrationBuilder.InsertData(
                table: "ZipCode",
                column: "ZipCode",
                values: new object[]
                {
                "10001","10002","10003","02139","15213",
                "33101","60601","73301","90001","94105"
                });

            // ---------------------------
            // CAR MAKES / MODELS / SUBMODELS
            // ---------------------------
            migrationBuilder.InsertData(
                table: "CarMake",
                columns: new[] { "MakeId", "Name" },
                values: new object[,]
                {
                { 1, "Ford" },
                { 2, "Toyota" },
                { 3, "Honda" }
                });

            migrationBuilder.InsertData(
                table: "CarModel",
                columns: new[] { "ModelId", "MakeId", "Name" },
                values: new object[,]
                {
                { 1, 1, "Focus" },
                { 2, 2, "Camry" },
                { 3, 3, "Civic" }
                });

            migrationBuilder.InsertData(
                table: "CarSubmodel",
                columns: new[] { "SubmodelId", "ModelId", "Name" },
                values: new object[,]
                {
                { 1, 1, "SE" },
                { 2, 2, "LE" },
                { 3, 3, "EX" }
                });

            // ---------------------------
            // CARS
            // ---------------------------
            migrationBuilder.InsertData(
                table: "Car",
                columns: new[] { "CarId", "Year", "SubmodelId" },
                values: new object[,]
                {
                { 1, 2018, 1 }, { 2, 2019, 1 }, { 3, 2020, 1 },
                { 4, 2018, 2 }, { 5, 2019, 2 }, { 6, 2020, 2 },
                { 7, 2018, 3 }, { 8, 2019, 3 }, { 9, 2020, 3 },
                { 10, 2021, 3 }
                });

            // ---------------------------
            // CUSTOMERS
            // ---------------------------
            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "CustomerId", "Name", "ZipCode", "Balance" },
                values: new object[,]
                {
                { 1, "Alice Johnson", "10001", 0.00m },
                { 2, "Bob Smith", "10002", 0.00m },
                { 3, "Carol Davis", "10003", 0.00m },
                { 4, "David Wilson", "02139", 0.00m },
                { 5, "Eve Thompson", "15213", 0.00m },
                { 6, "Frank Miller", "33101", 0.00m },
                { 7, "Grace Lee", "60601", 0.00m },
                { 8, "Henry Brown", "73301", 0.00m },
                { 9, "Ivy Martinez", "90001", 0.00m },
                { 10, "Jack Anderson", "94105", 0.00m }
                });

            // ---------------------------
            // ORDER STATUS (los 4 pedidos)
            // ---------------------------
            migrationBuilder.InsertData(
                table: "OrderStatus",
                columns: new[] { "StatusId", "Name" },
                values: new object[,]
                {
                { 1, "Pending Acceptance" },
                { 2, "Accepted" },
                { 3, "Picked Up" },
                { 4, "Closed" }
                });

            // ---------------------------
            // ORDERS (10)
            // ---------------------------
            var createdBase = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[]
                {
                "OrderId", "CustomerId", "CarId", "ZipCode",
                "CreatedAt", "CurrentStatusId", "CurrentStatusDate",
                "CurrentStatusChangedBy", "CurrentOrderBuyerQuoteId"
                },
                values: new object[,]
                {
                { 1, 1, 1, "10001", createdBase.AddDays(0), 1, createdBase.AddDays(0), "system", null },
                { 2, 2, 2, "10002", createdBase.AddDays(1), 1, createdBase.AddDays(1), "system", null },
                { 3, 3, 3, "10003", createdBase.AddDays(2), 2, createdBase.AddDays(2), "system", null },
                { 4, 4, 4, "02139", createdBase.AddDays(3), 2, createdBase.AddDays(3), "system", null },
                { 5, 5, 5, "15213", createdBase.AddDays(4), 3, createdBase.AddDays(4), "system", null },
                { 6, 6, 6, "33101", createdBase.AddDays(5), 3, createdBase.AddDays(5), "system", null },
                { 7, 7, 7, "60601", createdBase.AddDays(6), 4, createdBase.AddDays(6), "system", null },
                { 8, 8, 8, "73301", createdBase.AddDays(7), 4, createdBase.AddDays(7), "system", null },
                { 9, 9, 9, "90001", createdBase.AddDays(8), 1, createdBase.AddDays(8), "system", null },
                { 10, 10, 10, "94105", createdBase.AddDays(9), 2, createdBase.AddDays(9), "system", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Orders
            for (int id = 1; id <= 10; id++)
                migrationBuilder.DeleteData("Order", "OrderId", id);

            // OrderStatus
            for (int id = 1; id <= 4; id++)
                migrationBuilder.DeleteData("OrderStatus", "StatusId", id);

            // Customers
            for (int id = 1; id <= 10; id++)
                migrationBuilder.DeleteData("Customer", "CustomerId", id);

            // Cars
            for (int id = 1; id <= 10; id++)
                migrationBuilder.DeleteData("Car", "CarId", id);

            // CarSubmodels
            migrationBuilder.DeleteData("CarSubmodel", "SubmodelId", 1);
            migrationBuilder.DeleteData("CarSubmodel", "SubmodelId", 2);
            migrationBuilder.DeleteData("CarSubmodel", "SubmodelId", 3);

            // CarModels
            migrationBuilder.DeleteData("CarModel", "ModelId", 1);
            migrationBuilder.DeleteData("CarModel", "ModelId", 2);
            migrationBuilder.DeleteData("CarModel", "ModelId", 3);

            // CarMakes
            migrationBuilder.DeleteData("CarMake", "MakeId", 1);
            migrationBuilder.DeleteData("CarMake", "MakeId", 2);
            migrationBuilder.DeleteData("CarMake", "MakeId", 3);

            // ZipCodes
            string[] zips = { "10001", "10002", "10003", "02139", "15213", "33101", "60601", "73301", "90001", "94105" };
            foreach (var z in zips)
                migrationBuilder.DeleteData("ZipCode", "ZipCode", z);
        }
    }
}
