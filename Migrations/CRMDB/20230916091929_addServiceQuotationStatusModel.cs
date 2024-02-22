using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class addServiceQuotationStatusModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceQuotationRequestStatusId",
                table: "ServiceQuotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceQuotationRequestStatuses",
                columns: table => new
                {
                    ServiceQuotationRequestStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceQuotationRequestStatuses", x => x.ServiceQuotationRequestStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceQuotations_ServiceQuotationRequestStatusId",
                table: "ServiceQuotations",
                column: "ServiceQuotationRequestStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceQuotations_ServiceQuotationRequestStatuses_ServiceQuotationRequestStatusId",
                table: "ServiceQuotations",
                column: "ServiceQuotationRequestStatusId",
                principalTable: "ServiceQuotationRequestStatuses",
                principalColumn: "ServiceQuotationRequestStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceQuotations_ServiceQuotationRequestStatuses_ServiceQuotationRequestStatusId",
                table: "ServiceQuotations");

            migrationBuilder.DropTable(
                name: "ServiceQuotationRequestStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ServiceQuotations_ServiceQuotationRequestStatusId",
                table: "ServiceQuotations");

            migrationBuilder.DropColumn(
                name: "ServiceQuotationRequestStatusId",
                table: "ServiceQuotations");
        }
    }
}
