using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "BusinessTemplateConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "ValidationMessageEn",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ValidationMessageAr",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "BusinessTemplateConfigs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRequired",
                table: "BusinessTemplateConfigs",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "BusinessTemplateFieldCaptionEn",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessTemplateFieldCaptionAr",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<int>(
                name: "BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "BusinessCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "BusinessTemplateConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "ValidationMessageEn",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ValidationMessageAr",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "BusinessTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRequired",
                table: "BusinessTemplateConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessTemplateFieldCaptionEn",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessTemplateFieldCaptionAr",
                table: "BusinessTemplateConfigs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                column: "BusinessCategoryId",
                principalTable: "BusinessCategories",
                principalColumn: "BusinessCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
