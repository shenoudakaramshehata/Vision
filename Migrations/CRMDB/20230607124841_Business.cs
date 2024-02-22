using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class Business : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ClassifiedBusiness",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ClassifiedBusiness",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "ClassifiedBusiness",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "ClassifiedBusiness",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "ClassifiedBusiness");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ClassifiedBusiness");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "ClassifiedBusiness");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "ClassifiedBusiness");
        }
    }
}
