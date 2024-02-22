using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    Isactive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTemplateConfigs",
                columns: table => new
                {
                    ProductTemplateConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    ProductTemplateFieldCaptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProductTemplateFieldCaptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ValidationMessageAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationMessageEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTemplateConfigs", x => x.ProductTemplateConfigId);
                    table.ForeignKey(
                        name: "FK_ProductTemplateConfigs_FieldType_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldType",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTemplateConfigs_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "ProductContents",
                columns: table => new
                {
                    ProductContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTemplateConfigId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductContents", x => x.ProductContentId);
                    table.ForeignKey(
                        name: "FK_ProductContents_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductContents_ProductTemplateConfigs_ProductTemplateConfigId",
                        column: x => x.ProductTemplateConfigId,
                        principalTable: "ProductTemplateConfigs",
                        principalColumn: "ProductTemplateConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTemplateOptions",
                columns: table => new
                {
                    ProductTemplateOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTemplateConfigId = table.Column<int>(type: "int", nullable: true),
                    OptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    OptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTemplateOptions", x => x.ProductTemplateOptionId);
                    table.ForeignKey(
                        name: "FK_ProductTemplateOptions_ProductTemplateConfigs_ProductTemplateConfigId",
                        column: x => x.ProductTemplateConfigId,
                        principalTable: "ProductTemplateConfigs",
                        principalColumn: "ProductTemplateConfigId");
                });

            migrationBuilder.CreateTable(
                name: "ProductContentValues",
                columns: table => new
                {
                    ProductContentValueId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductContentId = table.Column<long>(type: "bigint", nullable: false),
                    ContentValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductContentValues", x => x.ProductContentValueId);
                    table.ForeignKey(
                        name: "FK_ProductContentValues_ProductContents_ProductContentId",
                        column: x => x.ProductContentId,
                        principalTable: "ProductContents",
                        principalColumn: "ProductContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductContents_ProductId",
                table: "ProductContents",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductContents_ProductTemplateConfigId",
                table: "ProductContents",
                column: "ProductTemplateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductContentValues_ProductContentId",
                table: "ProductContentValues",
                column: "ProductContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateConfigs_FieldTypeId",
                table: "ProductTemplateConfigs",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateConfigs_ProductCategoryId",
                table: "ProductTemplateConfigs",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateOptions_ProductTemplateConfigId",
                table: "ProductTemplateOptions",
                column: "ProductTemplateConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductContentValues");

            migrationBuilder.DropTable(
                name: "ProductTemplateOptions");

            migrationBuilder.DropTable(
                name: "ProductContents");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductTemplateConfigs");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
