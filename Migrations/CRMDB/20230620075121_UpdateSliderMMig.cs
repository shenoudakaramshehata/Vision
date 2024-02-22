using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class UpdateSliderMMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "Sliders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntityTypeId",
                table: "Sliders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sliders_EntityTypeId",
                table: "Sliders",
                column: "EntityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sliders_EntityTypes_EntityTypeId",
                table: "Sliders",
                column: "EntityTypeId",
                principalTable: "EntityTypes",
                principalColumn: "EntityTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sliders_EntityTypes_EntityTypeId",
                table: "Sliders");

            migrationBuilder.DropIndex(
                name: "IX_Sliders_EntityTypeId",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "EntityTypeId",
                table: "Sliders");
        }
    }
}
