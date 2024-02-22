using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateProductModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplateConfigs_ProductCategories_ProductCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropIndex(
                name: "IX_ProductTemplateConfigs_ProductCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.AddColumn<int>(
                name: "BusinessCategoryId",
                table: "ProductTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ClassifiedBusinessId",
                table: "ProductCategories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateConfigs_BusinessCategoryId",
                table: "ProductTemplateConfigs",
                column: "BusinessCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ClassifiedBusinessId",
                table: "ProductCategories",
                column: "ClassifiedBusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ClassifiedBusiness_ClassifiedBusinessId",
                table: "ProductCategories",
                column: "ClassifiedBusinessId",
                principalTable: "ClassifiedBusiness",
                principalColumn: "ClassifiedBusinessId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "ProductTemplateConfigs",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "BusinessCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ClassifiedBusiness_ClassifiedBusinessId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropIndex(
                name: "IX_ProductTemplateConfigs_BusinessCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_ClassifiedBusinessId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "BusinessCategoryId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "ClassifiedBusinessId",
                table: "ProductCategories");

            migrationBuilder.AddColumn<long>(
                name: "ProductCategoryId",
                table: "ProductTemplateConfigs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateConfigs_ProductCategoryId",
                table: "ProductTemplateConfigs",
                column: "ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplateConfigs_ProductCategories_ProductCategoryId",
                table: "ProductTemplateConfigs",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "ProductCategoryId");
        }
    }
}
