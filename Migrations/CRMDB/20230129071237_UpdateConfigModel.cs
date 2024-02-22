using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class UpdateConfigModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "AdTemplateConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AdTemplateConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ValidationMessageAr",
                table: "AdTemplateConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationMessageEn",
                table: "AdTemplateConfig",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "AdTemplateConfig");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AdTemplateConfig");

            migrationBuilder.DropColumn(
                name: "ValidationMessageAr",
                table: "AdTemplateConfig");

            migrationBuilder.DropColumn(
                name: "ValidationMessageEn",
                table: "AdTemplateConfig");
        }
    }
}
