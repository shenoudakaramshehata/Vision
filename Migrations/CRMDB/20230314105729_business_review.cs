using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class business_review : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AddListings_AddListingId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "AddListingId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "ClassifiedBusinessId",
                table: "Reviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ClassifiedBusinessId",
                table: "Reviews",
                column: "ClassifiedBusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AddListings_AddListingId",
                table: "Reviews",
                column: "AddListingId",
                principalTable: "AddListings",
                principalColumn: "AddListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ClassifiedBusiness_ClassifiedBusinessId",
                table: "Reviews",
                column: "ClassifiedBusinessId",
                principalTable: "ClassifiedBusiness",
                principalColumn: "ClassifiedBusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AddListings_AddListingId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ClassifiedBusiness_ClassifiedBusinessId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ClassifiedBusinessId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ClassifiedBusinessId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "AddListingId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AddListings_AddListingId",
                table: "Reviews",
                column: "AddListingId",
                principalTable: "AddListings",
                principalColumn: "AddListingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
