using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class BussinessOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Deliverycost",
                table: "ClassifiedBusiness",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "BussinessPlans",
                columns: table => new
                {
                    BussinessPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanTlAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanTlEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    DurationInMonth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BussinessPlans", x => x.BussinessPlanId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    CustomerAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Piece = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avenue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApartmentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.CustomerAddressId);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    ProductQty = table.Column<int>(type: "int", nullable: false),
                    ProductTotal = table.Column<double>(type: "float", nullable: false),
                    Deliverycost = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.ShoppingCartId);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusiniessSubscriptions",
                columns: table => new
                {
                    BusiniessSubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BussinessPlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaymentID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusiniessSubscriptions", x => x.BusiniessSubscriptionId);
                    table.ForeignKey(
                        name: "FK_BusiniessSubscriptions_BussinessPlans_BussinessPlanId",
                        column: x => x.BussinessPlanId,
                        principalTable: "BussinessPlans",
                        principalColumn: "BussinessPlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusiniessSubscriptions_ClassifiedBusiness_ClassifiedBusinessId",
                        column: x => x.ClassifiedBusinessId,
                        principalTable: "ClassifiedBusiness",
                        principalColumn: "ClassifiedBusinessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusiniessSubscriptions_PaymentMehods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMehods",
                        principalColumn: "PaymentMethodId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerAddressId = table.Column<int>(type: "int", nullable: true),
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeliverd = table.Column<bool>(type: "bit", nullable: true),
                    OrderTotal = table.Column<double>(type: "float", nullable: false),
                    Deliverycost = table.Column<double>(type: "float", nullable: true),
                    OrderNet = table.Column<double>(type: "float", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    UniqeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_ClassifiedBusiness_ClassifiedBusinessId",
                        column: x => x.ClassifiedBusinessId,
                        principalTable: "ClassifiedBusiness",
                        principalColumn: "ClassifiedBusinessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_CustomerAddresses_CustomerAddressId",
                        column: x => x.CustomerAddressId,
                        principalTable: "CustomerAddresses",
                        principalColumn: "CustomerAddressId");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    ProductQuantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemExtraProducts",
                columns: table => new
                {
                    OrderItemExtraProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProductExtraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemExtraProducts", x => x.OrderItemExtraProductId);
                    table.ForeignKey(
                        name: "FK_OrderItemExtraProducts_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItemExtraProducts_ProductExtras_ProductExtraId",
                        column: x => x.ProductExtraId,
                        principalTable: "ProductExtras",
                        principalColumn: "ProductExtraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusiniessSubscriptions_BussinessPlanId",
                table: "BusiniessSubscriptions",
                column: "BussinessPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BusiniessSubscriptions_ClassifiedBusinessId",
                table: "BusiniessSubscriptions",
                column: "ClassifiedBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusiniessSubscriptions_PaymentMethodId",
                table: "BusiniessSubscriptions",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemExtraProducts_OrderItemId",
                table: "OrderItemExtraProducts",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemExtraProducts_ProductExtraId",
                table: "OrderItemExtraProducts",
                column: "ProductExtraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClassifiedBusinessId",
                table: "Orders",
                column: "ClassifiedBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerAddressId",
                table: "Orders",
                column: "CustomerAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProductId",
                table: "ShoppingCarts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusiniessSubscriptions");

            migrationBuilder.DropTable(
                name: "OrderItemExtraProducts");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "BussinessPlans");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "Deliverycost",
                table: "ClassifiedBusiness");
        }
    }
}
