using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class UpdateProductModelMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropIndex(
                name: "IX_ProductTemplateConfigs_BusinessCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "BusinessCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.AddColumn<long>(
                name: "ProductTypeId",
                table: "ProductTemplateConfigs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Products",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IsFixedPrice",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MainPic",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ProductTypeId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ProductExtras",
                columns: table => new
                {
                    ProductExtraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ExtraTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraDes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductExtras", x => x.ProductExtraId);
                    table.ForeignKey(
                        name: "FK_ProductExtras_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    ProductPriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductPriceTilteAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductPriceTilteEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ProductPriceDes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrices", x => x.ProductPriceId);
                    table.ForeignKey(
                        name: "FK_ProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    ProductTypeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.ProductTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateConfigs_ProductTypeId",
                table: "ProductTemplateConfigs",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductExtras_ProductId",
                table: "ProductExtras",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplateConfigs_ProductTypes_ProductTypeId",
                table: "ProductTemplateConfigs",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplateConfigs_ProductTypes_ProductTypeId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropTable(
                name: "ProductExtras");

            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductTemplateConfigs_ProductTypeId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "IsFixedPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MainPic",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "BusinessCategoryId",
                table: "ProductTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateConfigs_BusinessCategoryId",
                table: "ProductTemplateConfigs",
                column: "BusinessCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "ProductTemplateConfigs",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "BusinessCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
