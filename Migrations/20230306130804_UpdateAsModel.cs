using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations
{
    public partial class UpdateAsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2c45370-72a9-4212-93d6-b75b3129642f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5bd52a90-fba7-483f-aa28-3587f5175eee", "8cf39bf1-66a6-4984-8031-e409b5823bf4", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5bd52a90-fba7-483f-aa28-3587f5175eee");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b2c45370-72a9-4212-93d6-b75b3129642f", "7dba8e53-93db-4b76-b01e-81b19376cde0", "Admin", "ADMIN" });
        }
    }
}
