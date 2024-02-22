using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateAdTemplateOptionMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pic",
                table: "AdTemplateOption",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AdTemplateOption",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pic",
                table: "AdTemplateOption");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AdTemplateOption");
        }
    }
}
