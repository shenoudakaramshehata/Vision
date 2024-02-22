using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class SeddingServiceQuotationStatusModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceQuotationRequestStatuses",
                columns: new[] { "ServiceQuotationRequestStatusId", "StatusTitleAr", "StatusTitleEn" },
                values: new object[] { 1, "انشاء", "Initiated" });

            migrationBuilder.InsertData(
                table: "ServiceQuotationRequestStatuses",
                columns: new[] { "ServiceQuotationRequestStatusId", "StatusTitleAr", "StatusTitleEn" },
                values: new object[] { 2, "تم القبول", "Accepted" });

            migrationBuilder.InsertData(
                table: "ServiceQuotationRequestStatuses",
                columns: new[] { "ServiceQuotationRequestStatusId", "StatusTitleAr", "StatusTitleEn" },
                values: new object[] { 3, "تم الرفض", "Rejected" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceQuotationRequestStatuses",
                keyColumn: "ServiceQuotationRequestStatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceQuotationRequestStatuses",
                keyColumn: "ServiceQuotationRequestStatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceQuotationRequestStatuses",
                keyColumn: "ServiceQuotationRequestStatusId",
                keyValue: 3);
        }
    }
}
