using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class Step : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "BusinessTemplateConfigs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AdTemplateConfigId",
                table: "AdTemplateOption",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Step",
                table: "BusinessTemplateConfigs");

            migrationBuilder.AlterColumn<int>(
                name: "AdTemplateConfigId",
                table: "AdTemplateOption",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
