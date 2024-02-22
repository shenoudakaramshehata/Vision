using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class shoppingCartProductPriceNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductPrices_ProductPriceId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_ProductPrices_ProductPriceId",
                table: "ShoppingCarts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductPriceId",
                table: "ShoppingCarts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductPriceId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductPrices_ProductPriceId",
                table: "OrderItems",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_ProductPrices_ProductPriceId",
                table: "ShoppingCarts",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductPrices_ProductPriceId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_ProductPrices_ProductPriceId",
                table: "ShoppingCarts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductPriceId",
                table: "ShoppingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductPriceId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductPrices_ProductPriceId",
                table: "OrderItems",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_ProductPrices_ProductPriceId",
                table: "ShoppingCarts",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
