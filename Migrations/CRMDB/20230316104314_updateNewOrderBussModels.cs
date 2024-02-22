using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateNewOrderBussModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "ShoppingCarts",
                newName: "ItemPrice");

            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "OrderItems",
                newName: "ItemPrice");

            migrationBuilder.AddColumn<int>(
                name: "ProductPriceId",
                table: "ShoppingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApartmentNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avenue",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Lat",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lng",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderSerial",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Piece",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductPriceId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "OrderItemExtraProducts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProductPriceId",
                table: "ShoppingCarts",
                column: "ProductPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductPriceId",
                table: "OrderItems",
                column: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductPrices_ProductPriceId",
                table: "OrderItems",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_ProductPrices_ProductPriceId",
                table: "ShoppingCarts",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductPrices_ProductPriceId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_ProductPrices_ProductPriceId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ProductPriceId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductPriceId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductPriceId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Avenue",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Building",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderSerial",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Piece",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductPriceId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderItemExtraProducts");

            migrationBuilder.RenameColumn(
                name: "ItemPrice",
                table: "ShoppingCarts",
                newName: "ProductPrice");

            migrationBuilder.RenameColumn(
                name: "ItemPrice",
                table: "OrderItems",
                newName: "ProductPrice");
        }
    }
}
