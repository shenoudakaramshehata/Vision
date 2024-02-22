using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class BusinessAreaandCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "ClassifiedBusiness",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "ClassifiedBusiness",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "ClassifiedBusiness");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ClassifiedBusiness");
        }
    }
}
