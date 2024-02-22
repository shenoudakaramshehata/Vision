using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class SeedingEntityTypeDataMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EntityTypes",
                columns: new[] { "EntityTypeId", "EntityTitleAr", "EntityTitleEn" },
                values: new object[] { 1, "إعلان", "Classified Ads" });

            migrationBuilder.InsertData(
                table: "EntityTypes",
                columns: new[] { "EntityTypeId", "EntityTitleAr", "EntityTitleEn" },
                values: new object[] { 2, "عمل", "Bussiness Directory" });

            migrationBuilder.InsertData(
                table: "EntityTypes",
                columns: new[] { "EntityTypeId", "EntityTitleAr", "EntityTitleEn" },
                values: new object[] { 3, "ملف شخصي", "Profile" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EntityTypes",
                keyColumn: "EntityTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EntityTypes",
                keyColumn: "EntityTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EntityTypes",
                keyColumn: "EntityTypeId",
                keyValue: 3);
        }
    }
}
