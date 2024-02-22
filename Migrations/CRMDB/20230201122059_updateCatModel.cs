using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Migrations.CRMDB
{
    public partial class updateCatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryTitleAr",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryPic",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryTitleAr",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryPic",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryPic", "CategoryTitleAr", "CategoryTitleEn", "Description", "SortOrder", "Tags" },
                values: new object[,]
                {
                    { 1, null, null, "Business Support & Supplies", null, null, null },
                    { 2, null, null, "Automotive", null, null, null },
                    { 3, null, null, "Computers & Electronics", null, null, null },
                    { 4, null, null, "Construction & Contractors", null, null, null },
                    { 5, null, null, "Education", null, null, null },
                    { 6, null, null, "Entertainment", null, null, null },
                    { 7, null, null, "Food & Dining", null, null, null },
                    { 8, null, null, "Health & Medicine", null, null, null },
                    { 9, null, null, "Home & Garden", null, null, null },
                    { 10, null, null, "Legal & Financial", null, null, null },
                    { 11, null, null, "Manufacturing, Wholesale, Distribution", null, null, null },
                    { 12, null, null, "Merchants (Retail)", null, null, null },
                    { 13, null, null, "Miscellaneous", null, null, null },
                    { 14, null, null, "Personal Care & Services", null, null, null },
                    { 15, null, null, "Real Estate", null, null, null },
                    { 16, null, null, "Travel & Transportation", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "PageContents",
                columns: new[] { "PageContentId", "ContentAr", "ContentEn", "PageTitleAr", "PageTitleEn" },
                values: new object[,]
                {
                    { 1, "من نحن", "About Page", "من نحن", "About" },
                    { 2, "الشروط والاحكام", "Condition and Terms Page", "الشروط والاحكام", "Condition and Terms" },
                    { 3, "سياسة الخصوصية", "Privacy Policy Page", "سياسة الخصوصية", "Privacy Policy" }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "SubCategoryID", "CategoryId", "Description", "SortOrder", "SubCategoryPic", "SubCategoryTitleAr", "SubCategoryTitleEn", "Tags" },
                values: new object[,]
                {
                    { 1, 1, null, null, null, null, "Auto Accessories", null },
                    { 2, 1, null, null, null, null, "Auto Dealers – New", null },
                    { 3, 1, null, null, null, null, "Auto Dealers – Used", null },
                    { 4, 1, null, null, null, null, "Detail & Carwash", null },
                    { 5, 1, null, null, null, null, "Gas Stations", null },
                    { 6, 1, null, null, null, null, "Motorcycle Sales & Repair", null },
                    { 7, 1, null, null, null, null, "Rental & Leasing", null },
                    { 8, 1, null, null, null, null, "Service, Repair & Parts", null },
                    { 9, 1, null, null, null, null, "Towing", null },
                    { 10, 2, null, null, null, null, "Consultants", null },
                    { 11, 2, null, null, null, null, "Employment Agency", null },
                    { 12, 2, null, null, null, null, "Marketing & Communications", null },
                    { 13, 2, null, null, null, null, "Office Supplies", null },
                    { 14, 2, null, null, null, null, "Printing & Publishing", null },
                    { 15, 3, null, null, null, null, "Computer Programming & Support", null },
                    { 16, 3, null, null, null, null, "Consumer Electronics & Accessories", null },
                    { 17, 4, null, null, null, null, "Architects, Landscape Architects, Engineers & Surveyors", null },
                    { 18, 4, null, null, null, null, "Blasting & Demolition", null },
                    { 19, 4, null, null, null, null, "Construction Companies", null },
                    { 20, 4, null, null, null, null, "Electricians", null },
                    { 21, 4, null, null, null, null, "Engineer, Survey", null },
                    { 22, 4, null, null, null, null, "Environmental Assessments", null },
                    { 23, 4, null, null, null, null, "Inspectors", null },
                    { 24, 4, null, null, null, null, "Plaster & Concrete", null },
                    { 25, 4, null, null, null, null, "Plumbers", null },
                    { 26, 4, null, null, null, null, "Roofers", null },
                    { 27, 5, null, null, null, null, "Adult & Continuing Education", null },
                    { 28, 5, null, null, null, null, "Early Childhood Education", null },
                    { 29, 5, null, null, null, null, "Educational Resources", null },
                    { 30, 5, null, null, null, null, "Other Educational", null },
                    { 32, 6, null, null, null, null, "Artists, Writers", null },
                    { 33, 6, null, null, null, null, "Event Planners & Supplies", null },
                    { 34, 6, null, null, null, null, "Golf Courses", null },
                    { 35, 6, null, null, null, null, "Movies", null },
                    { 36, 6, null, null, null, null, "Productions", null },
                    { 37, 7, null, null, null, null, "Desserts, Catering & Supplies", null },
                    { 38, 7, null, null, null, null, "Fast Food & Carry Out", null },
                    { 39, 7, null, null, null, null, "Grocery, Beverage & Tobacco", null },
                    { 40, 7, null, null, null, null, "Restaurants", null },
                    { 41, 8, null, null, null, null, "Acupuncture", null },
                    { 42, 8, null, null, null, null, "Assisted Living & Home Health Care", null },
                    { 43, 8, null, null, null, null, "Audiologist", null }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "SubCategoryID", "CategoryId", "Description", "SortOrder", "SubCategoryPic", "SubCategoryTitleAr", "SubCategoryTitleEn", "Tags" },
                values: new object[,]
                {
                    { 44, 8, null, null, null, null, "Chiropractic", null },
                    { 45, 8, null, null, null, null, "Clinics & Medical Centers", null },
                    { 46, 8, null, null, null, null, "Dental ", null },
                    { 47, 8, null, null, null, null, "Diet I& Nutrition", null },
                    { 48, 8, null, null, null, null, "Laboratory, Imaging & Diagnostic", null },
                    { 49, 8, null, null, null, null, "Massage Therapy ", null },
                    { 50, 8, null, null, null, null, "Mental Health", null },
                    { 51, 8, null, null, null, null, "Nurse", null },
                    { 52, 8, null, null, null, null, "Optical", null },
                    { 53, 8, null, null, null, null, "Pharmacy, Drug & Vitamin Stores", null },
                    { 54, 8, null, null, null, null, "Physical Therapy", null },
                    { 55, 8, null, null, null, null, "Physicians & Assistants", null },
                    { 56, 8, null, null, null, null, "Podiatry", null },
                    { 57, 8, null, null, null, null, "Social Worker", null },
                    { 58, 8, null, null, null, null, "Animal Hospita", null },
                    { 59, 8, null, null, null, null, "Veterinary & Animal Surgeons ", null },
                    { 60, 9, null, null, null, null, "Antiques & Collectibles ", null },
                    { 61, 9, null, null, null, null, "Cleaning", null },
                    { 62, 9, null, null, null, null, "Crafts, Hobbies & Sports", null },
                    { 63, 9, null, null, null, null, "Flower Shops", null },
                    { 64, 9, null, null, null, null, "Home Furnishings", null },
                    { 65, 9, null, null, null, null, "Home Goods", null },
                    { 66, 9, null, null, null, null, "Home Improvements & Repairs", null },
                    { 67, 9, null, null, null, null, "Landscape & Lawn Service", null },
                    { 68, 9, null, null, null, null, "Pest Control", null },
                    { 69, 9, null, null, null, null, "Pool Supplies & Service", null },
                    { 70, 9, null, null, null, null, "Security System & Services", null },
                    { 71, 10, null, null, null, null, "Accountants ", null },
                    { 72, 10, null, null, null, null, "Attorneys", null },
                    { 73, 10, null, null, null, null, "Financial Institutions", null },
                    { 74, 10, null, null, null, null, "Financial Services", null },
                    { 75, 10, null, null, null, null, "Insurance", null },
                    { 76, 10, null, null, null, null, "Other Legal", null },
                    { 77, 11, null, null, null, null, "Distribution, Import/Export", null },
                    { 78, 11, null, null, null, null, "Manufacturing", null },
                    { 79, 11, null, null, null, null, "Wholesale", null },
                    { 80, 12, null, null, null, null, "Cards & Gifts", null },
                    { 81, 12, null, null, null, null, "Clothing & Accessories", null },
                    { 82, 12, null, null, null, null, "Department Stores, Sporting Goods", null },
                    { 83, 12, null, null, null, null, "General", null },
                    { 84, 12, null, null, null, null, "Jewelry", null },
                    { 85, 12, null, null, null, null, "Shoes", null }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "SubCategoryID", "CategoryId", "Description", "SortOrder", "SubCategoryPic", "SubCategoryTitleAr", "SubCategoryTitleEn", "Tags" },
                values: new object[,]
                {
                    { 86, 13, null, null, null, null, "Civic Groups", null },
                    { 87, 13, null, null, null, null, "Funeral Service Providers & Cemetaries", null },
                    { 88, 13, null, null, null, null, "Miscellaneous", null },
                    { 89, 13, null, null, null, null, "Utility Companies", null },
                    { 90, 14, null, null, null, null, "Animal Care & Supplies", null },
                    { 91, 14, null, null, null, null, "Barber & Beauty Salons", null },
                    { 92, 14, null, null, null, null, "Beauty Supplies", null },
                    { 93, 14, null, null, null, null, "Dry Cleaners & Laundromats", null },
                    { 94, 14, null, null, null, null, "Exercise & Fitness", null },
                    { 95, 14, null, null, null, null, "Massage & Body Works", null },
                    { 96, 14, null, null, null, null, "Nail Salons", null },
                    { 97, 14, null, null, null, null, "Shoe Repairs", null },
                    { 98, 15, null, null, null, null, "Agencies & Brokerage", null },
                    { 99, 14, null, null, null, null, "Tailors", null },
                    { 100, 15, null, null, null, null, "Agents & Brokers", null },
                    { 101, 15, null, null, null, null, "Apartment & Home Rental", null },
                    { 102, 15, null, null, null, null, "Mortgage Broker & Lender", null },
                    { 103, 15, null, null, null, null, "Property Management", null },
                    { 104, 15, null, null, null, null, "Title Company", null },
                    { 105, 16, null, null, null, null, "Hotel, Motel & Extended Stay", null },
                    { 106, 16, null, null, null, null, "Moving & Storage", null },
                    { 107, 16, null, null, null, null, "Packaging & Shipping", null },
                    { 108, 16, null, null, null, null, "Transportation", null },
                    { 109, 16, null, null, null, null, "Travel & Tourism", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PageContents",
                keyColumn: "PageContentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PageContents",
                keyColumn: "PageContentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PageContents",
                keyColumn: "PageContentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryID",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 16);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryTitleAr",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryPic",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryTitleAr",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryPic",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
