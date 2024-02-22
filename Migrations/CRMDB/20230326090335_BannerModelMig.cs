using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class BannerModelMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntityTitle",
                table: "EntityTypes",
                newName: "EntityTitleEn");

            migrationBuilder.AddColumn<string>(
                name: "EntityTitleAr",
                table: "EntityTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BannerPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    BannerIsActive = table.Column<bool>(type: "bit", nullable: false),
                    BannerOrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.BannerId);
                    table.ForeignKey(
                        name: "FK_Banners_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_EntityTypeId",
                table: "Banners",
                column: "EntityTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropColumn(
                name: "EntityTitleAr",
                table: "EntityTypes");

            migrationBuilder.RenameColumn(
                name: "EntityTitleEn",
                table: "EntityTypes",
                newName: "EntityTitle");
        }
    }
}
