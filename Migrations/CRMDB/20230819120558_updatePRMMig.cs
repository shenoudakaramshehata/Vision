using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updatePRMMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Products",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Products",
                newName: "TitleAr");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Products",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "TitleAr",
                table: "Products",
                newName: "Location");
        }
    }
}
