using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateInClassifiedAdModelMig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ClassifiedAd",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainPic",
                table: "ClassifiedAd",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Price",
                table: "ClassifiedAd",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "ClassifiedAd",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "ClassifiedAd",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdsImages",
                columns: table => new
                {
                    AdsImageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassifiedAdId = table.Column<long>(type: "bigint", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdsImages", x => x.AdsImageId);
                    table.ForeignKey(
                        name: "FK_AdsImages_ClassifiedAd_ClassifiedAdId",
                        column: x => x.ClassifiedAdId,
                        principalTable: "ClassifiedAd",
                        principalColumn: "ClassifiedAdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdsImages_ClassifiedAdId",
                table: "AdsImages",
                column: "ClassifiedAdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdsImages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ClassifiedAd");

            migrationBuilder.DropColumn(
                name: "MainPic",
                table: "ClassifiedAd");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ClassifiedAd");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "ClassifiedAd");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "ClassifiedAd");
        }
    }
}
