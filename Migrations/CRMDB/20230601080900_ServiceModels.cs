using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class ServiceModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceCatagories",
                columns: table => new
                {
                    ServiceCatagoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCatagoryTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceCatagoryTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ClassifiedBusinessId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCatagories", x => x.ServiceCatagoryId);
                    table.ForeignKey(
                        name: "FK_ServiceCatagories_ClassifiedBusiness_ClassifiedBusinessId",
                        column: x => x.ClassifiedBusinessId,
                        principalTable: "ClassifiedBusiness",
                        principalColumn: "ClassifiedBusinessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    ServiceTypeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTypeTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTypeTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.ServiceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    MainPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceCatagoryId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                    table.ForeignKey(
                        name: "FK_Services_ServiceCatagories_ServiceCatagoryId",
                        column: x => x.ServiceCatagoryId,
                        principalTable: "ServiceCatagories",
                        principalColumn: "ServiceCatagoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Services_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTemplateConfigs",
                columns: table => new
                {
                    ServiceTemplateConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    ServiceTemplateFieldCaptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTemplateFieldCaptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ValidationMessageAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationMessageEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTemplateConfigs", x => x.ServiceTemplateConfigId);
                    table.ForeignKey(
                        name: "FK_ServiceTemplateConfigs_FieldType_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldType",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceTemplateConfigs_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceImages",
                columns: table => new
                {
                    ServiceImageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceImages", x => x.ServiceImageId);
                    table.ForeignKey(
                        name: "FK_ServiceImages_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContents",
                columns: table => new
                {
                    ServiceContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTemplateConfigId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContents", x => x.ServiceContentId);
                    table.ForeignKey(
                        name: "FK_ServiceContents_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceContents_ServiceTemplateConfigs_ServiceTemplateConfigId",
                        column: x => x.ServiceTemplateConfigId,
                        principalTable: "ServiceTemplateConfigs",
                        principalColumn: "ServiceTemplateConfigId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTemplateOptions",
                columns: table => new
                {
                    ServiceTemplateOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTemplateConfigId = table.Column<int>(type: "int", nullable: true),
                    OptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTemplateOptions", x => x.ServiceTemplateOptionId);
                    table.ForeignKey(
                        name: "FK_ServiceTemplateOptions_ServiceTemplateConfigs_ServiceTemplateConfigId",
                        column: x => x.ServiceTemplateConfigId,
                        principalTable: "ServiceTemplateConfigs",
                        principalColumn: "ServiceTemplateConfigId");
                });

            migrationBuilder.CreateTable(
                name: "ServiceContentValues",
                columns: table => new
                {
                    ServiceContentValueId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceContentId = table.Column<long>(type: "bigint", nullable: false),
                    ContentValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContentValues", x => x.ServiceContentValueId);
                    table.ForeignKey(
                        name: "FK_ServiceContentValues_ServiceContents_ServiceContentId",
                        column: x => x.ServiceContentId,
                        principalTable: "ServiceContents",
                        principalColumn: "ServiceContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCatagories_ClassifiedBusinessId",
                table: "ServiceCatagories",
                column: "ClassifiedBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContents_ServiceId",
                table: "ServiceContents",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContents_ServiceTemplateConfigId",
                table: "ServiceContents",
                column: "ServiceTemplateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContentValues_ServiceContentId",
                table: "ServiceContentValues",
                column: "ServiceContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceImages_ServiceId",
                table: "ServiceImages",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCatagoryId",
                table: "Services",
                column: "ServiceCatagoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceTypeId",
                table: "Services",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTemplateConfigs_FieldTypeId",
                table: "ServiceTemplateConfigs",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTemplateConfigs_ServiceTypeId",
                table: "ServiceTemplateConfigs",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTemplateOptions_ServiceTemplateConfigId",
                table: "ServiceTemplateOptions",
                column: "ServiceTemplateConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceContentValues");

            migrationBuilder.DropTable(
                name: "ServiceImages");

            migrationBuilder.DropTable(
                name: "ServiceTemplateOptions");

            migrationBuilder.DropTable(
                name: "ServiceContents");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ServiceTemplateConfigs");

            migrationBuilder.DropTable(
                name: "ServiceCatagories");

            migrationBuilder.DropTable(
                name: "ServiceTypes");
        }
    }
}
