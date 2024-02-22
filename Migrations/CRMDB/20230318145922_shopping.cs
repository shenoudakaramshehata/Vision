using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class shopping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopingCartProductExtraFeatures",
                columns: table => new
                {
                    ShopingCartProductExtraFeaturesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductExtraId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ShoppingCartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopingCartProductExtraFeatures", x => x.ShopingCartProductExtraFeaturesId);
                    table.ForeignKey(
                        name: "FK_ShopingCartProductExtraFeatures_ProductExtras_ProductExtraId",
                        column: x => x.ProductExtraId,
                        principalTable: "ProductExtras",
                        principalColumn: "ProductExtraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopingCartProductExtraFeatures_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "ShoppingCartId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopingCartProductExtraFeatures_ProductExtraId",
                table: "ShopingCartProductExtraFeatures",
                column: "ProductExtraId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopingCartProductExtraFeatures_ShoppingCartId",
                table: "ShopingCartProductExtraFeatures",
                column: "ShoppingCartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopingCartProductExtraFeatures");
        }
    }
}
