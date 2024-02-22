using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class shopingcartextra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopingCartProductExtraFeatures_ShoppingCarts_ShoppingCartId",
                table: "ShopingCartProductExtraFeatures");

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "ShopingCartProductExtraFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopingCartProductExtraFeatures_ShoppingCarts_ShoppingCartId",
                table: "ShopingCartProductExtraFeatures",
                column: "ShoppingCartId",
                principalTable: "ShoppingCarts",
                principalColumn: "ShoppingCartId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopingCartProductExtraFeatures_ShoppingCarts_ShoppingCartId",
                table: "ShopingCartProductExtraFeatures");

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "ShopingCartProductExtraFeatures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopingCartProductExtraFeatures_ShoppingCarts_ShoppingCartId",
                table: "ShopingCartProductExtraFeatures",
                column: "ShoppingCartId",
                principalTable: "ShoppingCarts",
                principalColumn: "ShoppingCartId");
        }
    }
}
