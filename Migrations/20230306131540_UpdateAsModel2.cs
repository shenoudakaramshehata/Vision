using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations
{
    public partial class UpdateAsModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5bd52a90-fba7-483f-aa28-3587f5175eee");

            migrationBuilder.AddColumn<int>(
                name: "AvailableClassified",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7031feee-d1ce-4941-8cd3-61f4c30ca256", "6f5b0080-b759-4134-ab9f-86d357c6f306", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7031feee-d1ce-4941-8cd3-61f4c30ca256");

            migrationBuilder.DropColumn(
                name: "AvailableClassified",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5bd52a90-fba7-483f-aa28-3587f5175eee", "8cf39bf1-66a6-4984-8031-e409b5823bf4", "Admin", "ADMIN" });
        }
    }
}
