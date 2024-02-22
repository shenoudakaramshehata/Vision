using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class AdzModelsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ClassifiedAdsCategoryTitleEn",
                table: "ClassifiedAdsCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassifiedAdsCategoryTitleAr",
                table: "ClassifiedAdsCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassifiedAdsCategoryPic",
                table: "ClassifiedAdsCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdTemplateFieldCaptionEn",
                table: "AdTemplateConfig",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdTemplateFieldCaptionAr",
                table: "AdTemplateConfig",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ClassifiedAd",
                columns: table => new
                {
                    ClassifiedAdId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublishDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UseId = table.Column<string>(type: "text", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    ClassifiedAdsCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifiedAd", x => x.ClassifiedAdId);
                    table.ForeignKey(
                        name: "FK_ClassifiedAd_ClassifiedAdsCategory",
                        column: x => x.ClassifiedAdsCategoryId,
                        principalTable: "ClassifiedAdsCategory",
                        principalColumn: "ClassifiedAdsCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "AdContent",
                columns: table => new
                {
                    AdContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassifiedAdId = table.Column<long>(type: "bigint", nullable: false),
                    AdTemplateConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdContent", x => x.AdContentId);
                    table.ForeignKey(
                        name: "FK_AdContent_AdTemplateConfig_AdTemplateConfigId",
                        column: x => x.AdTemplateConfigId,
                        principalTable: "AdTemplateConfig",
                        principalColumn: "AdTemplateConfigId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdContent_ClassifiedAd",
                        column: x => x.ClassifiedAdId,
                        principalTable: "ClassifiedAd",
                        principalColumn: "ClassifiedAdId");
                });

            migrationBuilder.CreateTable(
                name: "AdContentValue",
                columns: table => new
                {
                    AdContentValueId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdContentId = table.Column<long>(type: "bigint", nullable: false),
                    ContentValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdContentValue", x => x.AdContentValueId);
                    table.ForeignKey(
                        name: "FK_AdContentValue_AdContent",
                        column: x => x.AdContentId,
                        principalTable: "AdContent",
                        principalColumn: "AdContentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdContent_AdTemplateConfigId",
                table: "AdContent",
                column: "AdTemplateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AdContent_ClassifiedAdId",
                table: "AdContent",
                column: "ClassifiedAdId");

            migrationBuilder.CreateIndex(
                name: "IX_AdContentValue_AdContentId",
                table: "AdContentValue",
                column: "AdContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassifiedAd_ClassifiedAdsCategoryId",
                table: "ClassifiedAd",
                column: "ClassifiedAdsCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdContentValue");

            migrationBuilder.DropTable(
                name: "AdContent");

            migrationBuilder.DropTable(
                name: "ClassifiedAd");

            migrationBuilder.AlterColumn<string>(
                name: "ClassifiedAdsCategoryTitleEn",
                table: "ClassifiedAdsCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ClassifiedAdsCategoryTitleAr",
                table: "ClassifiedAdsCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ClassifiedAdsCategoryPic",
                table: "ClassifiedAdsCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdTemplateFieldCaptionEn",
                table: "AdTemplateConfig",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "AdTemplateFieldCaptionAr",
                table: "AdTemplateConfig",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);
        }
    }
}
