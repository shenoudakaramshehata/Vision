using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class usersubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusiniessSubscriptions_ClassifiedBusiness_ClassifiedBusinessId",
                table: "BusiniessSubscriptions");

            migrationBuilder.AlterColumn<long>(
                name: "ClassifiedBusinessId",
                table: "BusiniessSubscriptions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BusiniessSubscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusiniessSubscriptions_ClassifiedBusiness_ClassifiedBusinessId",
                table: "BusiniessSubscriptions",
                column: "ClassifiedBusinessId",
                principalTable: "ClassifiedBusiness",
                principalColumn: "ClassifiedBusinessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusiniessSubscriptions_ClassifiedBusiness_ClassifiedBusinessId",
                table: "BusiniessSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BusiniessSubscriptions");

            migrationBuilder.AlterColumn<long>(
                name: "ClassifiedBusinessId",
                table: "BusiniessSubscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusiniessSubscriptions_ClassifiedBusiness_ClassifiedBusinessId",
                table: "BusiniessSubscriptions",
                column: "ClassifiedBusinessId",
                principalTable: "ClassifiedBusiness",
                principalColumn: "ClassifiedBusinessId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
