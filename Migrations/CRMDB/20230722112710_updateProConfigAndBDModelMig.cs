using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateProConfigAndBDModelMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ProductTemplateOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasChild",
                table: "ProductTemplateConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ProductTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "ProductTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "BusinessTemplateOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasChild",
                table: "BusinessTemplateConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "BusinessTemplateConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ProductTemplateOptions");

            migrationBuilder.DropColumn(
                name: "HasChild",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "BusinessTemplateOptions");

            migrationBuilder.DropColumn(
                name: "HasChild",
                table: "BusinessTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "BusinessTemplateConfigs");
        }
    }
}
