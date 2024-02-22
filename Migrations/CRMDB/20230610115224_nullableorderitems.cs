using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class nullableorderitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemExtraProducts_OrderItems_OrderItemId",
                table: "OrderItemExtraProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemExtraProducts_ProductExtras_ProductExtraId",
                table: "OrderItemExtraProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_Products_ProductId",
                table: "ProductPrices");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ProductPrices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ProductExtraId",
                table: "OrderItemExtraProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItemExtraProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemExtraProducts_OrderItems_OrderItemId",
                table: "OrderItemExtraProducts",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemExtraProducts_ProductExtras_ProductExtraId",
                table: "OrderItemExtraProducts",
                column: "ProductExtraId",
                principalTable: "ProductExtras",
                principalColumn: "ProductExtraId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_Products_ProductId",
                table: "ProductPrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemExtraProducts_OrderItems_OrderItemId",
                table: "OrderItemExtraProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemExtraProducts_ProductExtras_ProductExtraId",
                table: "OrderItemExtraProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_Products_ProductId",
                table: "ProductPrices");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ProductPrices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductExtraId",
                table: "OrderItemExtraProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItemExtraProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemExtraProducts_OrderItems_OrderItemId",
                table: "OrderItemExtraProducts",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemExtraProducts_ProductExtras_ProductExtraId",
                table: "OrderItemExtraProducts",
                column: "ProductExtraId",
                principalTable: "ProductExtras",
                principalColumn: "ProductExtraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_Products_ProductId",
                table: "ProductPrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
