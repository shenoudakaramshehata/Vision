using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vision.Models;

#nullable disable

namespace Vision.Data
{
    public partial class CRMDBContext : DbContext
    {
        public CRMDBContext()
        {

        }

        public CRMDBContext(DbContextOptions<CRMDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AdTemplateConfig>(entity =>
            {
                entity.HasOne(d => d.ClassifiedAdsCategory)
                    .WithMany(p => p.AdTemplateConfigs)
                    .HasForeignKey(d => d.ClassifiedAdsCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdTemplateConfig_ClassifiedAdsCategory");

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.AdTemplateConfigs)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdTemplateConfig_FieldType");
            });

            modelBuilder.Entity<AdTemplateOption>(entity =>
            {
                entity.HasOne(d => d.AdTemplateConfig)
                    .WithMany(p => p.AdTemplateOptions)
                    .HasForeignKey(d => d.AdTemplateConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdTemplateOption_AdTemplateConfig");
            });

            modelBuilder.Entity<ClassifiedAdsCategory>(entity =>
            {
                entity.HasOne(d => d.ClassifiedAdsCategoryParent)
                    .WithMany(p => p.InverseClassifiedAdsCategoryParent)
                    .HasForeignKey(d => d.ClassifiedAdsCategoryParentId)
                    .HasConstraintName("FK_ClassifiedAdsCategory_ClassifiedAdsCategory");
            });

            modelBuilder.Entity<FieldType>(entity =>
            {
                entity.Property(e => e.FieldTypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<AdContent>(entity =>
            {
                entity.ToTable("AdContent");

                entity.HasOne(d => d.ClassifiedAd)
                    .WithMany(p => p.AdContents)
                    .HasForeignKey(d => d.ClassifiedAdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdContent_ClassifiedAd");
            });

            modelBuilder.Entity<AdContentValue>(entity =>
            {
                entity.ToTable("AdContentValue");

                entity.HasOne(d => d.AdContent)
                    .WithMany(p => p.AdContentValues)
                    .HasForeignKey(d => d.AdContentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdContentValue_AdContent");
            });

            modelBuilder.Entity<ClassifiedAd>(entity =>
            {
                entity.ToTable("ClassifiedAd");

                entity.Property(e => e.PublishDate).HasColumnType("datetime");

                //entity.Property(e => e.UseId).HasColumnType("text");
                entity.Property(e => e.UseId).HasMaxLength(50);

                entity.HasOne(d => d.ClassifiedAdsCategory)
                    .WithMany(p => p.ClassifiedAds)
                    .HasForeignKey(d => d.ClassifiedAdsCategoryId)
                    .HasConstraintName("FK_ClassifiedAd_ClassifiedAdsCategory");
            });

            modelBuilder.Entity<ClassifiedAdsCategory>(entity =>
            {
                entity.ToTable("ClassifiedAdsCategory");

                entity.HasIndex(e => e.ClassifiedAdsCategoryParentId, "IX_ClassifiedAdsCategory_ClassifiedAdsCategoryParentId");

                entity.Property(e => e.ClassifiedAdsCategoryDescAr).HasMaxLength(50);

                entity.Property(e => e.ClassifiedAdsCategoryDescEn).HasMaxLength(50);

                entity.Property(e => e.ClassifiedAdsCategoryPic).HasMaxLength(250);

                entity.Property(e => e.ClassifiedAdsCategoryTitleAr).HasMaxLength(50);

                entity.Property(e => e.ClassifiedAdsCategoryTitleEn).HasMaxLength(50);

                entity.HasOne(d => d.ClassifiedAdsCategoryParent)
                    .WithMany(p => p.InverseClassifiedAdsCategoryParent)
                    .HasForeignKey(d => d.ClassifiedAdsCategoryParentId)
                    .HasConstraintName("FK_ClassifiedAdsCategory_ClassifiedAdsCategory");
            });


            OnModelCreatingPartial(modelBuilder);

        }
        public virtual DbSet<AdsImage> AdsImages { get; set; }
        public virtual DbSet<AdTemplateConfig> AdTemplateConfigs { get; set; }
        public virtual DbSet<AdTemplateOption> AdTemplateOptions { get; set; }
        public virtual DbSet<ClassifiedAdsCategory> ClassifiedAdsCategories { get; set; }
        public virtual DbSet<FieldType> FieldTypes { get; set; }
        public virtual DbSet<AdContent> AdContents { get; set; }
        public virtual DbSet<AdContentValue> AdContentValues { get; set; }
        public virtual DbSet<ClassifiedAd> ClassifiedAds { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        //////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        public virtual DbSet<BusinessTemplateConfig> BusinessTemplateConfigs { get; set; }
        public virtual DbSet<BusinessTemplateOption> BusinessTemplateOptions { get; set; }
        public virtual DbSet<ClassifiedBusiness> ClassifiedBusiness { get; set; }
        public virtual DbSet<BusinessContent> BusinessContents { get; set; }
        public virtual DbSet<BusinessContentValue> BusinessContentValues { get; set; }
        public virtual DbSet<BusinessCategory> BusinessCategories { get; set; }
        public virtual DbSet<BusinessWorkingHours> BusinessWorkingHours { get; set; }

        public virtual DbSet<FavouriteBusiness> FavouriteBusiness { get; set; }
        /////////////////////////////Product/////////////////////////////////
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<ProductExtra> ProductExtras { get; set; }
        public virtual DbSet<ProductContent> ProductContents { get; set; }
        public virtual DbSet<ProductContentValue> ProductContentValues { get; set; }
        public virtual DbSet<ProductTemplateConfig> ProductTemplateConfigs { get; set; }
        public virtual DbSet<ProductTemplateOption> ProductTemplateOptions { get; set; }

        //////////////////////////////////////////////////////////////
        /// <summary>
        /// Wallet
        /// </summary>
        public virtual DbSet<PaymentMehod> PaymentMehods { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<WalletSubscription> WalletSubscriptions { get; set; }

        //////////////////////////////////////////
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<AddListing> AddListings { get; set; }
        public virtual DbSet<ListingPhotos> ListingPhotos { get; set; }
        public virtual DbSet<ListingVideos> ListingVideos { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<Folwers> Folwers { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        //public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<PageContent> PageContents { get; set; }
        public virtual DbSet<SoicialMidiaLink> SoicialMidiaLinks { get; set; }
        public virtual DbSet<FAQ> FAQ { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<FavouriteClassified> FavouriteClassifieds { get; set; }
        public virtual DbSet<FavouriteProfile> FavouriteProfiles { get; set; }
        public virtual DbSet<FolowClassified> FolowClassifieds { get; set; }
        public virtual DbSet<FolowProfile> FolowProfile { get; set; }
        public virtual DbSet<BDOffer>BDOffers { get; set; }
        public virtual DbSet<BDOfferImage> BDOfferImages { get; set; }
        public virtual DbSet<FavouriteProduct> FavouriteProducts { get; set; }
        /// <summary>
        /// //Bussiness Order
        /// </summary>
        /// <param name="Bussiness Order"></param>
        /// 
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<BussinessPlan> BussinessPlans  { get; set; }
        public virtual DbSet<BusiniessSubscription> BusiniessSubscriptions  { get; set; }
        public virtual DbSet<OrderItemExtraProduct> OrderItemExtraProducts  { get; set; }
        public virtual DbSet<ShopingCartProductExtraFeatures> ShopingCartProductExtraFeatures { get; set; }
        public virtual DbSet<SearchEntity> SearchEntities { get; set; }
        public virtual DbSet<AppVersion> AppVersions { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceImage> ServiceImages { get; set; }
        public virtual DbSet<ServiceCatagory> ServiceCatagories { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ServiceTemplateConfig> ServiceTemplateConfigs { get; set; }
        public virtual DbSet<ServiceTemplateOption> ServiceTemplateOptions { get; set; }
        public virtual DbSet<ServiceContent> ServiceContents { get; set; }
        public virtual DbSet<ServiceContentValue> ServiceContentValues { get; set; }
        public virtual DbSet<ServiceFavourite> ServiceFavourites { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        public virtual DbSet<BDImage> BDImages { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ServiceQuotation> ServiceQuotations { get; set; }
        public virtual DbSet<ServiceQuotationRequestStatus> ServiceQuotationRequestStatuses { get; set; }

        


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceQuotationRequestStatus>().HasData(new ServiceQuotationRequestStatus { ServiceQuotationRequestStatusId = 1, StatusTitleAr = "انشاء", StatusTitleEn = "Initiated" });
            modelBuilder.Entity<ServiceQuotationRequestStatus>().HasData(new ServiceQuotationRequestStatus { ServiceQuotationRequestStatusId = 2, StatusTitleAr = "تم القبول", StatusTitleEn = "Accepted" });
            modelBuilder.Entity<ServiceQuotationRequestStatus>().HasData(new ServiceQuotationRequestStatus { ServiceQuotationRequestStatusId = 3, StatusTitleAr = "تم الرفض", StatusTitleEn = "Rejected" });
            modelBuilder.Entity<PageContent>().HasData(new PageContent { PageContentId = 1, PageTitleAr = "من نحن", PageTitleEn = "About", ContentAr = "من نحن", ContentEn = "About Page" });
            modelBuilder.Entity<PageContent>().HasData(new PageContent { PageContentId = 2, PageTitleAr = "الشروط والاحكام", PageTitleEn = "Condition and Terms", ContentAr = "الشروط والاحكام", ContentEn = "Condition and Terms Page" });
            modelBuilder.Entity<PageContent>().HasData(new PageContent { PageContentId = 3, PageTitleAr = "سياسة الخصوصية", PageTitleEn = "Privacy Policy", ContentAr = "سياسة الخصوصية", ContentEn = "Privacy Policy Page" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 1, CategoryTitleEn = "Business Support & Supplies" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 2, CategoryTitleEn = "Automotive" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 3, CategoryTitleEn = "Computers & Electronics" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 4, CategoryTitleEn = "Construction & Contractors" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 5, CategoryTitleEn = "Education" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 6, CategoryTitleEn = "Entertainment" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 7, CategoryTitleEn = "Food & Dining" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 8, CategoryTitleEn = "Health & Medicine" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 9, CategoryTitleEn = "Home & Garden" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 10, CategoryTitleEn = "Legal & Financial" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 11, CategoryTitleEn = "Manufacturing, Wholesale, Distribution" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 12, CategoryTitleEn = "Merchants (Retail)" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 13, CategoryTitleEn = "Miscellaneous" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 14, CategoryTitleEn = "Personal Care & Services" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 15, CategoryTitleEn = "Real Estate" });
            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 16, CategoryTitleEn = "Travel & Transportation" });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 1, SubCategoryTitleEn = "Auto Accessories", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 2, SubCategoryTitleEn = "Auto Dealers – New", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 3, SubCategoryTitleEn = "Auto Dealers – Used", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 4, SubCategoryTitleEn = "Detail & Carwash", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 5, SubCategoryTitleEn = "Gas Stations", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 6, SubCategoryTitleEn = "Motorcycle Sales & Repair", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 7, SubCategoryTitleEn = "Rental & Leasing", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 8, SubCategoryTitleEn = "Service, Repair & Parts", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 9, SubCategoryTitleEn = "Towing", CategoryId = 1 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 10, SubCategoryTitleEn = "Consultants", CategoryId = 2 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 11, SubCategoryTitleEn = "Employment Agency", CategoryId = 2 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 12, SubCategoryTitleEn = "Marketing & Communications", CategoryId = 2 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 13, SubCategoryTitleEn = "Office Supplies", CategoryId = 2 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 14, SubCategoryTitleEn = "Printing & Publishing", CategoryId = 2 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 15, SubCategoryTitleEn = "Computer Programming & Support", CategoryId = 3 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 16, SubCategoryTitleEn = "Consumer Electronics & Accessories", CategoryId = 3 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 17, SubCategoryTitleEn = "Architects, Landscape Architects, Engineers & Surveyors", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 18, SubCategoryTitleEn = "Blasting & Demolition", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 19, SubCategoryTitleEn = "Construction Companies", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 20, SubCategoryTitleEn = "Electricians", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 21, SubCategoryTitleEn = "Engineer, Survey", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 22, SubCategoryTitleEn = "Environmental Assessments", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 23, SubCategoryTitleEn = "Inspectors", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 24, SubCategoryTitleEn = "Plaster & Concrete", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 25, SubCategoryTitleEn = "Plumbers", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 26, SubCategoryTitleEn = "Roofers", CategoryId = 4 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 27, SubCategoryTitleEn = "Adult & Continuing Education", CategoryId = 5 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 28, SubCategoryTitleEn = "Early Childhood Education", CategoryId = 5 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 29, SubCategoryTitleEn = "Educational Resources", CategoryId = 5 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 30, SubCategoryTitleEn = "Other Educational", CategoryId = 5 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 32, SubCategoryTitleEn = "Artists, Writers", CategoryId = 6 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 33, SubCategoryTitleEn = "Event Planners & Supplies", CategoryId = 6 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 34, SubCategoryTitleEn = "Golf Courses", CategoryId = 6 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 35, SubCategoryTitleEn = "Movies", CategoryId = 6 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 36, SubCategoryTitleEn = "Productions", CategoryId = 6 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 37, SubCategoryTitleEn = "Desserts, Catering & Supplies", CategoryId = 7 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 38, SubCategoryTitleEn = "Fast Food & Carry Out", CategoryId = 7 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 39, SubCategoryTitleEn = "Grocery, Beverage & Tobacco", CategoryId = 7 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 40, SubCategoryTitleEn = "Restaurants", CategoryId = 7 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 41, SubCategoryTitleEn = "Acupuncture", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 42, SubCategoryTitleEn = "Assisted Living & Home Health Care", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 43, SubCategoryTitleEn = "Audiologist", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 44, SubCategoryTitleEn = "Chiropractic", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 45, SubCategoryTitleEn = "Clinics & Medical Centers", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 46, SubCategoryTitleEn = "Dental ", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 47, SubCategoryTitleEn = "Diet I& Nutrition", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 48, SubCategoryTitleEn = "Laboratory, Imaging & Diagnostic", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 49, SubCategoryTitleEn = "Massage Therapy ", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 50, SubCategoryTitleEn = "Mental Health", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 51, SubCategoryTitleEn = "Nurse", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 52, SubCategoryTitleEn = "Optical", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 53, SubCategoryTitleEn = "Pharmacy, Drug & Vitamin Stores", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 54, SubCategoryTitleEn = "Physical Therapy", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 55, SubCategoryTitleEn = "Physicians & Assistants", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 56, SubCategoryTitleEn = "Podiatry", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 57, SubCategoryTitleEn = "Social Worker", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 58, SubCategoryTitleEn = "Animal Hospita", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 59, SubCategoryTitleEn = "Veterinary & Animal Surgeons ", CategoryId = 8 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 60, SubCategoryTitleEn = "Antiques & Collectibles ", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 61, SubCategoryTitleEn = "Cleaning", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 62, SubCategoryTitleEn = "Crafts, Hobbies & Sports", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 63, SubCategoryTitleEn = "Flower Shops", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 64, SubCategoryTitleEn = "Home Furnishings", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 65, SubCategoryTitleEn = "Home Goods", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 66, SubCategoryTitleEn = "Home Improvements & Repairs", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 67, SubCategoryTitleEn = "Landscape & Lawn Service", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 68, SubCategoryTitleEn = "Pest Control", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 69, SubCategoryTitleEn = "Pool Supplies & Service", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 70, SubCategoryTitleEn = "Security System & Services", CategoryId = 9 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 71, SubCategoryTitleEn = "Accountants ", CategoryId = 10 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 72, SubCategoryTitleEn = "Attorneys", CategoryId = 10 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 73, SubCategoryTitleEn = "Financial Institutions", CategoryId = 10 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 74, SubCategoryTitleEn = "Financial Services", CategoryId = 10 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 75, SubCategoryTitleEn = "Insurance", CategoryId = 10 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 76, SubCategoryTitleEn = "Other Legal", CategoryId = 10 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 77, SubCategoryTitleEn = "Distribution, Import/Export", CategoryId = 11 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 78, SubCategoryTitleEn = "Manufacturing", CategoryId = 11 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 79, SubCategoryTitleEn = "Wholesale", CategoryId = 11 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 80, SubCategoryTitleEn = "Cards & Gifts", CategoryId = 12 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 81, SubCategoryTitleEn = "Clothing & Accessories", CategoryId = 12 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 82, SubCategoryTitleEn = "Department Stores, Sporting Goods", CategoryId = 12 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 83, SubCategoryTitleEn = "General", CategoryId = 12 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 84, SubCategoryTitleEn = "Jewelry", CategoryId = 12 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 85, SubCategoryTitleEn = "Shoes", CategoryId = 12 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 86, SubCategoryTitleEn = "Civic Groups", CategoryId = 13 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 87, SubCategoryTitleEn = "Funeral Service Providers & Cemetaries", CategoryId = 13 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 88, SubCategoryTitleEn = "Miscellaneous", CategoryId = 13 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 89, SubCategoryTitleEn = "Utility Companies", CategoryId = 13 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 90, SubCategoryTitleEn = "Animal Care & Supplies", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 91, SubCategoryTitleEn = "Barber & Beauty Salons", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 92, SubCategoryTitleEn = "Beauty Supplies", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 93, SubCategoryTitleEn = "Dry Cleaners & Laundromats", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 94, SubCategoryTitleEn = "Exercise & Fitness", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 95, SubCategoryTitleEn = "Massage & Body Works", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 96, SubCategoryTitleEn = "Nail Salons", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 97, SubCategoryTitleEn = "Shoe Repairs", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 98, SubCategoryTitleEn = "Agencies & Brokerage", CategoryId = 15 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 99, SubCategoryTitleEn = "Tailors", CategoryId = 14 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 100, SubCategoryTitleEn = "Agents & Brokers", CategoryId = 15 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 101, SubCategoryTitleEn = "Apartment & Home Rental", CategoryId = 15 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 102, SubCategoryTitleEn = "Mortgage Broker & Lender", CategoryId = 15 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 103, SubCategoryTitleEn = "Property Management", CategoryId = 15 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 104, SubCategoryTitleEn = "Title Company", CategoryId = 15 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 105, SubCategoryTitleEn = "Hotel, Motel & Extended Stay", CategoryId = 16 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 106, SubCategoryTitleEn = "Moving & Storage", CategoryId = 16 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 107, SubCategoryTitleEn = "Packaging & Shipping", CategoryId = 16 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 108, SubCategoryTitleEn = "Transportation", CategoryId = 16 });
            modelBuilder.Entity<SubCategory>().HasData(new SubCategory { SubCategoryID = 109, SubCategoryTitleEn = "Travel & Tourism", CategoryId = 16 });
            modelBuilder.Entity<EntityType>().HasData(new EntityType { EntityTypeId = 1, EntityTitleAr = "إعلان", EntityTitleEn = "Classified Ads" });
            modelBuilder.Entity<EntityType>().HasData(new EntityType { EntityTypeId = 2, EntityTitleAr = "عمل", EntityTitleEn = "Bussiness Directory" });
            modelBuilder.Entity<EntityType>().HasData(new EntityType { EntityTypeId = 3, EntityTitleAr = "ملف شخصي", EntityTitleEn = "Profile" });

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

       

    }
}
