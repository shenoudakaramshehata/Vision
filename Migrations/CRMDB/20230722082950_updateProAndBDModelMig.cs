using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateProAndBDModelMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsList",
                table: "ProductTemplateConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Reel",
                table: "ClassifiedBusiness",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsList",
                table: "BusinessTemplateConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsList",
                table: "ProductTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "Reel",
                table: "ClassifiedBusiness");

            migrationBuilder.DropColumn(
                name: "IsList",
                table: "BusinessTemplateConfigs");
        }
    }
}
