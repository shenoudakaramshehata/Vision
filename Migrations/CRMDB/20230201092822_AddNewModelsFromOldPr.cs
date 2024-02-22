using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class AddNewModelsFromOldPr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryPic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendingDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryTlAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryTlEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.EntityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "FAQ",
                columns: table => new
                {
                    FAQId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQ", x => x.FAQId);
                });

            migrationBuilder.CreateTable(
                name: "FavouriteClassifieds",
                columns: table => new
                {
                    FavouriteClassifiedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassifiedAdId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteClassifieds", x => x.FavouriteClassifiedId);
                    table.ForeignKey(
                        name: "FK_FavouriteClassifieds_ClassifiedAd_ClassifiedAdId",
                        column: x => x.ClassifiedAdId,
                        principalTable: "ClassifiedAd",
                        principalColumn: "ClassifiedAdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavouriteProfiles",
                columns: table => new
                {
                    FavouriteProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteProfiles", x => x.FavouriteProfileId);
                });

            migrationBuilder.CreateTable(
                name: "FolowClassifieds",
                columns: table => new
                {
                    FolowClassifiedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassifiedAdId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolowClassifieds", x => x.FolowClassifiedId);
                    table.ForeignKey(
                        name: "FK_FolowClassifieds_ClassifiedAd_ClassifiedAdId",
                        column: x => x.ClassifiedAdId,
                        principalTable: "ClassifiedAd",
                        principalColumn: "ClassifiedAdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolowProfile",
                columns: table => new
                {
                    FolowProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolowProfile", x => x.FolowProfileId);
                });

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    PageContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.PageContentId);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    QuotationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassifiedAdId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.QuotationId);
                    table.ForeignKey(
                        name: "FK_Quotations_ClassifiedAd_ClassifiedAdId",
                        column: x => x.ClassifiedAdId,
                        principalTable: "ClassifiedAd",
                        principalColumn: "ClassifiedAdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoicialMidiaLinks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facebooklink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatsApplink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkedInlink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instgramlink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TwitterLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YoutubeLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoicialMidiaLinks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    SubCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubCategoryTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategoryTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategoryPic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.SubCategoryID);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddListings",
                columns: table => new
                {
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    MainLocataion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPeroson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    ListingBanner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListingLogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromoVideo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddListings", x => x.AddListingId);
                    table.ForeignKey(
                        name: "FK_AddListings_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddListings_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityTlAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityTlEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Long = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_Branches_AddListings_AddListingId",
                        column: x => x.AddListingId,
                        principalTable: "AddListings",
                        principalColumn: "AddListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favourites",
                columns: table => new
                {
                    FavouriteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourites", x => x.FavouriteId);
                    table.ForeignKey(
                        name: "FK_Favourites_AddListings_AddListingId",
                        column: x => x.AddListingId,
                        principalTable: "AddListings",
                        principalColumn: "AddListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folwers",
                columns: table => new
                {
                    FolwersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folwers", x => x.FolwersId);
                    table.ForeignKey(
                        name: "FK_Folwers_AddListings_AddListingId",
                        column: x => x.AddListingId,
                        principalTable: "AddListings",
                        principalColumn: "AddListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingPhotos_AddListings_AddListingId",
                        column: x => x.AddListingId,
                        principalTable: "AddListings",
                        principalColumn: "AddListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingVideos_AddListings_AddListingId",
                        column: x => x.AddListingId,
                        principalTable: "AddListings",
                        principalColumn: "AddListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddListingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_AddListings_AddListingId",
                        column: x => x.AddListingId,
                        principalTable: "AddListings",
                        principalColumn: "AddListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddListings_CategoryId",
                table: "AddListings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AddListings_CountryId",
                table: "AddListings",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_AddListingId",
                table: "Branches",
                column: "AddListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteClassifieds_ClassifiedAdId",
                table: "FavouriteClassifieds",
                column: "ClassifiedAdId");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_AddListingId",
                table: "Favourites",
                column: "AddListingId");

            migrationBuilder.CreateIndex(
                name: "IX_FolowClassifieds_ClassifiedAdId",
                table: "FolowClassifieds",
                column: "ClassifiedAdId");

            migrationBuilder.CreateIndex(
                name: "IX_Folwers_AddListingId",
                table: "Folwers",
                column: "AddListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingPhotos_AddListingId",
                table: "ListingPhotos",
                column: "AddListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingVideos_AddListingId",
                table: "ListingVideos",
                column: "AddListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ClassifiedAdId",
                table: "Quotations",
                column: "ClassifiedAdId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AddListingId",
                table: "Reviews",
                column: "AddListingId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.DropTable(
                name: "FAQ");

            migrationBuilder.DropTable(
                name: "FavouriteClassifieds");

            migrationBuilder.DropTable(
                name: "FavouriteProfiles");

            migrationBuilder.DropTable(
                name: "Favourites");

            migrationBuilder.DropTable(
                name: "FolowClassifieds");

            migrationBuilder.DropTable(
                name: "FolowProfile");

            migrationBuilder.DropTable(
                name: "Folwers");

            migrationBuilder.DropTable(
                name: "ListingPhotos");

            migrationBuilder.DropTable(
                name: "ListingVideos");

            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SoicialMidiaLinks");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "AddListings");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
