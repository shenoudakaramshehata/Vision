using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class WalletModels2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletSubscriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WalletSubscriptions",
                columns: table => new
                {
                    WalletSubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMehodPaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
