using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class BDOfferModelsMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BDOffers",
                columns: table => new
                {
                    BDOfferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfferDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BDOffers", x => x.BDOfferId);
                    table.ForeignKey(
                        name: "FK_BDOffers_ClassifiedBusiness_ClassifiedBusinessId",
                        column: x => x.ClassifiedBusinessId,
                        principalTable: "ClassifiedBusiness",
                        principalColumn: "ClassifiedBusinessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BDOfferImages",
                columns: table => new
                {
                    BDOfferImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BDOfferId = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BDOfferImages", x => x.BDOfferImageId);
                    table.ForeignKey(
                        name: "FK_BDOfferImages_BDOffers_BDOfferId",
                        column: x => x.BDOfferId,
                        principalTable: "BDOffers",
                        principalColumn: "BDOfferId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BDOfferImages_BDOfferId",
                table: "BDOfferImages",
                column: "BDOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_BDOffers_ClassifiedBusinessId",
                table: "BDOffers",
                column: "ClassifiedBusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BDOfferImages");

            migrationBuilder.DropTable(
                name: "BDOffers");
        }
    }
}
