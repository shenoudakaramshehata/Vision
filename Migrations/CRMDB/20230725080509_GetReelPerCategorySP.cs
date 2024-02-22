using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class GetReelPerCategorySP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[GetReelPerCategory] 
                        @ClassifiedAdsCategoryId int
                 
                AS
                BEGIN
                    SET NOCOUNT ON;
                   SELECT       dbo.ClassifiedAd.ClassifiedAdId, dbo.ClassifiedAd.PublishDate, dbo.ClassifiedAd.IsActive, dbo.ClassifiedAd.ClassifiedAdsCategoryId, dbo.ClassifiedAd.Description, dbo.ClassifiedAd.MainPic, dbo.ClassifiedAd.PhoneNumber, 
                         dbo.AspNetUsers.UserName, dbo.AspNetUsers.FullName, dbo.AspNetUsers.ProfilePicture, dbo.ClassifiedAd.Reel, dbo.ClassifiedAd.UseId
                        FROM            dbo.ClassifiedAd LEFT OUTER JOIN
                         dbo.AspNetUsers ON dbo.ClassifiedAd.UseId = dbo.AspNetUsers.Id
						 where dbo.ClassifiedAd.IsActive=1  and dbo.ClassifiedAd.ClassifiedAdsCategoryId=@ClassifiedAdsCategoryId 
                END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE dbo.GetReelPerCategory ");
        }
    }
}
