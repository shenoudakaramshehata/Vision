using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class BDSelfJoin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "BusinessTemplateConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassifiedBusiness_BusinessCategories_BusinessCategoryId",
                table: "ClassifiedBusiness");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessCategories",
                table: "BusinessCategories");

            migrationBuilder.RenameTable(
                name: "BusinessCategories",
                newName: "BusinessCategory");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "ClassifiedAd",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessCategoryParentId",
                table: "BusinessCategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessCategory",
                table: "BusinessCategory",
                column: "BusinessCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCategory_BusinessCategoryParentId",
                table: "BusinessCategory",
                column: "BusinessCategoryParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessCategory_BusinessCategory_BusinessCategoryParentId",
                table: "BusinessCategory",
                column: "BusinessCategoryParentId",
                principalTable: "BusinessCategory",
                principalColumn: "BusinessCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategory_BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategory",
                principalColumn: "BusinessCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassifiedBusiness_BusinessCategory_BusinessCategoryId",
                table: "ClassifiedBusiness",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategory",
                principalColumn: "BusinessCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessCategory_BusinessCategory_BusinessCategoryParentId",
                table: "BusinessCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategory_BusinessCategoryId",
                table: "BusinessTemplateConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassifiedBusiness_BusinessCategory_BusinessCategoryId",
                table: "ClassifiedBusiness");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessCategory",
                table: "BusinessCategory");

            migrationBuilder.DropIndex(
                name: "IX_BusinessCategory_BusinessCategoryParentId",
                table: "BusinessCategory");

            migrationBuilder.DropColumn(
                name: "BusinessCategoryParentId",
                table: "BusinessCategory");

            migrationBuilder.RenameTable(
                name: "BusinessCategory",
                newName: "BusinessCategories");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "ClassifiedAd",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessCategories",
                table: "BusinessCategories",
                column: "BusinessCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "BusinessCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassifiedBusiness_BusinessCategories_BusinessCategoryId",
                table: "ClassifiedBusiness",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "BusinessCategoryId");
        }
    }
}
