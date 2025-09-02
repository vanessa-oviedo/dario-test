using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wheelzy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InicialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buyer",
                columns: table => new
                {
                    BuyerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buyer", x => x.BuyerId);
                });

            migrationBuilder.CreateTable(
                name: "CarMake",
                columns: table => new
                {
                    MakeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarMake", x => x.MakeId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "ZipCode",
                columns: table => new
                {
                    ZipCode = table.Column<string>(type: "char(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipCode", x => x.ZipCode);
                });

            migrationBuilder.CreateTable(
                name: "CarModel",
                columns: table => new
                {
                    ModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MakeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModel", x => x.ModelId);
                    table.ForeignKey(
                        name: "FK_CarModel_CarMake_MakeId",
                        column: x => x.MakeId,
                        principalTable: "CarMake",
                        principalColumn: "MakeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerZipCoverage",
                columns: table => new
                {
                    BuyerZipCoverageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    ZipCode = table.Column<string>(type: "char(5)", nullable: false),
                    DefaultQuoteAmount = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerZipCoverage", x => x.BuyerZipCoverageId);
                    table.ForeignKey(
                        name: "FK_BuyerZipCoverage_Buyer_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Buyer",
                        principalColumn: "BuyerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerZipCoverage_ZipCode_ZipCode",
                        column: x => x.ZipCode,
                        principalTable: "ZipCode",
                        principalColumn: "ZipCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    ZipCode = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                    Balance = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customer_ZipCode_ZipCode",
                        column: x => x.ZipCode,
                        principalTable: "ZipCode",
                        principalColumn: "ZipCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarSubmodel",
                columns: table => new
                {
                    SubmodelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSubmodel", x => x.SubmodelId);
                    table.ForeignKey(
                        name: "FK_CarSubmodel_CarModel_ModelId",
                        column: x => x.ModelId,
                        principalTable: "CarModel",
                        principalColumn: "ModelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    CarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    SubmodelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.CarId);
                    table.CheckConstraint("CK_Car_YearRange", "[Year] >= 1900 AND [Year] <= 2100");
                    table.ForeignKey(
                        name: "FK_Car_CarSubmodel_SubmodelId",
                        column: x => x.SubmodelId,
                        principalTable: "CarSubmodel",
                        principalColumn: "SubmodelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCars",
                columns: table => new
                {
                    CustomerCarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCars", x => x.CustomerCarId);
                    table.ForeignKey(
                        name: "FK_CustomerCars_Car_CarId",
                        column: x => x.CarId,
                        principalTable: "Car",
                        principalColumn: "CarId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCars_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "money", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    DueAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ExternalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.InvoiceId);
                    table.CheckConstraint("CK_Invoice_PaidDate", "([IsPaid]=(0) AND [PaidAt] IS NULL) OR ([IsPaid]=(1) AND [PaidAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Invoice_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    ZipCode = table.Column<string>(type: "char(5)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CurrentStatusId = table.Column<int>(type: "int", nullable: true),
                    CurrentStatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStatusChangedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentOrderBuyerQuoteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.CheckConstraint("CK_Order_PickedUpDate", "([CurrentStatusId] IS NULL OR [CurrentStatusId] <> 3 OR [CurrentStatusDate] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Order_Car_CarId",
                        column: x => x.CarId,
                        principalTable: "Car",
                        principalColumn: "CarId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_OrderStatus_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_ZipCode_ZipCode",
                        column: x => x.ZipCode,
                        principalTable: "ZipCode",
                        principalColumn: "ZipCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderBuyerQuote",
                columns: table => new
                {
                    OrderBuyerQuoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    BuyerZipCoverageId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "money", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBuyerQuote", x => x.OrderBuyerQuoteId);
                    table.ForeignKey(
                        name: "FK_OrderBuyerQuote_BuyerZipCoverage_BuyerZipCoverageId",
                        column: x => x.BuyerZipCoverageId,
                        principalTable: "BuyerZipCoverage",
                        principalColumn: "BuyerZipCoverageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderBuyerQuote_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusHistory",
                columns: table => new
                {
                    OrderStatusHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistory", x => x.OrderStatusHistoryId);
                    table.CheckConstraint("CK_CaseStatusHistory_PickedUpDate", "([StatusId] <> 3 OR [StatusDate] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OrderStatusHistory_OrderStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistory_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buyer_Name",
                table: "Buyer",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerZipCoverage_Zip",
                table: "BuyerZipCoverage",
                column: "ZipCode");

            migrationBuilder.CreateIndex(
                name: "UQ_Buyer_Zip",
                table: "BuyerZipCoverage",
                columns: new[] { "BuyerId", "ZipCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Car_SubmodelId",
                table: "Car",
                column: "SubmodelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarMake_Name",
                table: "CarMake",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarModel_MakeId",
                table: "CarModel",
                column: "MakeId");

            migrationBuilder.CreateIndex(
                name: "UQ_CarModel",
                table: "CarModel",
                columns: new[] { "MakeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarSubmodel_ModelId",
                table: "CarSubmodel",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "UQ_CarSubmodel",
                table: "CarSubmodel",
                columns: new[] { "ModelId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customer",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ZipCode",
                table: "Customer",
                column: "ZipCode");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCars_CarId",
                table: "CustomerCars",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCars_CustomerId",
                table: "CustomerCars",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CustomerId_IssuedAt",
                table: "Invoice",
                columns: new[] { "CustomerId", "IssuedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_OrderId",
                table: "Invoice",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CarId",
                table: "Order",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CurrentOrderBuyerQuoteId",
                table: "Order",
                columns: new[] { "CurrentOrderBuyerQuoteId", "OrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CurrentStatusId",
                table: "Order",
                column: "CurrentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                table: "Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ZipCode",
                table: "Order",
                column: "ZipCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBuyerQuote_BuyerZipCoverageId",
                table: "OrderBuyerQuote",
                column: "BuyerZipCoverageId");

            migrationBuilder.CreateIndex(
                name: "UQ_Quote_Per_Case_Buyer",
                table: "OrderBuyerQuote",
                columns: new[] { "OrderId", "BuyerZipCoverageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_OrderBuyerQuote_Id_OrderId",
                table: "OrderBuyerQuote",
                columns: new[] { "OrderBuyerQuoteId", "OrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_Name",
                table: "OrderStatus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistory_OrderId",
                table: "OrderStatusHistory",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistory_StatusId",
                table: "OrderStatusHistory",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Order_OrderId",
                table: "Invoice",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderBuyerQuote_CurrentOrderBuyerQuoteId",
                table: "Order",
                column: "CurrentOrderBuyerQuoteId",
                principalTable: "OrderBuyerQuote",
                principalColumn: "OrderBuyerQuoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyerZipCoverage_Buyer_BuyerId",
                table: "BuyerZipCoverage");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyerZipCoverage_ZipCode_ZipCode",
                table: "BuyerZipCoverage");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_ZipCode_ZipCode",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_ZipCode_ZipCode",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarSubmodel_SubmodelId",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Car_CarId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderBuyerQuote_Order_OrderId",
                table: "OrderBuyerQuote");

            migrationBuilder.DropTable(
                name: "CustomerCars");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "OrderStatusHistory");

            migrationBuilder.DropTable(
                name: "Buyer");

            migrationBuilder.DropTable(
                name: "ZipCode");

            migrationBuilder.DropTable(
                name: "CarSubmodel");

            migrationBuilder.DropTable(
                name: "CarModel");

            migrationBuilder.DropTable(
                name: "CarMake");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "OrderBuyerQuote");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "BuyerZipCoverage");
        }
    }
}
