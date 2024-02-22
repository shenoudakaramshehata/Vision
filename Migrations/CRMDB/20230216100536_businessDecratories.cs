using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class businessDecratories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CountryTlEn",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryTlAr",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CityTlEn",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CityTlAr",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "BusinessCategories",
                columns: table => new
                {
                    BusinessCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessCategoryTitleAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BusinessCategoryTitleEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BusinessCategorySortOrder = table.Column<int>(type: "int", nullable: true),
                    BusinessCategoryCategoryPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessCategoryIsActive = table.Column<bool>(type: "bit", nullable: false),
                    BusinessCategoryDescAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BusinessCategoryDescEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCategories", x => x.BusinessCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTemplateConfigs",
                columns: table => new
                {
                    BusinessTemplateConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    BusinessTemplateFieldCaptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BusinessTemplateFieldCaptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BusinessCategoryId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ValidationMessageAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationMessageEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTemplateConfigs", x => x.BusinessTemplateConfigId);
                    table.ForeignKey(
                        name: "FK_BusinessTemplateConfigs_BusinessCategories_BusinessCategoryId",
                        column: x => x.BusinessCategoryId,
                        principalTable: "BusinessCategories",
                        principalColumn: "BusinessCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessTemplateConfigs_FieldType_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldType",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassifiedBusiness",
                columns: table => new
                {
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UseId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    BusinessCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassifiedBusiness", x => x.ClassifiedBusinessId);
                    table.ForeignKey(
                        name: "FK_ClassifiedBusiness_BusinessCategories_BusinessCategoryId",
                        column: x => x.BusinessCategoryId,
                        principalTable: "BusinessCategories",
                        principalColumn: "BusinessCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessTemplateOptions",
                columns: table => new
                {
                    BusinessTemplateOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessTemplateConfigId = table.Column<int>(type: "int", nullable: false),
                    OptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    OptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTemplateOptions", x => x.BusinessTemplateOptionId);
                    table.ForeignKey(
                        name: "FK_BusinessTemplateOptions_BusinessTemplateConfigs_BusinessTemplateConfigId",
                        column: x => x.BusinessTemplateConfigId,
                        principalTable: "BusinessTemplateConfigs",
                        principalColumn: "BusinessTemplateConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessContents",
                columns: table => new
                {
                    BusinessContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessTemplateConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessContents", x => x.BusinessContentId);
                    table.ForeignKey(
                        name: "FK_BusinessContents_BusinessTemplateConfigs_BusinessTemplateConfigId",
                        column: x => x.BusinessTemplateConfigId,
                        principalTable: "BusinessTemplateConfigs",
                        principalColumn: "BusinessTemplateConfigId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessContents_ClassifiedBusiness_ClassifiedBusinessId",
                        column: x => x.ClassifiedBusinessId,
                        principalTable: "ClassifiedBusiness",
                        principalColumn: "ClassifiedBusinessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessContentValues",
                columns: table => new
                {
                    BusinessContentValueId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessContentId = table.Column<long>(type: "bigint", nullable: false),
                    ContentValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessContentValues", x => x.BusinessContentValueId);
                    table.ForeignKey(
                        name: "FK_BusinessContentValues_BusinessContents_BusinessContentId",
                        column: x => x.BusinessContentId,
                        principalTable: "BusinessContents",
                        principalColumn: "BusinessContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessContents_BusinessTemplateConfigId",
                table: "BusinessContents",
                column: "BusinessTemplateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessContents_ClassifiedBusinessId",
                table: "BusinessContents",
                column: "ClassifiedBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessContentValues_BusinessContentId",
                table: "BusinessContentValues",
                column: "BusinessContentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessTemplateConfigs_BusinessCategoryId",
                table: "BusinessTemplateConfigs",
                column: "BusinessCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessTemplateConfigs_FieldTypeId",
                table: "BusinessTemplateConfigs",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessTemplateOptions_BusinessTemplateConfigId",
                table: "BusinessTemplateOptions",
                column: "BusinessTemplateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassifiedBusiness_BusinessCategoryId",
                table: "ClassifiedBusiness",
                column: "BusinessCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessContentValues");

            migrationBuilder.DropTable(
                name: "BusinessTemplateOptions");

            migrationBuilder.DropTable(
                name: "BusinessContents");

            migrationBuilder.DropTable(
                name: "BusinessTemplateConfigs");

            migrationBuilder.DropTable(
                name: "ClassifiedBusiness");

            migrationBuilder.DropTable(
                name: "BusinessCategories");

            migrationBuilder.AlterColumn<string>(
                name: "CountryTlEn",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryTlAr",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CityTlEn",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CityTlAr",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
