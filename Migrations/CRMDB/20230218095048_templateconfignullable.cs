using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class templateconfignullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessTemplateOptions_BusinessTemplateConfigs_BusinessTemplateConfigId",
                table: "BusinessTemplateOptions");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessTemplateConfigId",
                table: "BusinessTemplateOptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessTemplateOptions_BusinessTemplateConfigs_BusinessTemplateConfigId",
                table: "BusinessTemplateOptions",
                column: "BusinessTemplateConfigId",
                principalTable: "BusinessTemplateConfigs",
                principalColumn: "BusinessTemplateConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessTemplateOptions_BusinessTemplateConfigs_BusinessTemplateConfigId",
                table: "BusinessTemplateOptions");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessTemplateConfigId",
                table: "BusinessTemplateOptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessTemplateOptions_BusinessTemplateConfigs_BusinessTemplateConfigId",
                table: "BusinessTemplateOptions",
                column: "BusinessTemplateConfigId",
                principalTable: "BusinessTemplateConfigs",
                principalColumn: "BusinessTemplateConfigId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
