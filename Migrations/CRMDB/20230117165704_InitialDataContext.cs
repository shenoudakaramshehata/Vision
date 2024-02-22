using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class InitialDataContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassifiedAdsCategory",
                columns: table => new
                {
                    ClassifiedAdsCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassifiedAdsCategoryTitleAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClassifiedAdsCategoryTitleEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClassifiedAdsCategorySortOrder = table.Column<int>(type: "int", nullable: true),
                    ClassifiedAdsCategoryPic = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClassifiedAdsCategoryIsActive = table.Column<bool>(type: "bit", nullable: true),
                    ClassifiedAdsCategoryDescAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClassifiedAdsCategoryDescEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClassifiedAdsCategoryParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifiedAdsCategory", x => x.ClassifiedAdsCategoryId);
                    table.ForeignKey(
                        name: "FK_ClassifiedAdsCategory_ClassifiedAdsCategory",
                        column: x => x.ClassifiedAdsCategoryParentId,
                        principalTable: "ClassifiedAdsCategory",
                        principalColumn: "ClassifiedAdsCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "FieldType",
                columns: table => new
                {
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldType", x => x.FieldTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AdTemplateConfig",
                columns: table => new
                {
                    AdTemplateConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    AdTemplateFieldCaptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AdTemplateFieldCaptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ClassifiedAdsCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdTemplateConfig", x => x.AdTemplateConfigId);
                    table.ForeignKey(
                        name: "FK_AdTemplateConfig_ClassifiedAdsCategory",
                        column: x => x.ClassifiedAdsCategoryId,
                        principalTable: "ClassifiedAdsCategory",
                        principalColumn: "ClassifiedAdsCategoryId");
                    table.ForeignKey(
                        name: "FK_AdTemplateConfig_FieldType",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldType",
                        principalColumn: "FieldTypeId");
                });

            migrationBuilder.CreateTable(
                name: "AdTemplateOption",
                columns: table => new
                {
                    AdTemplateOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdTemplateConfigId = table.Column<int>(type: "int", nullable: false),
                    OptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    OptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdTemplateOption", x => x.AdTemplateOptionId);
                    table.ForeignKey(
                        name: "FK_AdTemplateOption_AdTemplateConfig",
                        column: x => x.AdTemplateConfigId,
                        principalTable: "AdTemplateConfig",
                        principalColumn: "AdTemplateConfigId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdTemplateConfig_ClassifiedAdsCategoryId",
                table: "AdTemplateConfig",
                column: "ClassifiedAdsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AdTemplateConfig_FieldTypeId",
                table: "AdTemplateConfig",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdTemplateOption_AdTemplateConfigId",
                table: "AdTemplateOption",
                column: "AdTemplateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassifiedAdsCategory_ClassifiedAdsCategoryParentId",
                table: "ClassifiedAdsCategory",
                column: "ClassifiedAdsCategoryParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdTemplateOption");

            migrationBuilder.DropTable(
                name: "AdTemplateConfig");

            migrationBuilder.DropTable(
                name: "ClassifiedAdsCategory");

            migrationBuilder.DropTable(
                name: "FieldType");
        }
    }
}
