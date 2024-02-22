using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class city_and_Area : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Cities",
                newName: "CityOrderIndex");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "ClassifiedAd",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "ClassifiedAd",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CityIsActive",
                table: "Cities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    AreaTlAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaTlEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaIsActive = table.Column<bool>(type: "bit", nullable: false),
                    AreaOrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_Areas_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CityId",
                table: "Areas",
                column: "CityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "ClassifiedAd");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ClassifiedAd");

            migrationBuilder.DropColumn(
                name: "CityIsActive",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "CityOrderIndex",
                table: "Cities",
                newName: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
