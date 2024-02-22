using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class BusinessFavorite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouriteBusiness",
                columns: table => new
                {
                    FavouriteClassifiedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteBusiness", x => x.FavouriteClassifiedId);
                    table.ForeignKey(
                        name: "FK_FavouriteBusiness_ClassifiedBusiness_ClassifiedBusinessId",
                        column: x => x.ClassifiedBusinessId,
                        principalTable: "ClassifiedBusiness",
                        principalColumn: "ClassifiedBusinessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteBusiness_ClassifiedBusinessId",
                table: "FavouriteBusiness",
                column: "ClassifiedBusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteBusiness");
        }
    }
}
