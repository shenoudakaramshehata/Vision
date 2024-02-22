using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class WalletModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMehods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodNameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodNameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodPic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMehods", x => x.PaymentMethodId);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    WalletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NumberOfClassifed = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.WalletId);
                });

            migrationBuilder.CreateTable(
                name: "WalletSubscriptions",
                columns: table => new
                {
                    WalletSubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    PaymentID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PaymentMehodPaymentMethodId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletSubscriptions", x => x.WalletSubscriptionId);
                    table.ForeignKey(
                        name: "FK_WalletSubscriptions_PaymentMehods_PaymentMehodPaymentMethodId",
                        column: x => x.PaymentMehodPaymentMethodId,
                        principalTable: "PaymentMehods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WalletSubscriptions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "WalletId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletSubscriptions_PaymentMehodPaymentMethodId",
                table: "WalletSubscriptions",
                column: "PaymentMehodPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSubscriptions_WalletId",
                table: "WalletSubscriptions",
                column: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletSubscriptions");

            migrationBuilder.DropTable(
                name: "PaymentMehods");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
