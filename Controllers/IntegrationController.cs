using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using Vision.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Vision.ViewModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Microsoft.AspNetCore.Components.RenderTree;
using Email;
using System.Linq;
using Snickler.EFCore;

namespace Vision.Controllers
{
    [Route("api/[controller]")]
    public class VisionAPIController : ControllerBase
    {
        private readonly CRMDBContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly Email.IEmailSender _emailSender;
        public ApplicationDbContext _applicationDbContext { get; set; }
        public HttpClient httpClient { get; set; }
        public VisionAPIController(CRMDBContext dbContext, IWebHostEnvironment hostEnvironment, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, Email.IEmailSender emailSender, ApplicationDbContext applicationDbContext)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            _applicationDbContext = applicationDbContext;
        }
        #region User Auth
        [HttpGet]
        [Route("Login")]
        public async Task<ActionResult<ApplicationUser>> Login(string Email, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    return Ok(new { status = false, message = "Email Not Confirmed" });
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, Password, true);
                if (result.Succeeded)
                {
                    return Ok(new { Status = "Success", User = user });
                }
            }
            var invalidResponse = new { status = false };
            return Ok(invalidResponse);
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel Model)
        {
            var userExists = await _userManager.FindByEmailAsync(Model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });
            var user = new ApplicationUser
            {
                FullName = Model.FullName,
                Email = Model.Email,
                UserName = Model.Email,
                PhoneNumber = Model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, Model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }
            string returnUrl = null;
            returnUrl ??= Url.Content("~/");
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("GetConfirmEmail", "VisionAPI", new { Email = user.Email }, Request.Scheme, "albaheth.me");
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            //            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            var message = new Message(new string[] { $"{user.Email}" }, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", null);
            await _emailSender.SendEmailAsync(message);
            return Ok(new { Status = "Success", Message = "User created successfully!", user });
        }
        [HttpGet]
        [Route("GetConfirmEmail")]
        public async Task<IActionResult> GetConfirmEmail(string Email)
        {
            if (Email == null)
            {
                return BadRequest("Enter User Id..");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string userEmail, string currentPassword, string newPassword)
        {
            if (currentPassword == newPassword)
            {

                return BadRequest("Current Password Is The Same New Password ");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return BadRequest("User Not Found ");
                }
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!result.Succeeded)
                {
                    return BadRequest("Can Not Update User Password ");
                }
                await _signInManager.RefreshSignInAsync(user);
                return Ok(new { status = "success" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(string ProfileId, string userId)
        {
            try
            {
                var profile = await _userManager.FindByIdAsync(ProfileId);
                //var listings = _context.AddListings.Where(a => a.CreatedByUser == userid).ToList();
                //List<Datalist> listingmedia = new List<Datalist>();

                if (profile != null)
                {
                    profile.Photos = _applicationDbContext.Photos.Where(e => e.Id == ProfileId).OrderBy(a => a.PublishDate).ToList();
                    profile.Videos = _applicationDbContext.Videos.Where(e => e.Id == ProfileId).OrderBy(a => a.PublishDate).ToList();
                    //foreach (var item in user.Photos)
                    //{
                    //    listingmedia.Add(new Datalist(item.PhotoID, item.Image));
                    //}
                    //foreach (var item in user.Videos)
                    //{
                    //    listingmedia.Add(new Datalist(item.VideoID, item.VideoT));
                    //}

                    profile.Languages = _applicationDbContext.Languages.Where(e => e.Id == ProfileId).ToList();
                    profile.Interests = _applicationDbContext.Interests.Where(e => e.Id == ProfileId).ToList();
                    profile.Skills = _applicationDbContext.Skills.Where(e => e.Id == ProfileId).ToList();
                    profile.LifeEvents = _applicationDbContext.LifeEvents.Where(e => e.Id == ProfileId).ToList();
                    profile.Educations = _applicationDbContext.Educations.Where(e => e.Id == ProfileId).ToList();
                    bool IsFavourite = _dbContext.FavouriteProfiles.Any(o => o.Id == ProfileId && o.UserId == userId);
                    bool IsFolowing = _dbContext.FolowProfile.Any(o => o.Id == ProfileId && o.UserId == userId);

                    return Ok(new { profile, IsFollow = IsFolowing, IsFavourite = IsFavourite });

                }
                return BadRequest("User Not Found");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        #endregion
        #region Bussiness 
        [HttpGet]
        [Route("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _dbContext.Categories.ToListAsync();
                var model = new
                {
                    status = true,
                    Categories = categories
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetCategoryByID")]
        public IActionResult GetCategoryByID(int CategoryId)
        {
            if (CategoryId != 0)
            {
                try
                {
                    var categories = _dbContext.Categories.Find(CategoryId);
                    if (categories != null)
                    {
                        return Ok(categories);
                    }
                    return BadRequest("Category NotFound..");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("Enter Valid ID..");

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool validateCategory(Category category)
        {
            if (category.CategoryTitleAr != null && category.CategoryTitleEn != null)
            {
                return true;
            }
            return false;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool validateSubCategory(SubCategory Subcategory)
        {
            if (Subcategory.SubCategoryTitleAr != null && Subcategory.SubCategoryTitleEn != null && Subcategory.CategoryId != 0)
            {
                return true;
            }
            return false;
        }
        [HttpGet]
        [Route("GetSubCategories")]
        public async Task<IActionResult> GetSubCategories(int CategoryId)
        {
            try
            {
                var subCategories = await _dbContext.SubCategories.Where(a => a.CategoryId == CategoryId).ToListAsync();
                var model = new
                {
                    status = true,
                    SubCategories = subCategories
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetSubCategoryByID")]
        public IActionResult GetSubCategoryByID(int SubCategoryId)
        {
            if (SubCategoryId != 0)
            {
                try
                {
                    var categories = _dbContext.SubCategories.Where(a => a.SubCategoryID == SubCategoryId).Include(a => a.Category).Select(a => new { a.SubCategoryTitleAr, a.SubCategoryTitleEn, a.SortOrder, a.Tags, a.SubCategoryPic, a.Description, a.Category.CategoryTitleAr, a.Category.CategoryTitleEn, a.SubCategoryID });
                    if (categories != null)
                    {
                        return Ok(categories);
                    }
                    return BadRequest("Sub Category NotFound..");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("Enter Valid ID..");

        }


        [HttpGet]
        [Route("GetAllMedia")]
        public async Task<IActionResult> GetAllMedia(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                var media = new List<MediaVM>();

                var photosList = _applicationDbContext.Photos.Where(e => e.Id == user.Id);
                var VideosList = _applicationDbContext.Videos.Where(e => e.Id == user.Id);
                if (photosList != null)
                {
                    foreach (var item in photosList)
                    {
                        var mediaobj = new MediaVM() { MediaId = item.PhotoID, Caption = item.Caption, MediaURL = item.Image, PublishDate = item.PublishDate };
                        media.Add(mediaobj);
                    }
                }
                if (VideosList != null)
                {
                    foreach (var item in VideosList)
                    {
                        var mediaobj = new MediaVM() { MediaId = item.VideoID, Caption = item.Caption, MediaURL = item.VideoT, PublishDate = item.PublishDate };
                        media.Add(mediaobj);
                    }
                }
                if (media != null)
                {
                    media = media.OrderBy(a => a.PublishDate).ToList();
                }

                return Ok(new { Status = "Success", Media = media });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpPost]
        [Route("AddBussinesPhotos")]
        public IActionResult AddBussinesPhotos(int listingId, IFormFile Photo, string caption)
        {
            try
            {

                var listing = _dbContext.AddListings.Find(listingId);
                if (listing == null)
                {
                    return Ok(new { status = "false", message = "Addlisting Not Found.." });
                }
                if (Photo == null)
                {
                    return Ok(new { status = "false", message = "Please Upload Photo.." });
                }
                var photoobj = new ListingPhotos() { Caption = caption, PublishDate = DateTime.Now, AddListingId = listingId };
                string folder = "Images/ListingMedia/Photos/";
                photoobj.PhotoUrl = UploadImage(folder, Photo);
                _dbContext.ListingPhotos.Add(photoobj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Listing photo Added successfully..", photo = photoobj });

            }
            catch (Exception e)
            {

                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPost]
        [Route("AddBussinesVideo")]
        public IActionResult AddBussinesVideo(int listingId, IFormFile Video, string caption)
        {
            try
            {

                var listing = _dbContext.AddListings.Find(listingId);
                if (listing == null)
                {
                    return Ok(new { status = "false", message = "Addlisting Not Found.." });
                }
                if (Video == null)
                {
                    return Ok(new { status = "false", message = "Please Upload Video.." });
                }
                var videoobj = new ListingVideos() { Caption = caption, PublishDate = DateTime.Now, AddListingId = listingId };
                string folder = "Images/ListingMedia/Videos/";
                videoobj.VideoUrl = UploadImage(folder, Video);
                _dbContext.ListingVideos.Add(videoobj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Listing Video Added successfully..", Video = videoobj });

            }
            catch (Exception e)
            {

                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPost]
        [Route("AddListing")]
        public async Task<IActionResult> AddListing(IFormFile Listinglogo, IFormFile PromoVideo, IFormFile listingbanner, IFormFileCollection Videos, IFormFileCollection Photos, AddListing addListing)
        {
            var user = await _userManager.FindByEmailAsync(addListing.CreatedByUser);

            if (user == null)
            {
                return Ok(new { status = "false", message = "User Not Found.." });
            }


            List<ListingPhotos> photolistings = new List<ListingPhotos>();
            if (Photos.Count() > 0)
            {
                for (int i = 0; i < Photos.Count(); i++)
                {
                    ListingPhotos photoobj = new ListingPhotos();
                    if (Photos[i] != null)
                    {
                        string folder = "Images/ListingMedia/Photos/";
                        photoobj.PhotoUrl = UploadImage(folder, Photos[i]);
                    }

                    photolistings.Add(photoobj);
                }
                addListing.ListingPhotos = photolistings;
            }


            List<ListingVideos> videoListing = new List<ListingVideos>();
            if (Videos.Count() > 0)
            {
                for (int i = 0; i < Videos.Count(); i++)
                {
                    ListingVideos videoobj = new ListingVideos();
                    if (Videos[i] != null)
                    {
                        string folder = "Images/ListingMedia/Videos/";
                        videoobj.VideoUrl = UploadImage(folder, Videos[i]);
                    }

                    videoListing.Add(videoobj);
                }
                addListing.ListingVideos = videoListing;
            }

            if (Listinglogo != null)
            {
                string folder = "Images/ListingMedia/Logos/";
                addListing.ListingLogo = UploadImage(folder, Listinglogo);
            }
            if (PromoVideo != null)
            {
                string folder = "Images/ListingMedia/Videos/";
                addListing.PromoVideo = UploadImage(folder, PromoVideo);
            }
            if (listingbanner != null)
            {
                string folder = "Images/ListingMedia/Banners/";
                addListing.ListingBanner = UploadImage(folder, listingbanner);
            }
            addListing.AddedDate = DateTime.Now;
            _dbContext.AddListings.Add(addListing);
            _dbContext.SaveChanges();
            return Ok(new { status = "success", addListing });

        }
        [HttpPut]
        [Route("EditListing")]
        public async Task<IActionResult> EditListing(IFormFile Listinglogo, IFormFile PromoVideo, IFormFile listingbanner, IFormFileCollection Videos, IFormFileCollection Photos, AddListing addListing)
        {
            var model = _dbContext.AddListings.Where(c => c.AddListingId == addListing.AddListingId).FirstOrDefault();
            if (Listinglogo != null)
            {
                string folder = "Images/ListingMedia/Logos/";
                addListing.ListingLogo = UploadImage(folder, Listinglogo);
                model.ListingLogo = addListing.ListingLogo;
            }
            if (PromoVideo != null)
            {
                string folder = "Images/ListingMedia/Videos/";
                addListing.PromoVideo = UploadImage(folder, PromoVideo);
                model.PromoVideo = addListing.PromoVideo;
            }
            if (listingbanner != null)
            {
                string folder = "Images/ListingMedia/Banners/";
                addListing.ListingBanner = UploadImage(folder, listingbanner);
                model.ListingBanner = addListing.ListingBanner;
            }
            List<ListingPhotos> photolistings = new List<ListingPhotos>();
            if (Photos.Count() > 0)
            {
                for (int i = 0; i < Photos.Count(); i++)
                {
                    ListingPhotos photoobj = new ListingPhotos();
                    if (Photos[i] != null)
                    {
                        string folder = "Images/ListingMedia/Photos/";
                        photoobj.PhotoUrl = UploadImage(folder, Photos[i]);
                    }

                    photolistings.Add(photoobj);
                }
                addListing.ListingPhotos = photolistings;
                model.ListingPhotos = photolistings;
            }


            List<ListingVideos> videoListing = new List<ListingVideos>();
            if (Videos.Count() > 0)
            {
                for (int i = 0; i < Videos.Count(); i++)
                {
                    ListingVideos videoobj = new ListingVideos();
                    if (Videos[i] != null)
                    {
                        string folder = "Images/ListingMedia/Videos/";
                        videoobj.VideoUrl = UploadImage(folder, Videos[i]);
                    }

                    videoListing.Add(videoobj);
                }
                addListing.ListingVideos = videoListing;
                model.ListingVideos = videoListing;

            }
            try
            {
                if (model == null)
                {
                    return Ok(new { status = "false" });

                }
                var branchesid = _dbContext.Branches.Where(a => a.AddListingId == addListing.AddListingId);
                _dbContext.Branches.RemoveRange(branchesid);
                model.Address = addListing.Address;
                model.Title = addListing.Title;
                model.Phone1 = addListing.Phone1;
                model.Phone2 = addListing.Phone2;
                model.Tags = addListing.Tags;
                model.Reviews = addListing.Reviews;
                model.Rating = addListing.Rating;
                model.MainLocataion = addListing.MainLocataion;
                model.CityId = addListing.CityId;
                model.ContactPeroson = addListing.ContactPeroson;
                model.CountryId = addListing.CountryId;
                model.Branches = addListing.Branches;
                model.CategoryId = addListing.CategoryId;
                model.Discription = addListing.Discription;
                model.Email = addListing.Email;
                model.Fax = addListing.Fax;
                model.Website = addListing.Website;

                _dbContext.Attach(model).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { status = "success", model });

            }
            catch (Exception)
            {
                return Ok(new { status = "failed" });

            }
        }

        [HttpDelete]
        [Route("DeleteListing")]
        public IActionResult DeleteListing(int ListingId)
        {
            if (ListingId != 0)
            {
                try
                {
                    var Listing = _dbContext.AddListings.Find(ListingId);
                    if (Listing != null)
                    {
                        _dbContext.AddListings.Remove(Listing);
                        _dbContext.SaveChanges();
                        return Ok("Listing Deleted Successfully..");
                    }
                    return BadRequest("Listing Not Found..");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("Enter Valid ID..");

        }
        [HttpGet]
        [Route("GetListingbyId")]
        public IActionResult GetListingbyId(int ListingId, string userid)
        {
            if (ListingId != 0)
            {
                try
                {
                    var Listing = _dbContext.AddListings.Include(a => a.Branches).Include(a => a.ListingPhotos).Include(a => a.ListingVideos).Include(a => a.Country).Where(a => a.AddListingId == ListingId).Select(c => new
                    {
                        AddedDate = c.AddedDate,
                        AddListingId = c.AddListingId,
                        Address = c.Address,
                        Branches = c.Branches,
                        CategoryId = c.CategoryId,
                        CountryId = c.CountryId,
                        CityId = c.CityId,
                        Country = c.Country,
                        CreatedByUser = c.CreatedByUser,
                        Discription = c.Discription,
                        ListingLogo = c.ListingLogo,
                        ListingPhotos = c.ListingPhotos,
                        ListingVideos = c.ListingVideos,
                        ListingBanner = c.ListingBanner,
                        Fax = c.Fax,
                        Email = c.Email,
                        Website = c.Website,
                        Phone1 = c.Phone1,
                        Phone2 = c.Phone2,
                        Rating = c.Rating,
                        Reviews = c.Reviews,
                        ContactPeroson = c.ContactPeroson,
                        MainLocataion = c.MainLocataion,
                        PromoVideo = c.PromoVideo,
                        Tags = c.Tags,
                        Title = c.Title,
                        IsFavourite = _dbContext.Favourites.Any(o => o.AddListingId == c.AddListingId && o.UserId == userid),
                        IsFolowing = _dbContext.Folwers.Any(o => o.AddListingId == c.AddListingId && o.UserId == userid),

                    }).FirstOrDefault();

                    if (Listing != null)
                    {
                        return Ok(new { Status = true, Listing = Listing });
                    }
                    return Ok(new { status = false, message = "Listing  NotFound.." });

                }
                catch (Exception e)
                {
                    return Ok(new { status = false, message = e.Message });
                }
            }
            return BadRequest("Enter Valid ID..");
        }
        [HttpGet]
        [Route("GetListingbyUserId")]
        public IActionResult GetListingbyUserId(string Email, string userId)
        {
            try
            {
                var Listing = _dbContext.AddListings.Include(a => a.Branches).Include(a => a.ListingPhotos).Include(a => a.ListingVideos).Include(a => a.Country).Where(a => a.CreatedByUser == Email).Select(c => new
                {
                    AddedDate = c.AddedDate,
                    AddListingId = c.AddListingId,
                    Address = c.Address,
                    Branches = c.Branches,
                    CategoryId = c.CategoryId,
                    CountryId = c.CountryId,
                    CityId = c.CityId,
                    Country = c.Country,
                    CreatedByUser = c.CreatedByUser,
                    Discription = c.Discription,
                    ListingLogo = c.ListingLogo,
                    ListingPhotos = c.ListingPhotos,
                    ListingVideos = c.ListingVideos,
                    ListingBanner = c.ListingBanner,
                    Fax = c.Fax,
                    Email = c.Email,
                    Website = c.Website,
                    Phone1 = c.Phone1,
                    Phone2 = c.Phone2,
                    Rating = c.Rating,
                    Reviews = c.Reviews,
                    ContactPeroson = c.ContactPeroson,
                    MainLocataion = c.MainLocataion,
                    PromoVideo = c.PromoVideo,
                    Tags = c.Tags,
                    Title = c.Title,
                    IsFavourite = _dbContext.Favourites.Any(o => o.AddListingId == c.AddListingId && o.UserId == userId),
                    IsFolowing = _dbContext.Folwers.Any(o => o.AddListingId == c.AddListingId && o.UserId == userId),

                }).FirstOrDefault();

                if (Listing != null)
                {
                    return Ok(new { Status = true, Listing = Listing });
                }
                return Ok(new { status = false, message = "Listing  NotFound.." });

            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }

        }

        [HttpGet]
        [Route("GetAllReviews")]
        public IActionResult GetAllReviews(int listingid)
        {
            if (listingid != 0)
            {
                try
                {
                    //var reviews = _dbContext.Reviews.Where(a => a.AddListingId == listingid).ToList();
                    var reviews = _dbContext.Reviews.ToList();
                    if (reviews == null)
                    {
                        return Ok(new { status = "false" });
                    }
                    return Ok(new { status = "success", reviews });
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("Enter Valid ListingId..");

        }

        [HttpGet]
        [Route("GetAllCountries")]
        public async Task<IActionResult> GetAllCountries()
        {
            try
            {
                var countries = await _dbContext.Countries.ToListAsync();
                var model = new
                {
                    status = true,
                    Countries = countries
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidateBranchModel(BranchVM branch)
        {
            if (branch.Lat == null || branch.Long == null || branch.Title == null)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("AddBranch")]
        public IActionResult AddBranch(BranchVM branch)
        {
            var lsting = _dbContext.AddListings.Find(branch.AddListingId);
            if (lsting == null)
            {
                return Ok(new { status = false, message = "AddListing Not Found..." });
            }
            if (!ValidateBranchModel(branch))
            {
                return Ok(new { status = false, message = "Enter Required Data..." });
            }
            try
            {
                var model = new Branch() { AddListingId = branch.AddListingId, Lat = branch.Lat, Long = branch.Long, Title = branch.Title };
                _dbContext.Branches.Add(model);
                _dbContext.SaveChanges();
                return Ok(new { status = true, Branch = model });

            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPut]
        [Route("EditBranch")]
        public IActionResult EditBranch(BranchVM branch)
        {

            if (branch.BranchId == 0)
            {
                return Ok(new { status = false, message = "Enter BranchId..." });
            }
            var model = _dbContext.Branches.Find(branch.BranchId);
            if (model == null)
            {
                return Ok(new { status = false, message = "Branch Not Found..." });
            }
            try
            {
                model.Title = branch.Title;
                model.Lat = branch.Lat;
                model.Long = branch.Long;
                _dbContext.Attach(model).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(new { status = true, Branch = model });
            }
            catch (Exception e)
            {

                return Ok(new { status = false, message = e.Message });

            }
        }
        [HttpDelete]
        [Route("DeleteBranch")]
        public IActionResult DeleteBranch(int BranchId)
        {
            var model = _dbContext.Branches.Find(BranchId);
            if (model == null)
            {
                return Ok(new { status = false, message = "Branch Not Found.." });
            }
            try
            {
                _dbContext.Branches.Remove(model);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Branch Deleted Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetBranchById")]
        public IActionResult GetBranchById(int branchid)
        {

            try
            {
                var Branch = _dbContext.Branches.Find(branchid);
                var model = new
                {
                    status = true,
                    Branch = Branch
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetAllBranches")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var Branches = await _dbContext.Branches.ToListAsync();
                var model = new
                {
                    status = true,
                    Branches = Branches
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }

        [HttpGet]
        [Route("GetBuisnessMedia")]
        public IActionResult GetBuisnessMedia(int buisnessid)
        {
            try
            {
                var listing = _dbContext.AddListings.Find(buisnessid);
                if (listing == null)
                {
                    return Ok(new { Status = "false", Reason = "Buisness Not Found" });
                }
                var media = new List<MediaVM>();

                var photosList = _dbContext.ListingPhotos.Where(e => e.AddListingId == buisnessid);
                var VideosList = _dbContext.ListingVideos.Where(e => e.AddListingId == buisnessid);
                if (photosList != null)
                {
                    foreach (var item in photosList)
                    {
                        var mediaobj = new MediaVM() { Caption = item.Caption, MediaURL = item.PhotoUrl, PublishDate = item.PublishDate };
                        media.Add(mediaobj);
                    }
                }
                if (VideosList != null)
                {
                    foreach (var item in VideosList)
                    {
                        var mediaobj = new MediaVM() { Caption = item.Caption, MediaURL = item.VideoUrl, PublishDate = item.PublishDate };
                        media.Add(mediaobj);
                    }
                }
                if (media != null)
                {
                    media = media.OrderBy(a => a.PublishDate).ToList();
                }

                return Ok(new { Status = "Success", Media = media });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpGet]
        [Route("GetAllBuisness")]
        public IActionResult GetAllBuisness(string userId)
        {

            try
            {
                var Buisness = _dbContext.AddListings.Include(a => a.Branches).Include(a => a.ListingPhotos).Include(a => a.ListingVideos).Include(a => a.Country).Select(c => new
                {
                    AddedDate = c.AddedDate,
                    AddListingId = c.AddListingId,
                    Address = c.Address,
                    Branches = c.Branches,
                    CategoryId = c.CategoryId,
                    CountryId = c.CountryId,
                    CityId = c.CityId,
                    Country = c.Country,
                    CreatedByUser = c.CreatedByUser,
                    Discription = c.Discription,
                    ListingLogo = c.ListingLogo,
                    ListingPhotos = c.ListingPhotos,
                    ListingVideos = c.ListingVideos,
                    ListingBanner = c.ListingBanner,
                    Fax = c.Fax,
                    Email = c.Email,
                    Website = c.Website,
                    Phone1 = c.Phone1,
                    Phone2 = c.Phone2,
                    Rating = c.Rating,
                    Reviews = c.Reviews,
                    ContactPeroson = c.ContactPeroson,
                    MainLocataion = c.MainLocataion,
                    PromoVideo = c.PromoVideo,
                    Tags = c.Tags,
                    Title = c.Title,
                    IsFavourite = _dbContext.Favourites.Any(o => o.AddListingId == c.AddListingId && o.UserId == userId),
                    IsFolowing = _dbContext.Folwers.Any(o => o.AddListingId == c.AddListingId && o.UserId == userId),

                }).ToList();

                if (Buisness != null)
                {
                    return Ok(new { Status = true, Buisness = Buisness });
                }
                return Ok(new { status = false, message = "Buisness  NotFound.." });

            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        //[HttpPost]
        //[Route("AddFavouriteBuisness")]
        //public IActionResult AddFavouriteBuisness(int buisnessid, string userid)
        //{
        //    try
        //    {

        //        var buisness = _dbContext.AddListings.Find(buisnessid);
        //        var user = _applicationDbContext.Users.Find(userid);
        //        if (buisness == null)
        //        {
        //            return Ok(new { status = false, message = "Buisness Not Found." });

        //        }
        //        if (user == null)
        //        {
        //            return Ok(new { status = false, message = "User Not Found." });

        //        }
        //        var favourite = _dbContext.Favourites.Where(a => a.AddListingId == buisnessid && a.UserId == userid).FirstOrDefault();
        //        if (favourite != null)
        //        {
        //            return Ok(new { status = false, message = "Buisness already added in favourite.." });
        //        }
        //        var favouriteobj = new Favourite() { UserId = userid, AddListingId = buisnessid };
        //        _dbContext.Favourites.Add(favouriteobj);
        //        _dbContext.SaveChanges();
        //        return Ok(new { status = true, message = "Buisness Added To Favourite.." });
        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(new { status = false, message = e.Message });
        //    }
        //}
        [HttpDelete]
        [Route("DeleteFavouriteBuisness")]
        public IActionResult DeleteFavouriteBuisness(long buisnessId, string userid)
        {
            try
            {
                var favourite = _dbContext.FavouriteBusiness.Where(e => e.UserId == userid && e.ClassifiedBusinessId == buisnessId).FirstOrDefault();
                if (favourite == null)
                {
                    return Ok(new { Status = false, message = "Favourite Buisness Not Found.." });
                }
                _dbContext.FavouriteBusiness.Remove(favourite);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, message = "Buisness Deleted Successfully From Favourite.." });

            }
            catch (Exception e)
            {

                return Ok(new { Status = false, message = e.Message });

            }
        }
        [HttpGet]
        [Route("GetFavouriteBuisnessByUser")]
        public async Task<IActionResult> GetFavouriteBuisnessByUser(string userid)
        {
            try
            {
                var user = _applicationDbContext.Users.Find(userid);
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });
                }
                var Favourite = await _dbContext.Favourites.Where(a => a.UserId == userid).Include(a => a.AddListing).ToListAsync();
                var model = new
                {
                    status = true,
                    FavouriteBusness = Favourite
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetFavouriteBuisnessCountByUser")]
        public IActionResult GetFavouriteBuisnessCountByUser(string userid)
        {
            try
            {
                var user = _applicationDbContext.Users.Find(userid);

                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });
                }
                var Favourite = _dbContext.Favourites.Where(a => a.UserId == userid).Count();
                var model = new
                {
                    status = true,
                    FavouriteCount = Favourite
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }


        [HttpGet]
        [Route("GetCountAddListingByUser")]
        public IActionResult GetCountAddListingByUser(string UserEmail)
        {
            try
            {

                int addListingCount = _dbContext.AddListings.Where(e => e.CreatedByUser == UserEmail).Count();
                var model = new
                {
                    status = true,
                    Count = addListingCount
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }
        }
        [HttpGet]
        [Route("GetAddListingByCountry")]
        public IActionResult GetAddListingByCountry(int CountryId)
        {
            try
            {
                var rnd = new Random();
                var addListingList = _dbContext.AddListings.Where(e => e.CountryId == CountryId).ToList();
                var RandomaddListingList = addListingList.OrderBy(x => rnd.Next());
                var model = new
                {
                    status = true,
                    AddListingList = RandomaddListingList
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }
        }


        [HttpGet]
        [Route("GetSearchResult")]
        public async Task<IActionResult> GetSearchResult(string Search)
        {
            if (Search == null)
            {
                return Ok(new { status = false, message = "Enter Any Text To search." });
            }
            try
            {
                var users = _applicationDbContext.Users.Where(a => a.UserName.ToUpper().Contains(Search.ToUpper())).ToList();
                var buisness = _dbContext.AddListings.Include(a => a.Country).Include(a => a.Category).Include(a => a.Reviews).Include(a => a.Branches).Include(a => a.ListingPhotos).Include(a => a.ListingVideos).Where(a => a.Country.CountryTlEn.ToUpper().Contains(Search.ToUpper()) || a.Category.CategoryTitleEn.ToUpper().Contains(Search.ToUpper()) || a.Category.CategoryTitleAr.ToUpper().Contains(Search.ToUpper())).ToList();
                //var classifiedads = _context.ClassifiedAds.Include(a => a.ClassifiedAdsType).Where(a => a.ClassifiedAdsType.TypeTitleEn.ToUpper().Contains(Search.ToUpper()) || a.ClassifiedAdsType.TypeTitleAr.ToUpper().Contains(Search.ToUpper()) || a.Title.ToUpper().Contains(Search.ToUpper())).ToList();
                //var userssw = await _userManager.FindByEmailAsync(Search);
                var Searchresult = new List<SearchVM>();
                foreach (var item in users)
                {
                    var userssw = await _userManager.FindByEmailAsync(item.UserName);
                    var searchobj = new SearchVM() { UserId = item.Email, id = item.Id, Title = userssw.FullName, CategoryEn = userssw.Job, CategoryAr = userssw.Job, image = userssw.ProfilePicture, Type = 1 };
                    Searchresult.Add(searchobj);
                }
                foreach (var item in buisness)
                {
                    var searchobj = new SearchVM() { UserId = item.CreatedByUser, id = item.AddListingId.ToString(), CategoryEn = item.Category.CategoryTitleEn, CategoryAr = item.Category.CategoryTitleAr, Title = item.Title, image = item.ListingLogo, Type = 2 };
                    Searchresult.Add(searchobj);
                }
                //foreach (var item in classifiedads)
                //{
                //    var searchobj = new SearchVM() { Email = item.AddedBy, id = item.ClassifiedAdsID.ToString(), CategoryEn = item.ClassifiedAdsType.TypeTitleEn, CategoryAr = item.ClassifiedAdsType.TypeTitleAr, Title = item.Title, image = item.MainPhoto, Type = 3 };
                //    Searchresult.Add(searchobj);
                //}
                if (Searchresult.Count() == 0)
                {
                    return Ok(new { status = false, message = "Not Matched Result.. " });
                }
                var rnd = new Random();
                var RandomList = Searchresult.OrderBy(x => rnd.Next());

                var model = new
                {
                    status = true,
                    searchresult = RandomList
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPost]
        [Route("AddQuotation")]
        public IActionResult AddQuotation(QuotationVM quotationVM)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == quotationVM.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var classifiedAdsObj = _dbContext.ClassifiedAds.Where(e => e.ClassifiedAdId == quotationVM.ClassifiedAdsID).FirstOrDefault();
                if (classifiedAdsObj == null)
                {
                    return Ok(new { status = false, message = "Classified Not Found" });
                }
                var quotation = new Quotation()
                {
                    ClassifiedAdId = quotationVM.ClassifiedAdsID,
                    UserId = quotationVM.UserId,
                    QuotationDate = DateTime.Now,
                    Description = quotationVM.Description,
                };

                _dbContext.Quotations.Add(quotation);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Quotation Added Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPost]
        [Route("FlowerToBussiness")]
        public IActionResult FlowerToBussiness(FolowVm folwers)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var addListingObj = _dbContext.AddListings.Where(e => e.AddListingId == folwers.AddListingId).FirstOrDefault();
                if (addListingObj == null)
                {
                    return Ok(new { status = false, message = "Bussiness Not Found" });
                }
                var folwerObj = new Folwers()
                {
                    UserId = folwers.UserId,
                    AddListingId = folwers.AddListingId

                };

                _dbContext.Folwers.Add(folwerObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Folwer Added Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpDelete]
        [Route("UnFlowerToBussiness")]
        public IActionResult UnFlowerToBussiness(FolowVm folwers)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var addListingObj = _dbContext.AddListings.Where(e => e.AddListingId == folwers.AddListingId).FirstOrDefault();
                if (addListingObj == null)
                {
                    return Ok(new { status = false, message = "Bussiness Not Found" });
                }
                var folwerObj = _dbContext.Folwers.Where(e => e.AddListingId == folwers.AddListingId && e.UserId == folwers.UserId).FirstOrDefault();

                _dbContext.Folwers.Remove(folwerObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Folwer Deleted Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetBussinessFolwerCount")]
        public IActionResult GetBussinessFolwerCount(int bussinessId)
        {

            try
            {

                int FolwersCount = _dbContext.Folwers.Where(e => e.AddListingId == bussinessId).Count();
                var model = new
                {
                    Status = true,
                    Count = FolwersCount
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllQuotation")]
        public IActionResult GetAllQuotation()
        {
            try
            {

                var Quotations = _dbContext.Quotations.ToList();
                var model = new
                {
                    status = true,
                    QuotationsList = Quotations
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }
        }
        [HttpGet]
        [Route("GetQuotationByUserId")]
        public IActionResult GetQuotationByUserId(string userId)
        {

            try
            {

                var Quotation = _dbContext.Quotations.Where(e => e.UserId == userId).ToList();
                var model = new
                {
                    Status = true,
                    QuotationList = Quotation
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllAreasByCityId")]
        public IActionResult GetAllAreasByCityId(int cityId)
        {

            try
            {

                var Areas = _dbContext.Areas.Where(e => e.CityId == cityId).ToList();
                var model = new
                {
                    Status = true,
                    Areas = Areas
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }

        }
        [HttpPost]
        [Route("AddMessage")]
        public IActionResult AddMessage(Contact contact)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Email == contact.Email).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                if (contact.Message == null)
                {
                    return Ok(new { status = false, message = "Message Is Required" });

                }
                if (contact.FullName == null)
                {
                    return Ok(new { status = false, message = "FullName Is Required" });

                }
                contact.SendingDate = DateTime.Now;
                _dbContext.Contacts.Add(contact);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Message Send Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }

        [HttpGet]
        [Route("GetPagesContentById")]
        public IActionResult GetPagesContentById([FromQuery] int PageContentId)
        {

            try
            {

                var pageContent = _dbContext.PageContents.FirstOrDefault(c => c.PageContentId == PageContentId);
                if (pageContent == null)
                {
                    return Ok(new { status = false, message = "Object Not Found" });

                }
                return Ok(new { status = true, pageContent = pageContent });

            }
            catch (Exception)
            {

                return Ok(new { status = false, message = "Something went wrong" });

            }
        }
        [HttpGet]
        [Route("getAllSocialLinks")]
        public IActionResult getAllSocialLinks()
        {
            try
            {
                string adminRoleId = _applicationDbContext.Roles.Where(e => e.Name == "Admin").FirstOrDefault().Id;
                var UserAdminId = _applicationDbContext.UserRoles.Where(e => e.RoleId == adminRoleId).FirstOrDefault().UserId;
                var user = _userManager.Users.Where(e => e.Id == UserAdminId).FirstOrDefault();
                var SocialLinks = _dbContext.SoicialMidiaLinks.ToList().Take(1).FirstOrDefault();

                if (SocialLinks == null)
                {
                    return Ok(new { Success = false, message = "Object Not Exist" });
                }
                var SocialObj = new SocialLinksVm()
                {
                    Instgramlink = SocialLinks.Instgramlink,
                    WhatsApplink = SocialLinks.WhatsApplink,
                    YoutubeLink = SocialLinks.WhatsApplink,
                    TwitterLink = SocialLinks.TwitterLink,
                    LinkedInlink = SocialLinks.LinkedInlink,
                    facebooklink = SocialLinks.facebooklink,
                    id = SocialLinks.id,
                    AdminEmail = user.Email,
                    AdminPhone = user.PhoneNumber

                };

                return Ok(new { Success = true, SocialLinks = SocialObj });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }


        }
        [HttpDelete]
        [Route("DeleteBussinessPhoto")]
        public IActionResult DeleteBussinessPhoto(int ListingPhotoId)
        {

            try
            {

                var ListinPhotogObj = _dbContext.ListingPhotos.Where(e => e.Id == ListingPhotoId).FirstOrDefault();
                if (ListinPhotogObj == null)
                {
                    return Ok(new { status = false, message = "Bussiness Photo Object Not Found" });
                }

                _dbContext.ListingPhotos.Remove(ListinPhotogObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Photo Deleted Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteBussinessVedio")]
        public IActionResult DeleteBussinessVedio(int ListingVedioId)
        {

            try
            {

                var ListingVideoObj = _dbContext.ListingVideos.Where(e => e.Id == ListingVedioId).FirstOrDefault();
                if (ListingVideoObj == null)
                {
                    return Ok(new { status = false, message = "Bussiness Video Object Not Found" });
                }

                _dbContext.ListingVideos.Remove(ListingVideoObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Video Deleted Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllMediaForBussiness")]
        public IActionResult GetAllMediaForBussiness(int bussinessId)
        {
            try
            {
                var addListingsObj = _dbContext.AddListings.Where(e => e.AddListingId == bussinessId).FirstOrDefault();
                if (addListingsObj == null)
                {
                    return Ok(new { Status = "false", Message = "Bussiness Not Found" });
                }
                var media = new List<MediaVM>();

                var photosList = _dbContext.ListingPhotos.Where(e => e.AddListingId == bussinessId);
                var VideosList = _dbContext.ListingVideos.Where(e => e.AddListingId == bussinessId);
                if (photosList != null)
                {
                    foreach (var item in photosList)
                    {
                        var mediaobj = new MediaVM() { MediaId = item.Id, Caption = item.Caption, MediaURL = item.PhotoUrl, PublishDate = item.PublishDate };
                        media.Add(mediaobj);
                    }
                }
                if (VideosList != null)
                {
                    foreach (var item in VideosList)
                    {
                        var mediaobj = new MediaVM() { MediaId = item.Id, Caption = item.Caption, MediaURL = item.VideoUrl, PublishDate = item.PublishDate };
                        media.Add(mediaobj);
                    }
                }
                if (media != null)
                {
                    media = media.OrderBy(a => a.PublishDate).ToList();
                }

                return Ok(new { Status = "Success", Media = media });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetFAQList")]
        public IActionResult GetFAQList()
        {
            try
            {
                var faqsList = _dbContext.FAQ.ToList();
                return Ok(new { Status = true, FaqsList = faqsList });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpPost]
        [Route("AddFavouriteToProfile")]
        public IActionResult AddFavouriteToProfile(string userProfileId, string userid)
        {
            try
            {

                var userProfile = _applicationDbContext.Users.Where(e => e.Id == userProfileId).FirstOrDefault();
                var user = _applicationDbContext.Users.Find(userid);
                if (userProfile == null)
                {
                    return Ok(new { status = false, message = "User Profile Not Found." });

                }
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });

                }
                var favourite = _dbContext.FavouriteProfiles.Where(a => a.Id == userProfileId && a.UserId == userid).FirstOrDefault();
                if (favourite != null)
                {
                    return Ok(new { status = false, message = "User Profile already added in favourite.." });
                }
                var favouriteobj = new FavouriteProfile() { UserId = userid, Id = userProfileId };
                _dbContext.FavouriteProfiles.Add(favouriteobj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "User Profile Added To Favourite.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("AddFavouriteToProfile")]
        public async Task<IActionResult> GetFavouriteClassifiedByUser(string userid)
        {
            try
            {
                var user = _applicationDbContext.Users.Find(userid);
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });
                }
                var Favourite = await _dbContext.FavouriteClassifieds.Where(a => a.UserId == userid).Include(a => a.ClassifiedAd).ToListAsync();
                var model = new
                {
                    status = true,
                    FavouriteClassified = Favourite
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetFavouriteProfileByUser")]
        public async Task<IActionResult> GetFavouriteProfileByUser(string userid)
        {
            try
            {
                var user = _applicationDbContext.Users.Find(userid);
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });
                }
                var Favourite = await _dbContext.FavouriteProfiles.Where(a => a.UserId == userid).ToListAsync();
                var model = new
                {
                    status = true,
                    FavouriteProfiles = Favourite
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPost]
        [Route("FlowerToClassifiedAds")]
        public IActionResult FlowerToClassifiedAds(FollowClassifiedVm folwers)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var addListingObj = _dbContext.ClassifiedAds.Where(e => e.ClassifiedAdId == folwers.ClssifiedId).FirstOrDefault();
                if (addListingObj == null)
                {
                    return Ok(new { status = false, message = "Classified Not Found" });
                }
                var folwerObj = new FolowClassified()
                {
                    UserId = folwers.UserId,
                    ClassifiedAdId = folwers.ClssifiedId

                };

                _dbContext.FolowClassifieds.Add(folwerObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Folwer To Classified Added Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpPost]
        [Route("FlowerToProfile")]
        public IActionResult FlowerToProfile(FolowProfileVm folwers)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var userProfile = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (userProfile == null)
                {
                    return Ok(new { status = false, message = "User Profile Not Found" });
                }
                var folwerObj = new FolowProfile()
                {
                    UserId = folwers.UserId,
                    Id = folwers.ProfileId

                };

                _dbContext.FolowProfile.Add(folwerObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Folwer To Profile Added Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpDelete]
        [Route("UnFlowerToClassified")]
        public IActionResult UnFlowerToClassified(FollowClassifiedVm folwers)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var classifiedAdsObj = _dbContext.ClassifiedAds.Where(e => e.ClassifiedAdId == folwers.ClssifiedId).FirstOrDefault();
                if (classifiedAdsObj == null)
                {
                    return Ok(new { status = false, message = "Classified Not Found" });
                }
                var folwerObj = _dbContext.FolowClassifieds.Where(e => e.ClassifiedAdId == folwers.ClssifiedId && e.UserId == folwers.UserId).FirstOrDefault();

                _dbContext.FolowClassifieds.Remove(folwerObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Folwer To Classified Deleted Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpDelete]
        [Route("UnFlowerToProfile")]
        public IActionResult UnFlowerToProfile(FolowProfileVm folwers)
        {

            try
            {
                var user = _applicationDbContext.Users.Where(a => a.Id == folwers.UserId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found" });
                }
                var classifiedAdsObj = _applicationDbContext.Users.Where(e => e.Id == folwers.ProfileId).FirstOrDefault();
                if (classifiedAdsObj == null)
                {
                    return Ok(new { status = false, message = "Profile Not Found" });
                }
                var folwerObj = _dbContext.FolowProfile.Where(e => e.Id == folwers.ProfileId && e.UserId == folwers.UserId).FirstOrDefault();

                _dbContext.FolowProfile.Remove(folwerObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Folwer To Profile Deleted Successfully.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetClassifiedAdsFolwerCount")]
        public IActionResult GetClassifiedAdsFolwerCount(int classifiedId)
        {

            try
            {

                int FolwersCount = _dbContext.FolowClassifieds.Where(e => e.ClassifiedAdId == classifiedId).Count();
                var model = new
                {
                    Status = true,
                    Count = FolwersCount
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetProfileFolwerCount")]
        public IActionResult GetProfileFolwerCount(string profileId)
        {

            try
            {

                int FolwersCount = _dbContext.FolowProfile.Where(e => e.Id == profileId).Count();
                var model = new
                {
                    Status = true,
                    Count = FolwersCount
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { status = true, Reason = e.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteFavouriteClassified")]
        public IActionResult DeleteFavouriteClassified(long classifiedId, string userid)
        {
            try
            {
                var favourite = _dbContext.FavouriteClassifieds.Where(e => e.ClassifiedAdId == classifiedId && e.UserId == userid).FirstOrDefault();
                if (favourite == null)
                {
                    return Ok(new { status = false, message = "Favourite Classified Not Found.." });
                }
                _dbContext.FavouriteClassifieds.Remove(favourite);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Classified Deleted Successfully From Favourite.." });

            }
            catch (Exception e)
            {

                return Ok(new { status = false, message = e.Message });

            }
        }
        [HttpDelete]
        [Route("DeleteFavouriteProfile")]
        public IActionResult DeleteFavouriteProfile(string profileId, string userid)
        {
            try
            {
                var favourite = _dbContext.FavouriteProfiles.Where(e => e.Id == profileId && e.UserId == userid).FirstOrDefault();
                if (favourite == null)
                {
                    return Ok(new { status = false, message = "Favourite Profile Not Found.." });
                }
                _dbContext.FavouriteProfiles.Remove(favourite);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Profile Deleted Successfully From Favourite.." });

            }
            catch (Exception e)
            {

                return Ok(new { status = false, message = e.Message });

            }
        }
        #endregion
        #region UserProfile
        [HttpGet]
        [Route("GetAllEducation")]
        public async Task<IActionResult> GetAllEducation(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var educationList = _applicationDbContext.Educations.Where(e => e.Id == user.Id);
                return Ok(new { Status = "Success", EducationList = educationList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidateEducationModel(Education education)
        {
            if (education.Year == 0 || education.Provider == null)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("AddEducation")]
        public async Task<IActionResult> AddEducation(Education education, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidateEducationModel(education))
                {
                    return Ok(new { Status = "false", Reason = "Enter All Required Faild" });
                }
                education.Id = user.Id;
                _applicationDbContext.Educations.Add(education);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Education = education });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteEducation")]
        public IActionResult DeleteEducation(int educationId)
        {
            if (educationId == 0)
            {
                return Ok(new { Status = "false", Reason = "Invalid Education Id" });
            }
            try
            {
                var educationObj = _applicationDbContext.Educations.Where(e => e.EducationID == educationId).FirstOrDefault();
                if (educationObj == null)
                {
                    return Ok(new { Status = "false", Reason = "Object Not Found" });
                }

                _applicationDbContext.Educations.Remove(educationObj);
                _applicationDbContext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Education Deleted Successfully" });


            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidateSkillModel(Skill skill)
        {
            if (skill.SkillTitle == null)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("AddSkill")]
        public async Task<IActionResult> AddSkill(Skill skill, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidateSkillModel(skill))
                {
                    return Ok(new { Status = "false", Reason = "Enter Skill Title" });
                }
                skill.Id = user.Id;
                _applicationDbContext.Skills.Add(skill);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Skill = skill });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllSkills")]
        public async Task<IActionResult> GetAllSkills(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var skillList = _applicationDbContext.Skills.Where(e => e.Id == user.Id);
                return Ok(new { Status = "Success", SkillList = skillList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteSkill")]
        public IActionResult DeleteSkill(int skillId)
        {
            if (skillId == 0)
            {
                return Ok(new { Status = "false", Reason = "Invalid Skill Id" });
            }
            try
            {
                var skillObj = _applicationDbContext.Skills.Where(e => e.SkillID == skillId).FirstOrDefault();
                if (skillObj == null)
                {
                    return Ok(new { Status = "false", Reason = "Object Not Found" });
                }

                _applicationDbContext.Skills.Remove(skillObj);
                _applicationDbContext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Skill Deleted Successfully" });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidateLanguageModel(Language language)
        {
            if (language.LanguageTitle == null)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("AddLanguage")]
        public async Task<IActionResult> AddLanguage(Language language, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidateLanguageModel(language))
                {
                    return Ok(new { Status = "false", Reason = "Enter Language Title" });
                }
                language.Id = user.Id;
                _applicationDbContext.Languages.Add(language);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Language = language });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllLanguages")]
        public async Task<IActionResult> GetAllLanguages(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var languagesList = _applicationDbContext.Languages.Where(e => e.Id == user.Id);
                return Ok(new { Status = "Success", LanguagesList = languagesList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        private bool ValidateLifeEventModel(LifeEvent lifeEvent)
        {
            if (lifeEvent.Caption == null || lifeEvent.EventType == null)
            {
                return false;
            }
            return true;
        }
        [HttpDelete]
        [Route("DeleteLanguage")]
        public IActionResult DeleteLanguage(int languageId)
        {
            if (languageId == 0)
            {
                return Ok(new { Status = "false", Reason = "Invalid Language Id" });
            }
            try
            {
                var languageObj = _applicationDbContext.Languages.Where(e => e.LanguageID == languageId).FirstOrDefault();
                if (languageObj == null)
                {
                    return Ok(new { Status = "false", Reason = "Object Not Found" });
                }

                _applicationDbContext.Languages.Remove(languageObj);
                _applicationDbContext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Language Deleted Successfully" });


            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpPost]
        [Route("AddLifeEvent")]
        public async Task<IActionResult> AddLifeEvent(LifeEvent lifeEvent, IFormFile media, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidateLifeEventModel(lifeEvent))
                {
                    return Ok(new { Status = "false", Reason = "Enter All Required Fields" });
                }
                if (media == null)
                {
                    return Ok(new { Status = "false", Reason = "Enter Life Event Media" });
                }
                if (media != null)
                {
                    string folder = "Images/ProfileImages/";
                    lifeEvent.Media = UploadImage(folder, media);
                }

                lifeEvent.Id = user.Id;
                _applicationDbContext.LifeEvents.Add(lifeEvent);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", LifeEvent = lifeEvent });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllLifeEvents")]
        public async Task<IActionResult> GetAllLifeEvents(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var lifeEventsList = _applicationDbContext.LifeEvents.Where(e => e.Id == user.Id);
                return Ok(new { Status = "Success", LifeEventsList = lifeEventsList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteLifeEvent")]
        public IActionResult DeleteLifeEvent(int lifeEventId)
        {
            if (lifeEventId == 0)
            {
                return Ok(new { Status = "false", Reason = "Invalid LifeEvent Id" });
            }
            try
            {
                var lifeEventObj = _applicationDbContext.LifeEvents.Where(e => e.LifeEventID == lifeEventId).FirstOrDefault();
                if (lifeEventObj == null)
                {
                    return Ok(new { Status = "false", Reason = "Object Not Found" });
                }

                _applicationDbContext.LifeEvents.Remove(lifeEventObj);
                _applicationDbContext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Life Event Deleted Successfully" });


            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidateInterestModel(Interest interest)
        {
            if (interest.InterestTitle == null)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("AddInterest")]
        public async Task<IActionResult> AddInterest(Interest interest, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidateInterestModel(interest))
                {
                    return Ok(new { Status = "false", Reason = "Enter Interest Title" });
                }


                interest.Id = user.Id;
                _applicationDbContext.Interests.Add(interest);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Interest = interest });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllInterest")]
        public async Task<IActionResult> GetAllInterest(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var interestsList = _applicationDbContext.Interests.Where(e => e.Id == user.Id);
                return Ok(new { Status = "Success", InterestsList = interestsList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteInterest")]
        public IActionResult DeleteInterest(int interestId)
        {
            if (interestId == 0)
            {
                return Ok(new { Status = "false", Reason = "Invalid interest Id" });
            }
            try
            {
                var interestObj = _applicationDbContext.Interests.Where(e => e.InterestID == interestId).FirstOrDefault();
                if (interestObj == null)
                {
                    return Ok(new { Status = "false", Reason = "Object Not Found" });
                }

                _applicationDbContext.Interests.Remove(interestObj);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Message = "Interest Deleted Successfully" });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidatePhotoModel(Photo photo)
        {
            if (photo.Caption == null)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        [Route("AddPhoto")]
        public async Task<IActionResult> AddPhoto(Photo photo, IFormFile image, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidatePhotoModel(photo))
                {
                    return Ok(new { Status = "false", Reason = "Enter Photo Caption" });
                }
                if (image == null)
                {
                    return Ok(new { Status = "false", Reason = "Plz Upload Image File" });
                }
                if (image != null)
                {
                    string folder = "Images/ProfileImages/";
                    photo.Image = UploadImage(folder, image);
                }

                photo.Id = user.Id;
                _applicationDbContext.Photos.Add(photo);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Photo = photo });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllPhotos")]
        public async Task<IActionResult> GetAllPhotos(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var photosList = _applicationDbContext.Photos.Where(e => e.Id == user.Id);

                return Ok(new { Status = "Success", PhotosList = photosList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool ValidatevideoModel(Video video)
        {
            if (video.Caption == null)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("AddVideo")]
        public async Task<IActionResult> AddVideo(Video video, IFormFile VideoFile, string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }
                if (!ValidatevideoModel(video))
                {
                    return Ok(new { Status = "false", Reason = "Enter Photo Caption" });
                }
                if (VideoFile == null)
                {
                    return Ok(new { Status = "false", Reason = "Plz Upload Video File" });
                }
                if (VideoFile != null)
                {
                    string folder = "Images/ProfileImages/";
                    video.VideoT = UploadImage(folder, VideoFile);
                }

                video.Id = user.Id;

                _applicationDbContext.Videos.Add(video);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = "Success", Video = video });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }

        }
        [HttpGet]
        [Route("GetAllVideos")]
        public async Task<IActionResult> GetAllVideos(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Ok(new { Status = "false", Reason = "User Not Found" });
                }

                var videosList = _applicationDbContext.Videos.Where(e => e.Id == user.Id);
                return Ok(new { Status = "Success", VideosList = videosList });

            }
            catch (Exception e)
            {
                return Ok(new { Status = "false", Reason = e.Message });
            }
        }
        [HttpPut]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(IFormFile Profilepic, IFormFile bannerpic, UserProfile userProfile)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userProfile.Email);
                if (user == null)
                    return BadRequest("Email Not Found..");
                user.Gender = userProfile.Gender;
                user.Bio = userProfile.Bio;
                user.Nationality = userProfile.Nationality;
                user.BirthDate = userProfile.BirthDate;
                user.Job = userProfile.Job;
                user.Qualification = userProfile.Qualification;
                //user.Location = userProfile.Location;
                user.FullName = userProfile.FullName;
                user.PhoneNumber = userProfile.Phone;
                user.FacebookLink = userProfile.FacebookLink;
                user.TwitterLink = userProfile.TwitterLink;
                user.InstagramLink = userProfile.InstagramLink;
                user.LinkedInLink = userProfile.LinkedInLink;
                user.LinkedInLink = userProfile.LinkedInLink;
                user.YoutubeLink = userProfile.YoutubeLink;
                user.NickName = userProfile.NickName;
                user.MaritalStatus = userProfile.MaritalStatus;
                user.City = userProfile.City;
                user.MapLocation = userProfile.MapLocation;
                user.Country = userProfile.Country;
                user.Phone2 = userProfile.Phone2;
                user.Folwers = userProfile.Folwers;
                user.Website = userProfile.Website;

                if (Profilepic != null)
                {
                    string folder = "Images/ProfileImages/";
                    user.ProfilePicture = UploadImage(folder, Profilepic);
                }
                if (bannerpic != null)
                {
                    string folder = "Images/BannerImages/";
                    user.Profilebanner = UploadImage(folder, bannerpic);
                }
                await _userManager.UpdateAsync(user);
                return Ok("updated successfully.. ");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        #endregion
        //====================================Classified Adz APIs=================================//

        [HttpGet]
        [Route("GetClassifiedAdzChart")]
        public async Task<ActionResult> GetClassifiedAdzChart()
        {
            //var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            //options.Converters.Add(new JsonStringEnumConverter());
            //JsonSerializerOptions options = new()
            //{
            //    ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //    WriteIndented = true,
            //    //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //    //PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //};
            try
            {


                var Data = _dbContext.ClassifiedAdsCategories.Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                    HasChild = _dbContext.ClassifiedAdsCategories.Any(x => x.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId),
                    AdTemplateConfigs = c.AdTemplateConfigs.OrderBy(l => l.SortOrder).ToList(),
                    AdTemplateOptions = c.AdTemplateConfigs.Select(l => l.AdTemplateOptions).ToList(),

                }).ToList();


                if (Data is null)
                {
                    return NotFound();
                }

                //return new JsonResult(Data, options)
                //{
                //    StatusCode = (int)HttpStatusCode.OK
                //};
                //   var newData =JsonSerializer.Serialize(Data, new JsonSerializerSettings { Formatting = Formatting.None });
                //var newData  =  JsonConvert.SerializeObject(Data, new JsonSerializerSettings { Formatting = Formatting.None, ReferenceLoopHandling =ReferenceLoopHandling.Ignore});

                // var newData =new JsonResult(Data, options);
                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        [HttpGet]
        [Route("GetChildsCatagory")]
        public async Task<ActionResult> GetChildsCatagory(int catagoryId)
        {

            try
            {
                var categories = _dbContext.ClassifiedAdsCategories.ToList();
                var parents = _dbContext.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryParentId == catagoryId).ToList();

                // map top-level categories and their children to DTOs
                var dtos = parents.Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                    ClassifiedAdsCategoryDescEn = c.ClassifiedAdsCategoryDescEn,
                    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                    Children = categories.Where(a => a.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId)
                                         .Select(b => new
                                         {
                                             ClassifiedAdsCategoryId = b.ClassifiedAdsCategoryId,
                                             ClassifiedAdsCategoryTitleAr = b.ClassifiedAdsCategoryTitleAr,
                                             ClassifiedAdsCategoryTitleEn = b.ClassifiedAdsCategoryTitleEn,
                                             ClassifiedAdsCategorySortOrder = b.ClassifiedAdsCategorySortOrder,
                                             ClassifiedAdsCategoryPic = b.ClassifiedAdsCategoryPic,
                                             ClassifiedAdsCategoryIsActive = b.ClassifiedAdsCategoryIsActive,
                                             ClassifiedAdsCategoryDescAr = b.ClassifiedAdsCategoryDescAr,
                                             ClassifiedAdsCategoryDescEn = b.ClassifiedAdsCategoryDescEn,
                                             ClassifiedAdsCategoryParentId = b.ClassifiedAdsCategoryParentId,
                                             Children = categories.Where(M => M.ClassifiedAdsCategoryParentId == b.ClassifiedAdsCategoryId)
                                                                  .Select(gc => new
                                                                  {
                                                                      ClassifiedAdsCategoryId = gc.ClassifiedAdsCategoryId,
                                                                      ClassifiedAdsCategoryTitleAr = gc.ClassifiedAdsCategoryTitleAr,
                                                                      ClassifiedAdsCategoryTitleEn = gc.ClassifiedAdsCategoryTitleEn,
                                                                      ClassifiedAdsCategorySortOrder = gc.ClassifiedAdsCategorySortOrder,
                                                                      ClassifiedAdsCategoryPic = gc.ClassifiedAdsCategoryPic,
                                                                      ClassifiedAdsCategoryIsActive = gc.ClassifiedAdsCategoryIsActive,
                                                                      ClassifiedAdsCategoryDescAr = gc.ClassifiedAdsCategoryDescAr,
                                                                      ClassifiedAdsCategoryDescEn = gc.ClassifiedAdsCategoryDescEn,
                                                                      ClassifiedAdsCategoryParentId = gc.ClassifiedAdsCategoryParentId,
                                                                      Children = categories.Where(K => K.ClassifiedAdsCategoryParentId == gc.ClassifiedAdsCategoryId)
                                                                  .Select(gcM => new
                                                                  {
                                                                      ClassifiedAdsCategoryId = gcM.ClassifiedAdsCategoryId,
                                                                      ClassifiedAdsCategoryTitleAr = gcM.ClassifiedAdsCategoryTitleAr,
                                                                      ClassifiedAdsCategoryTitleEn = gcM.ClassifiedAdsCategoryTitleEn,
                                                                      ClassifiedAdsCategorySortOrder = gcM.ClassifiedAdsCategorySortOrder,
                                                                      ClassifiedAdsCategoryPic = gcM.ClassifiedAdsCategoryPic,
                                                                      ClassifiedAdsCategoryIsActive = gcM.ClassifiedAdsCategoryIsActive,
                                                                      ClassifiedAdsCategoryDescAr = gcM.ClassifiedAdsCategoryDescAr,
                                                                      ClassifiedAdsCategoryDescEn = gcM.ClassifiedAdsCategoryDescEn,
                                                                      ClassifiedAdsCategoryParentId = gcM.ClassifiedAdsCategoryParentId,


                                                                  })

                                                                  })

                                         })
                });




                return Ok(new { Status = true, dtos, Message = "Process completed successfully" });

            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }


        }

        [HttpGet]
        [Route("GetClassifiedAdzCategories")]
        public async Task<ActionResult> GetClassifiedAdzCategories()
        {
            try
            {
                var Data = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryIsActive == true).OrderBy(e => e.ClassifiedAdsCategorySortOrder).Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                    ClassifiedAdsCategoryDescEn = c.ClassifiedAdsCategoryDescEn,
                    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                    HasChild = _dbContext.ClassifiedAdsCategories.Any(x => x.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId),

                }).ToListAsync();

                if (Data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }

        [HttpGet]
        [Route("GetClassifiedAdzChildCategories")]
        public async Task<ActionResult> GetClassifiedAdzChildCategories(int ClassifiedAdsCategoryId)
        {
            try
            {
                var Data = _dbContext.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryParentId == ClassifiedAdsCategoryId).Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                    ClassifiedAdsCategoryDescEn = c.ClassifiedAdsCategoryDescEn,
                    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                    HasChild = _dbContext.ClassifiedAdsCategories.Any(x => x.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId),

                }).ToListAsync();

                if (Data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }

        [HttpGet]
        [Route("GetClassifiedAdzParentCategory")]
        public async Task<ActionResult> GetClassifiedAdzParentCategory(int ClassifiedAdsCategoryId)
        {
            try
            {
                var Data = _dbContext.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == ClassifiedAdsCategoryId).Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                    ClassifiedAdsCategoryDescEn = c.ClassifiedAdsCategoryDescEn,
                    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                    HasChild = _dbContext.ClassifiedAdsCategories.Any(x => x.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId),

                }).FirstOrDefaultAsync();

                if (Data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }


        [HttpGet]
        [Route("GetClassifiedAdzConfigurationByCategoryId")]
        public async Task<ActionResult> GetClassifiedAdzConfigurationByCategoryId(int ClassifiedAdsCategoryId)
        {
            try
            {

                var Data = _dbContext.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == ClassifiedAdsCategoryId).Include(c => c.AdTemplateConfigs.OrderBy(e => e.SortOrder)).ThenInclude(c => c.AdTemplateOptions).ToList();

                if (Data is null)
                {
                    return NotFound();
                }


                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        [HttpGet]
        [Route("GetClassifiedAdzDetails")]
        public async Task<ActionResult> GetClassifiedAdzDetails(int ClassifiedAdId)
        {
            try
            {

                var Data = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == ClassifiedAdId).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).ToListAsync();

                if (Data is null)
                {
                    return NotFound();
                }


                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        [HttpGet]
        [Route("GetClassifiedAdzDetailsById")]
        public async Task<IActionResult> GetClassifiedAdzDetailsById(int ClassifiedAdId, string userId)
        {
            try
            {

                var classifiedVm = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == ClassifiedAdId).FirstOrDefault();
                if (classifiedVm == null)
                {
                    return Ok(new { Status = false, Message = "Classified Not Found" });
                }
                var user = await _userManager.FindByIdAsync(classifiedVm.UseId);


                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });

                }
                var ClassifiedAd = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == ClassifiedAdId).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    FullName = user.FullName,
                    ProfilePicture = user.ProfilePicture,
                    Job = user.Job,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    MainPic = c.MainPic,
                    Reel = c.Reel,
                    Location = c.Location,
                    PhoneNumber = c.PhoneNumber,
                    PhoneNumber2 = user.Phone2,
                    Description = c.Description,
                    City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
                    {
                        k.CityId,
                        k.CityTlAr,
                        k.CityTlEn,

                    }).FirstOrDefault(),
                    Arae = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
                    {
                        k.AreaId,
                        k.AreaTlAr,
                        k.AreaTlEn,

                    }).FirstOrDefault(),


                    AdsImages = c.AdsImages.Select(c => new
                    {
                        c.AdsImageId,
                        c.Image,
                    }).ToList(),
                    IsFavourite = _dbContext.FavouriteClassifieds.Any(o => o.ClassifiedAdId == ClassifiedAdId && o.UserId == userId),

                    AdContents = c.AdContents.Select(l => new
                    {
                        AdContentId = l.AdContentId,
                        AdTemplateConfigId = l.AdTemplateConfigId,

                        AdContentValues = l.AdContentValues.Select(k => new
                        {
                            AdContentValueId = k.AdContentValueId,
                            //ContentValue = k.ContentValue,
                            ContentValueEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            ContentValueAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,

                            //CheckBoxMultiple = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 ?  String.Join(", ", _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).Select(c => c.OptionEn).ToArray()): k.ContentValue,

                            FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
                            AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
                            AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
                        }).ToList()


                    }).ToList(),

                }).FirstOrDefault();


                if (ClassifiedAd is null)
                {
                    return NotFound();
                }
                classifiedVm.Views = classifiedVm.Views == null ? 0 : classifiedVm.Views + 1;
                _dbContext.SaveChanges();
                return Ok(new { Status = true, ClassifiedAd = ClassifiedAd, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetClassifiedAdzDetailsById")]
        //public async Task<IActionResult> GetClassifiedAdzDetailsById(int ClassifiedAdId, string userId)
        //{
        //    try
        //    {

        //        var classifiedVm = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == ClassifiedAdId).FirstOrDefault();
        //        if (classifiedVm == null)
        //        {
        //            return Ok(new { Status = false, Message = "Classified Not Found" });
        //        }
        //        var user = await _userManager.FindByIdAsync(classifiedVm.UseId);


        //        if (user == null)
        //        {
        //            return Ok(new { Status = false, Message = "User Not Found" });

        //        }
        //        var ClassifiedAd = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == ClassifiedAdId).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Select(c => new
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //            ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //            ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //            IsActive = c.IsActive,
        //            PublishDate = c.PublishDate,
        //            UseId = c.UseId,
        //            FullName = user.FullName,
        //            ProfilePicture = user.ProfilePicture,
        //            Job = user.Job,
        //            Views = c.Views,
        //            TitleAr = c.TitleAr,
        //            TitleEn = c.TitleEn,
        //            Price = c.Price,
        //            MainPic = c.MainPic,
        //            Reel = c.Reel,
        //            Location = c.Location,
        //            PhoneNumber = c.PhoneNumber,
        //            PhoneNumber2 = user.Phone2,
        //            Description = c.Description,
        //            City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
        //            {
        //                k.CityId,
        //                k.CityTlAr,
        //                k.CityTlEn,

        //            }).FirstOrDefault(),
        //            Arae = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
        //            {
        //                k.AreaId,
        //                k.AreaTlAr,
        //                k.AreaTlEn,

        //            }).FirstOrDefault(),


        //            AdsImages = c.AdsImages.Select(c => new
        //            {
        //                c.AdsImageId,
        //                c.Image,
        //            }).ToList(),
        //            IsFavourite = _dbContext.FavouriteClassifieds.Any(o => o.ClassifiedAdId == ClassifiedAdId && o.UserId == userId),

        //            AdContents = c.AdContents.Select(l => new
        //            {
        //                AdContentId = l.AdContentId,
        //                AdTemplateConfigId = l.AdTemplateConfigId,

        //                AdContentValues = l.AdContentValues.Select(k => new
        //                {
        //                    AdContentValueId = k.AdContentValueId,
        //                    //ContentValue = k.ContentValue,
        //                    ContentValueEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 5 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    ContentValueAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 5 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,

        //                    //CheckBoxMultiple = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 ?  String.Join(", ", _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).Select(c => c.OptionEn).ToArray()): k.ContentValue,

        //                    FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
        //                    AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
        //                }).ToList()


        //            }).ToList(),

        //        }).FirstOrDefault();


        //        if (ClassifiedAd is null)
        //        {
        //            return NotFound();
        //        }
        //        classifiedVm.Views = classifiedVm.Views == null ? 0 : classifiedVm.Views + 1;
        //        _dbContext.SaveChanges();
        //        return Ok(new { Status = true, ClassifiedAd = ClassifiedAd, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        #region 
        //[HttpGet]
        //[Route("GetAdsInCategory")]
        //public async Task<IActionResult> GetAdsInCategory(int CategoryID, int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var classifiedAds = await _dbContext.ClassifiedAds
        //            .Where(e => e.ClassifiedAdsCategoryId == CategoryID)
        //            .ToListAsync();
        //        var AllAds = await _dbContext.ClassifiedAds.ToListAsync();

        //        await LoadChildCategoriesAndAds(CategoryID, classifiedAds, AllAds);
        //        classifiedAds = classifiedAds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //        var classified = classifiedAds.Select(c => new
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,

        //            IsActive = c.IsActive,
        //            PublishDate = c.PublishDate,
        //            UseId = c.UseId,

        //            Views = c.Views,
        //            TitleAr = c.TitleAr,
        //            TitleEn = c.TitleEn,
        //            Price = c.Price,
        //            MainPic = c.MainPic,
        //            Description = c.Description,
        //            City = c.CityId,
        //            Area = c.AreaId

        //        }).ToList();


        //        return Ok(new { Status = true, ClassifiedAd = classified, Message = "Process completed successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }
        //}
        [HttpGet]
        [Route("GetAdsInCategory")]
        public async Task<IActionResult> GetAdsInCategory(int CategoryID, int page = 1, int pageSize = 10)
        {
            try
            {

                var catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == CategoryID).Select(e => e.SearchCatagoryLevel).ToList();
                var classified = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => e.ClassifiedAdsCategoryId == CategoryID || catList.Contains(e.ClassifiedAdsCategoryId.Value)).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    MainPic = c.MainPic,
                    Description = c.Description,
                    user = _userManager.FindByIdAsync(c.UseId),
                    City = c.CityId,
                    Area = c.AreaId,


                }).ToListAsync();
                if (classified != null)
                {
                    classified = classified.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    return Ok(new { Status = true, ClassifiedAd = classified, Message = "Process completed successfully" });

                }

                return Ok(new { Status = true, ClassifiedAd = classified, Message = "Process completed successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }


        private async Task LoadChildCategoriesAndAds(int categoryId, List<ClassifiedAd> classifiedAds, List<ClassifiedAd> AllclassifiedAds)
        {
            var childCategories = await _dbContext.ClassifiedAdsCategories
                .Where(e => e.ClassifiedAdsCategoryParentId == categoryId)
                .ToListAsync();

            foreach (var category in childCategories)
            {
                var categoryAds = AllclassifiedAds
                    .Where(e => e.ClassifiedAdsCategoryId == category.ClassifiedAdsCategoryId)
                    .ToList();

                classifiedAds.AddRange(categoryAds);

                await LoadChildCategoriesAndAds(category.ClassifiedAdsCategoryId, classifiedAds, AllclassifiedAds);
            }
        }
        private async Task LoadListChildCatagory(int categoryId, List<int> catagoryIds)
        {
            var childCategories = await _dbContext.ClassifiedAdsCategories
                   .Where(e => e.ClassifiedAdsCategoryParentId == categoryId)
                   .Select(e => e.ClassifiedAdsCategoryId).ToListAsync();
            //catagoryIds.AddRange(childCategories);

            foreach (int ChildcategoryId in childCategories)
            {
                catagoryIds.Add(ChildcategoryId);
                await LoadListChildCatagory(ChildcategoryId, catagoryIds);
            }
        }

        #endregion

        //#region 
        //[HttpGet]
        //[Route("GetAdsInCategory")]
        //public async Task<IActionResult> GetAdsInCategory(int CategoryID)
        //{
        //    try
        //    {
        //        ClassifiedAdsCategory category = _dbContext.ClassifiedAdsCategories.Include(e => e.ClassifiedAds).Where(e => e.ClassifiedAdsCategoryId == CategoryID).FirstOrDefault();

        //        if (category == null)
        //            return NotFound();

        //        List<ClassifiedAd> ads = GetAdsInCategoryAndChildren(category);
        //        //ads.Select(c => new
        //        //{
        //        //    ClassifiedAdId = c.ClassifiedAdId,
        //        //    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //        //    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //        //    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //        //    IsActive = c.IsActive,
        //        //    PublishDate = c.PublishDate,
        //        //    UseId = c.UseId,
        //        //    Views = c.Views,
        //        //    TitleAr = c.TitleAr,
        //        //    TitleEn = c.TitleEn,
        //        //    Price = c.Price,
        //        //    MainPic = c.MainPic,
        //        //    Description = c.Description,
        //        //    City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
        //        //    {
        //        //        k.CityId,
        //        //        k.CityTlAr,
        //        //        k.CityTlEn,

        //        //    }).FirstOrDefault(),
        //        //    Arae = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
        //        //    {
        //        //        k.AreaId,
        //        //        k.AreaTlAr,
        //        //        k.AreaTlEn,

        //        //    }).FirstOrDefault(),

        //        //}).ToList();
        //        return Ok(ads);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }
        //}

        //private List<ClassifiedAd> GetAdsInCategoryAndChildren(ClassifiedAdsCategory category)
        //{

        //    List<ClassifiedAd> ads = new List<ClassifiedAd>(category.ClassifiedAds);
        //    var Children = _dbContext.ClassifiedAdsCategories.Include(e => e.ClassifiedAds).Where(e => e.ClassifiedAdsCategoryParentId == category.ClassifiedAdsCategoryId).ToList();
        //    if (Children != null)
        //    {
        //        foreach (ClassifiedAdsCategory childCategory in Children)
        //        {
        //            List<ClassifiedAd> childAds = GetAdsInCategoryAndChildren(childCategory);
        //            ads.AddRange(childAds);
        //        }
        //    }

        //    return ads;
        //}

        //#endregion

        //[HttpGet]
        //[Route("GetAdsInCategory")]
        //public async Task<IActionResult> GetAdsInCategory(int CategoryID, int page = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        IQueryable<ClassifiedAd> query = _dbContext.ClassifiedAds
        //            .Where(e => e.ClassifiedAdsCategoryId == CategoryID);

        //        int totalCount = await query.CountAsync();

        //        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        //        List<ClassifiedAd> classifiedAds = await query
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        return Ok(new { Status = true, ClassifiedAd = classifiedAds, TotalPages = totalPages, Message = "Process completed successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }
        //}


        [HttpGet]
        [Route("GetAllClassifiedAdzDetailsByCategoryId")]
        public async Task<IActionResult> GetAllClassifiedAdzDetailsByCategoryId(int CategoryId)
        {
            try
            {
                var ClassifiedCategory = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == CategoryId).FirstOrDefault();
                if (ClassifiedCategory == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });

                }
                var ClassifiedAds = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdsCategoryId == CategoryId && c.IsActive == true).Include(c => c.ClassifiedAdsCategory).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,

                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    Description = c.Description,
                    MainPic = c.MainPic,
                    AdsImages = c.AdsImages.Select(c => new
                    {
                        c.AdsImageId,
                        c.Image,
                    }).ToList(),
                    AdContents = c.AdContents.Select(l => new
                    {
                        AdContentId = l.AdContentId,
                        AdTemplateConfigId = l.AdTemplateConfigId,

                        AdContentValues = l.AdContentValues.Select(k => new
                        {
                            AdContentValueId = k.AdContentValueId,
                            ContentValue = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
                            AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
                            AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,

                        }).ToList()


                    }).ToList(),

                }).ToListAsync();

                return Ok(new { Status = true, ClassifiedAds = ClassifiedAds, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        //ClassifiedAds = ClassifiedAds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //[HttpGet]
        //[Route("GetClassifiedAdsReels")]
        //public IActionResult GetClassifiedAdsReels()
        //{
        //    try
        //    {
        //        List<ReelResult> Result = new List<ReelResult>();
        //        _dbContext.LoadStoredProc("dbo.GetReels").ExecuteStoredProc((handler) =>
        //       {
        //           Result = handler.ReadToList<ReelResult>().ToList();

        //       });
        //        return Ok(new { Status = true, ClassifiedAds = Result, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetClassifiedAdsReels")]
        public IActionResult GetClassifiedAdsReels()
        {
            try
            {
                var users = _userManager.Users.ToList();
                var Result = _dbContext.ClassifiedAds.Where(e=>e.Reel!=null).OrderByDescending(e => e.PublishDate).Take(10).ToList().Select(e => new
                {
                    ClassifiedAdId = e.ClassifiedAdId,
                    ClassifiedAdsCategoryId = e.ClassifiedAdsCategoryId,
                    PublishDate = e.PublishDate,
                    Reel = e.Reel,
                    IsActive = e.IsActive,
                    Description = e.Description,
                    MainPic = e.MainPic,
                    UseId = e.UseId,
                    TitleEn = e.TitleEn,
                    TitleAr = e.TitleAr,
                    FullName = users.FirstOrDefault(x => x.Id == e.UseId).FullName,
                    PhoneNumber = e.PhoneNumber,
                    ProfilePicture = users.FirstOrDefault(x => x.Id == e.UseId).ProfilePicture



                }).ToList();
                // List<ReelResult> Result = new List<ReelResult>();
                // _dbContext.LoadStoredProc("dbo.GetReels").ExecuteStoredProc((handler) =>
                //{
                //    Result = handler.ReadToList<ReelResult>().OrderByDescending(e=>e.PublishDate).Take(10).ToList();

                //});
                return Ok(new { Status = true, ClassifiedAds = Result, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetClassifiedAdsReelsPerCategory")]
        public IActionResult GetClassifiedAdsReelsPerCategory(int ClassifiedAdsCategoryId)
        {
            try
            {
                var users = _userManager.Users.ToList();
                var Result = _dbContext.ClassifiedAds.Where(e=>e.ClassifiedAdsCategoryId== ClassifiedAdsCategoryId && e.Reel!=null).OrderByDescending(e => e.PublishDate).Take(10).ToList().Select(e => new
                {
                    ClassifiedAdId = e.ClassifiedAdId,
                    ClassifiedAdsCategoryId = e.ClassifiedAdsCategoryId,
                    PublishDate = e.PublishDate,
                    Reel = e.Reel,
                    IsActive = e.IsActive,
                    Description = e.Description,
                    MainPic = e.MainPic,
                    UseId = e.UseId,
                    TitleEn = e.TitleEn,
                    TitleAr = e.TitleAr,
                    FullName = users.FirstOrDefault(x => x.Id == e.UseId).FullName,
                    PhoneNumber = e.PhoneNumber,
                    ProfilePicture = users.FirstOrDefault(x => x.Id == e.UseId).ProfilePicture



                }).ToList();
                // List<ReelResult> Result = new List<ReelResult>();
                // _dbContext.LoadStoredProc("dbo.GetReels").ExecuteStoredProc((handler) =>
                //{
                //    Result = handler.ReadToList<ReelResult>().OrderByDescending(e=>e.PublishDate).Take(10).ToList();

                //});
                return Ok(new { Status = true, ClassifiedAds = Result, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
            //try
            //{
            //    List<ReelResult> Result = new List<ReelResult>();
            //    _dbContext.LoadStoredProc("dbo.GetReelPerCategory").WithSqlParam("ClassifiedAdsCategoryId", ClassifiedAdsCategoryId).ExecuteStoredProc((handler) =>
            //    {
            //        Result = handler.ReadToList<ReelResult>().ToList();

            //    });
            //    return Ok(new { Status = true, ClassifiedAds = Result, Message = "Process completed successfully" });
            //}

            //catch (Exception ex)
            //{
            //    return Ok(new { Status = false, Message = ex.Message });
            //}

        }

        private int CheckId(string value)
        {
            int convertedValu = 0;
            bool check = int.TryParse(value, out convertedValu);
            if (check)
            {
                return convertedValu;
            }
            else
            {
                convertedValu = 0;
                return convertedValu;
            }

        }
        //[HttpGet]
        //[Route("GetClassifiedAdzDetailsNew")]
        //public async Task<IActionResult> GetClassifiedAdzDetailsNew(int ClassifiedAdId)
        //{
        //    try
        //    {

        //        var ClassifiedAd = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == ClassifiedAdId).Include(c => c.ClassifiedAdsCategory).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Select(c => new AdzDetails
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //            ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //            IsActive = c.IsActive.Value,
        //            PublishDate = c.PublishDate.Value,
        //            UseId = c.UseId,
        //            Views = c.Views.Value,
        //            AdContent = c.AdContents.Select(l => new AdzDetailsContent
        //            {
        //                AdContentId = l.AdContentId,
        //                AdTemplateConfigId = l.AdTemplateConfigId,
        //                Values = l.AdContentValues.Select(k => k.ContentValue).ToList()
        //            }).ToList()


        //        }).ToList();
        //        if (ClassifiedAd is null)
        //        {
        //            return NotFound();
        //        }


        //        return Ok(new { Status = true, ClassifiedAd = ClassifiedAd, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = "Something went wrong" });
        //    }

        //}
        [HttpPost]
        [Route("AddClassifiedAd")]
        public async Task<ActionResult> AddClassifiedAd([FromForm] ClassifiedAdsVM classifiedAdsVM)
        {
            try
            {

                //var user = await _userManager.FindByIdAsync(classifiedAdsVM.UseId);


                //if (user == null)
                //{
                //    return Ok(new { Status = false, Message = "User Not Found" });

                //}
                var ClassifiedAdCat = _dbContext.ClassifiedAdsCategories.Find(classifiedAdsVM.ClassifiedAdsCategoryId);


                if (ClassifiedAdCat == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });

                }
                ClassifiedAd classifiedAd = new ClassifiedAd()
                {
                    IsActive = classifiedAdsVM.IsActive,
                    ClassifiedAdsCategoryId = classifiedAdsVM.ClassifiedAdsCategoryId,
                    PublishDate = DateTime.Now,
                    UseId = classifiedAdsVM.UseId,

                };
                _dbContext.ClassifiedAds.Add(classifiedAd);
                _dbContext.SaveChanges();

                foreach (var item in classifiedAdsVM.addContentVMs)
                {
                    var contentList = new List<AdContentValue>();
                    if (item.IsImage == true)
                    {

                        foreach (var elem in item.Values)
                        {
                            string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/ClassifiedAds");
                            var bytes = Convert.FromBase64String(elem);
                            string uniqePictureName = Guid.NewGuid() + ".jpeg";
                            string uploadedImagePath = Path.Combine(uploadFolder, uniqePictureName);
                            using (var imageFile = new FileStream(uploadedImagePath, FileMode.Create))
                            {
                                imageFile.Write(bytes, 0, bytes.Length);
                                imageFile.Flush();
                            }
                            var contentObj = new AdContentValue()
                            {
                                ContentValue = uniqePictureName
                            };
                            contentList.Add(contentObj);
                        }
                    }
                    else
                    {
                        foreach (var elem in item.Values)
                        {

                            var contentObj = new AdContentValue()
                            {
                                ContentValue = elem,
                            };
                            contentList.Add(contentObj);
                        }
                    }

                    var ContentValue = new AdContent()
                    {
                        ClassifiedAdId = classifiedAd.ClassifiedAdId,
                        AdTemplateConfigId = item.AdTemplateConfigId,
                        AdContentValues = contentList

                    };
                    _dbContext.AdContents.Add(ContentValue);
                    _dbContext.SaveChanges();
                }
                return Ok(new { Status = true, Message = "Classified Ads Added Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPost]
        [Route("AddNewClassifiedAd")]
        public async Task<ActionResult> AddNewClassifiedAd([FromForm] ClassifiedAdsVM classifiedAdsVM, List<MediaVm> mediaVms, IFormFile MainPic, IFormFile Reel, IFormFileCollection Slider)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(classifiedAdsVM.UseId);


                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });

                }
                if (user.AvailableClassified == 0)
                {
                    return Ok(new { Status = false, Message = "Can Not Add Classified Ads Please Subscribe To Any Wallet" });
                }
                var ClassifiedAdCat = _dbContext.ClassifiedAdsCategories.Find(classifiedAdsVM.ClassifiedAdsCategoryId);


                if (ClassifiedAdCat == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });

                }
                var cityObj = _dbContext.Cities.Find(classifiedAdsVM.CityId);


                if (cityObj == null)
                {
                    return Ok(new { Status = false, Message = "City Not Found" });

                }
                var AreaObj = _dbContext.Areas.Where(e => e.AreaId == classifiedAdsVM.AreaId && e.CityId == classifiedAdsVM.CityId).FirstOrDefault();


                if (AreaObj == null)
                {
                    return Ok(new { Status = false, Message = "Area Not Found" });

                }

                if (MainPic == null)
                {
                    return Ok(new { Status = false, Message = "Classified MainPic Is Required" });
                }
                //if (Slider == null)
                //{
                //    return Ok(new { Status = false, Message = "You Must Insert At Least 1 Pic" });
                //}
                //if (Slider.Count == 0)
                //{
                //    return Ok(new { Status = false, Message = "You Must Insert At Least 1 Pic" });
                //}



                ClassifiedAd classifiedAd = new ClassifiedAd()
                {
                    TitleAr = classifiedAdsVM.TitleAr,
                    TitleEn = classifiedAdsVM.TitleEn,
                    Price = classifiedAdsVM.Price,
                    Description = classifiedAdsVM.Description,
                    IsActive = classifiedAdsVM.IsActive,
                    ClassifiedAdsCategoryId = classifiedAdsVM.ClassifiedAdsCategoryId,
                    CityId = classifiedAdsVM.CityId,
                    AreaId = classifiedAdsVM.AreaId,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,
                    PhoneNumber = classifiedAdsVM.PhoneNumber,
                    Location = classifiedAdsVM.Location



                };

                var tags = "";
                classifiedAd.MainPic = UploadImage("Images/ClassifiedAds/", MainPic);
                if (Reel != null)
                {
                    classifiedAd.Reel = UploadImage("Images/ClassifiedAds/", Reel);
                }


                if (Slider != null)
                {
                    List<AdsImage> AdSlider = new List<AdsImage>();
                    foreach (var item in Slider)
                    {
                        var AdsImageObj = new AdsImage();
                        AdsImageObj.Image = UploadImage("Images/ClassifiedAds/", item);
                        AdSlider.Add(AdsImageObj);

                    }
                    classifiedAd.AdsImages = AdSlider;

                }
                _dbContext.ClassifiedAds.Add(classifiedAd);
                _dbContext.SaveChanges();
                user.AvailableClassified = user.AvailableClassified - 1;
                _applicationDbContext.SaveChanges();
                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var elem in mediaVms)
                        {
                            var contentListMediaValue = new List<AdContentValue>();
                            for (int i = 0; i < elem.Media.Count(); i++)
                            {
                                if (elem.Media[i] != null)
                                {
                                    string folder = "Images/ClassifiedAds/";
                                    var contentObj = new AdContentValue();
                                    contentObj.ContentValue = UploadImage(folder, elem.Media[i]);
                                    contentListMediaValue.Add(contentObj);
                                }

                            }
                            var ContentValueObj = new AdContent()
                            {
                                ClassifiedAdId = classifiedAd.ClassifiedAdId,
                                AdTemplateConfigId = elem.AdTemplateConfigId,
                                AdContentValues = contentListMediaValue

                            };
                            _dbContext.AdContents.Add(ContentValueObj);
                            _dbContext.SaveChanges();

                        }
                    }
                }
                if (classifiedAdsVM.addContentVMs != null)
                {
                    foreach (var item in classifiedAdsVM.addContentVMs)
                    {
                        var fieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == item.AdTemplateConfigId).FirstOrDefault().FieldTypeId;
                        var contentList = new List<AdContentValue>();
                        foreach (var elem in item.Values)
                        {

                            var contentObj = new AdContentValue()
                            {
                                ContentValue = elem,
                            };
                            contentList.Add(contentObj);
                            if (fieldTypeId == 3 || fieldTypeId == 6 || fieldTypeId == 13)
                            {
                                int AdTemplateOption = 0;
                                bool checkParsing = int.TryParse(elem, out AdTemplateOption);
                                if (checkParsing)
                                {
                                    var optionValues = _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == AdTemplateOption).FirstOrDefault();
                                    if (tags == "")
                                    {
                                        tags = optionValues.OptionEn + ";" + optionValues.OptionAr;

                                    }
                                    else
                                    {
                                        tags = tags + ";" + optionValues.OptionEn + ";" + optionValues.OptionAr;

                                    }
                                }
                            }
                        }


                        var ContentValue = new AdContent()
                        {
                            ClassifiedAdId = classifiedAd.ClassifiedAdId,
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            AdContentValues = contentList

                        };
                        _dbContext.AdContents.Add(ContentValue);


                    }
                    classifiedAd.Tags = tags;
                    _dbContext.Attach(classifiedAd).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }

                return Ok(new { Status = true, Message = "Classified Ads Added Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }

        [HttpGet]
        [Route("GetClassifiedAdsByCategoryParentId")]
        public async Task<ActionResult<List<ClassifiedAd>>> GetClassifiedAdsByCategoryParentId(int categoryParentId)
        {
            var categories = await GetCategoriesRecursive(categoryParentId);
            var Ads = await _dbContext.ClassifiedAds
                .Where(b => categories.Any(c => c.ClassifiedAdsCategoryId == b.ClassifiedAdsCategoryId))
                .ToListAsync();

            return Ads;
        }

        private async Task<List<ClassifiedAdsCategory>> GetCategoriesRecursive(int categoryId)
        {
            var categories = await _dbContext.ClassifiedAdsCategories
                .Where(c => c.ClassifiedAdsCategoryId == categoryId || c.ClassifiedAdsCategoryParentId == categoryId)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.InverseClassifiedAdsCategoryParent = await GetCategoriesRecursive(category.ClassifiedAdsCategoryId);
            }

            return categories;
        }


        [HttpGet]
        [Route("GetClassifiedAdzDetailsByUserId")]
        public async Task<IActionResult> GetClassifiedAdzDetailsByUserId(string userid)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userid);


                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });

                }
                var ClassifiedAd = _dbContext.ClassifiedAds.Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(c => c.UseId == user.Id && c.IsActive == true).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    MainPic = c.MainPic,
                    Description = c.Description,
                    PhoneNumber = c.PhoneNumber,
                    AdsImages = c.AdsImages.Select(c => new
                    {
                        c.AdsImageId,
                        c.Image,
                    }).ToList(),
                    AdContents = c.AdContents.Select(l => new
                    {
                        AdContentId = l.AdContentId,
                        AdTemplateConfigId = l.AdTemplateConfigId,

                        AdContentValues = l.AdContentValues.Select(k => new
                        {
                            AdContentValueId = k.AdContentValueId,
                            //ContentValue = k.ContentValue,
                            ContentValue = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
                            AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
                            AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
                        }).ToList()


                    }).ToList(),

                }).ToList();


                if (ClassifiedAd is null)
                {
                    return NotFound();
                }


                return Ok(new { Status = true, ClassifiedAd = ClassifiedAd, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetFilterClassifiedAds")]
        //public async Task<IActionResult> GetFilterClassifiedAds([FromForm] FilterModelVm classifiedFilterVm)
        //{
        //    try
        //    {
        //        List<AdContentValue> AdContentValueList = new List<AdContentValue>();
        //        List<object> ClassifiedList = new List<object>();
        //        AdContentValueList = await _dbContext.AdContentValues.Include(e => e.AdContent).ToListAsync();
        //        foreach (var item in classifiedFilterVm.adContentVMs)
        //        {
        //            var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && e.ContentValue == item.Value).Select(e => e.ContentValue).ToListAsync();
        //            AdContentValueList = AdContentValueList.Where(e => Values.Contains(e.ContentValue)).ToList();
        //            // AdContentValueList = AdContentValueList.Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && e.ContentValue == item.Value).ToList();

        //        }

        //        foreach (var item in AdContentValueList)
        //        {
        //            var classified = await _dbContext.ClassifiedAds.Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(a => a.ClassifiedAdId == item.AdContent.ClassifiedAdId && a.ClassifiedAdsCategoryId == classifiedFilterVm.ClassifiedAdsCategoryId && a.IsActive == true).Select(c => new
        //            {
        //                ClassifiedAdId = c.ClassifiedAdId,
        //                ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //                IsActive = c.IsActive,
        //                PublishDate = c.PublishDate,
        //                UseId = c.UseId,
        //                Views = c.Views,
        //                AdContents = c.AdContents.Select(l => new
        //                {
        //                    AdContentId = l.AdContentId,
        //                    AdTemplateConfigId = l.AdTemplateConfigId,

        //                    AdContentValues = l.AdContentValues.Select(k => new
        //                    {
        //                        AdContentValueId = k.AdContentValueId,
        //                        ContentValue = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                        FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                        AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
        //                        AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
        //                    }).ToList()


        //                }).ToList(),

        //            }).FirstOrDefaultAsync();
        //            ClassifiedList.Add(classified);
        //        }




        //        if (ClassifiedList is null)
        //        {
        //            return NotFound();
        //        }


        //        return Ok(new { Status = true, ClassifiedList = ClassifiedList, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        /// <summary>
        /// ////////Old Filter
        /// </summary>
        /// <param name="classifiedFilterVm"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("GetFilterClassifiedAds")]
        //public async Task<IActionResult> GetFilterClassifiedAds([FromForm] FilterModelVm classifiedFilterVm)
        //{
        //    try
        //    {
        //        List<long> ClassifiedIds = new List<long>();
        //        var classifiedAdsList = _dbContext.ClassifiedAds.Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(a => a.ClassifiedAdsCategoryId == classifiedFilterVm.ClassifiedAdsCategoryId).Select(c => new
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //            IsActive = c.IsActive,
        //            PublishDate = c.PublishDate,
        //            UseId = c.UseId,
        //            Views = c.Views,
        //            TitleAr = c.TitleAr,
        //            TitleEn = c.TitleEn,
        //            Price = c.Price,
        //            Description = c.Description,
        //            AdsImages = c.AdsImages.Select(c => new
        //            {
        //                c.AdsImageId,
        //                c.Image,
        //            }).ToList(),
        //            AdContents = c.AdContents.Select(l => new
        //            {
        //                AdContentId = l.AdContentId,
        //                AdTemplateConfigId = l.AdTemplateConfigId,

        //                AdContentValues = l.AdContentValues.Select(k => new
        //                {
        //                    AdContentValueId = k.AdContentValueId,
        //                    ContentValue = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
        //                    AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
        //                }).ToList()


        //            }).ToList(),

        //        }).ToList();
        //        if (classifiedFilterVm.adContentVMs != null)
        //        {
        //            foreach (var item in classifiedFilterVm.adContentVMs)
        //            {
        //                var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && e.ContentValue == item.Value).Select(e => e.ContentValue).ToListAsync();
        //                ClassifiedIds = _dbContext.AdContents.Include(e => e.AdContentValues).Where(e => Values.Contains(e.AdContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedAdId).ToList();
        //                classifiedAdsList = classifiedAdsList.Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();

        //            }
        //        }






        //        if (classifiedAdsList is null)
        //        {
        //            return NotFound();
        //        }


        //        return Ok(new { Status = true, ClassifiedList = classifiedAdsList, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        ////////Filter With Multi Values
        //[HttpGet]
        //[Route("GetFilterClassifiedAds")]
        //public async Task<IActionResult> GetFilterClassifiedAds([FromForm] FilterModelVm classifiedFilterVm, int page = 1, int pageSize = 10)
        //{


        //    try
        //    {
        //        var classifiedCatagory = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == classifiedFilterVm.ClassifiedAdsCategoryId).FirstOrDefault();
        //        if (classifiedCatagory == null)
        //        {
        //            return Ok(new { Status = false, Message = "Catagory Not Found" });
        //        }
        //        var catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == classifiedCatagory.ClassifiedAdsCategoryId).Select(e => e.SearchCatagoryLevel).ToList();


        //        List<long> ClassifiedIds = new List<long>();
        //        var classifiedAdsList = await _dbContext.ClassifiedAds.Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(a => catList.Contains(a.ClassifiedAdsCategoryId.Value) || a.ClassifiedAdsCategoryId == classifiedCatagory.ClassifiedAdsCategoryId).Select(c => new
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //            IsActive = c.IsActive,
        //            PublishDate = c.PublishDate,
        //            UseId = c.UseId,
        //            Views = c.Views,
        //            TitleAr = c.TitleAr,
        //            TitleEn = c.TitleEn,
        //            Price = c.Price,
        //            CityId = c.CityId,
        //            AreaId = c.AreaId,
        //            MainPic = c.MainPic,
        //            Description = c.Description,
        //            user = _userManager.FindByIdAsync(c.UseId),
        //            AdsImages = c.AdsImages.Select(c => new
        //            {
        //                c.AdsImageId,
        //                c.Image,
        //            }).ToList(),
        //            AdContents = c.AdContents.Select(l => new
        //            {
        //                AdContentId = l.AdContentId,
        //                AdTemplateConfigId = l.AdTemplateConfigId,

        //                AdContentValues = l.AdContentValues.Select(k => new
        //                {
        //                    AdContentValueId = k.AdContentValueId,
        //                    ContentValue = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
        //                    AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
        //                }).ToList()


        //            }).ToList(),

        //        }).ToListAsync();
        //        if (classifiedAdsList != null)
        //        {
        //            if (classifiedFilterVm.CityId != 0)
        //            {
        //                classifiedAdsList = classifiedAdsList.Where(e => e.CityId == classifiedFilterVm.CityId).ToList();
        //            }
        //            if (classifiedFilterVm.AreaId != 0)
        //            {
        //                classifiedAdsList = classifiedAdsList.Where(e => e.AreaId == classifiedFilterVm.AreaId).ToList();
        //            }
        //            if (classifiedFilterVm.FromPrice != 0 && classifiedFilterVm.ToPrice != 0)
        //            {
        //                classifiedAdsList = classifiedAdsList.Where(e => e.Price > classifiedFilterVm.FromPrice && e.Price < classifiedFilterVm.ToPrice).ToList();
        //            }
        //            if (classifiedFilterVm.adContentVMs != null)
        //            {
        //                foreach (var item in classifiedFilterVm.adContentVMs)
        //                {
        //                    if (item.AdTemplateConfigId != 0)
        //                    {
        //                        var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && item.Values.Contains(e.ContentValue)).Select(e => e.ContentValue).ToListAsync();
        //                        //var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId &&  e.ContentValue == item.Values[0]).Select(e => e.ContentValue).ToListAsync();
        //                        ClassifiedIds = _dbContext.AdContents.Include(e => e.AdContentValues).Where(e => Values.Contains(e.AdContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedAdId).ToList();
        //                        classifiedAdsList = classifiedAdsList.Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();
        //                        //classifiedAdsList = classifiedAdsList.Single(c => c.category_id == categoryNumber)Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();

        //                    }

        //                }
        //            }




        //        }

        //        if (classifiedAdsList is null)
        //        {
        //            return NotFound();
        //        }
        //        classifiedAdsList = classifiedAdsList.Where(e => e.IsActive == true).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //        return Ok(new { Status = true, ClassifiedList = classifiedAdsList, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetFilterClassifiedAds")]
        public async Task<IActionResult> GetFilterClassifiedAds([FromForm] FilterModelVm classifiedFilterVm, int page = 1, int pageSize = 10)
        {


            try
            {
                var classifiedCatagory = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == classifiedFilterVm.ClassifiedAdsCategoryId).FirstOrDefault();
                if (classifiedCatagory == null)
                {
                    return Ok(new { Status = false, Message = "Catagory Not Found" });
                }
                var catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == classifiedCatagory.ClassifiedAdsCategoryId).Select(e => e.SearchCatagoryLevel).ToList();


                List<long> ClassifiedIds = new List<long>();
                var classifiedAdsList = await _dbContext.ClassifiedAds.Include(c => c.AdsImages).Where(a => catList.Contains(a.ClassifiedAdsCategoryId.Value) || a.ClassifiedAdsCategoryId == classifiedCatagory.ClassifiedAdsCategoryId).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    CityId = c.CityId,
                    AreaId = c.AreaId,
                    MainPic = c.MainPic,
                    Description = c.Description,
                    user = _userManager.FindByIdAsync(c.UseId),
                    AdsImages = c.AdsImages.Select(c => new
                    {
                        c.AdsImageId,
                        c.Image,
                    }).ToList(),

                }).ToListAsync();
                if (classifiedAdsList != null)
                {
                    if (classifiedFilterVm.CityId != 0)
                    {
                        classifiedAdsList = classifiedAdsList.Where(e => e.CityId == classifiedFilterVm.CityId).ToList();
                    }
                    if (classifiedFilterVm.AreaId != 0)
                    {
                        classifiedAdsList = classifiedAdsList.Where(e => e.AreaId == classifiedFilterVm.AreaId).ToList();
                    }
                    if (classifiedFilterVm.FromPrice != 0 && classifiedFilterVm.ToPrice != 0)
                    {
                        classifiedAdsList = classifiedAdsList.Where(e => e.Price > classifiedFilterVm.FromPrice && e.Price < classifiedFilterVm.ToPrice).ToList();
                    }
                    if (classifiedFilterVm.adContentVMs != null)
                    {
                        foreach (var item in classifiedFilterVm.adContentVMs)
                        {
                            if (item.AdTemplateConfigId != 0)
                            {
                                var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && item.Values.Contains(e.ContentValue)).Select(e => e.ContentValue).ToListAsync();
                                //var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId &&  e.ContentValue == item.Values[0]).Select(e => e.ContentValue).ToListAsync();
                                ClassifiedIds = _dbContext.AdContents.Include(e => e.AdContentValues).Where(e => Values.Contains(e.AdContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedAdId).ToList();
                                classifiedAdsList = classifiedAdsList.Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();
                                //classifiedAdsList = classifiedAdsList.Single(c => c.category_id == categoryNumber)Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();

                            }

                        }
                    }




                }

                if (classifiedAdsList is null)
                {
                    return NotFound();
                }
                classifiedAdsList = classifiedAdsList.Where(e => e.IsActive == true).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new { Status = true, ClassifiedList = classifiedAdsList, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }

        [HttpGet]
        [Route("GetSampleCodeForFilter")]
        public async Task<IActionResult> GetSampleCodeForFilter()
        {


            try
            {
                var list1 = new List<int>() { 1, 2, 3, 4, 5 };
                var list2 = new List<int>() { 1, 2, 6, 7, 8 };
                list1 = list1.Where(e => list2.Contains(1)).ToList();
                return Ok(new { Status = true, list1 = list1, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetFilterClassifiedAdsNew")]
        //public async Task<IActionResult> GetFilterClassifiedAdsNew([FromForm] FilterModelVm classifiedFilterVm)
        //{
        //    try
        //    {

        //        List<ClassifiedAd> ClassifiedList = new List<ClassifiedAd>();

        //            ClassifiedList =  _dbContext.ClassifiedAds.Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(c=>c.ClassifiedAdsCategoryId == classifiedFilterVm.ClassifiedAdsCategoryId && c.IsActive == true).ToList();

        //        foreach (var item in classifiedFilterVm.adContentVMs)
        //        {
        //            var Values = await _dbContext.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && e.ContentValue == item.Value).Select(e => e.ContentValue).ToListAsync();
        //            ClassifiedList = ClassifiedList.Where(e => Values.Contains(ContentValue)).ToList();
        //            // AdContentValueList = AdContentValueList.Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && e.ContentValue == item.Value).ToList();

        //        }




        //        if (ClassifiedList is null)
        //        {
        //            return NotFound();
        //        }


        //        return Ok(new { Status = true, ClassifiedList = ClassifiedList, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}

        [HttpPost]
        [Route("AddFavouriteClassifiedAds")]
        public IActionResult AddFavouriteClassifiedAds(long classifiedId, string userid)
        {
            try
            {

                var buisness = _dbContext.ClassifiedAds.Find(classifiedId);
                var user = _applicationDbContext.Users.Find(userid);
                if (buisness == null)
                {
                    return Ok(new { status = false, message = "classified Not Found." });

                }
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });

                }
                var favourite = _dbContext.FavouriteClassifieds.Where(a => a.ClassifiedAdId == classifiedId && a.UserId == userid).FirstOrDefault();
                if (favourite != null)
                {
                    return Ok(new { status = false, message = "classified already added in favourite.." });
                }
                var favouriteobj = new FavouriteClassified() { UserId = userid, ClassifiedAdId = classifiedId };
                _dbContext.FavouriteClassifieds.Add(favouriteobj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Classified Added To Favourite.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }

        //====================================BusinessDirectory APIs=================================//
        #region BusinessDirectory APIs
        #region GetBusinessCategory
        [HttpGet]
        [Route("GetBusinessCategory")]
        public ActionResult GetBusinessCategories()
        {
            try
            {
                var BusinessCategory = _dbContext.BusinessCategories.Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
                    BusinessCategorySortOrder = c.BusinessCategorySortOrder,
                    BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
                    BusinessCategoryIsActive = c.BusinessCategoryIsActive,
                    BusinessCategoryDescAr = c.BusinessCategoryDescAr,
                    BusinessCategoryDescEn = c.BusinessCategoryDescEn,
                    BusinessCategoryParentId = c.BusinessCategoryParentId,
                    HasChild = _dbContext.BusinessCategories.Any(x => x.BusinessCategoryParentId == c.BusinessCategoryId),

                }).ToList();


                if (BusinessCategory is null)
                {
                    return NotFound(new { Status = false, Message = "There are no Business Categories" });
                }


                return Ok(new { Status = true, BusinessCategory });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetBusinessDirectory
        [HttpGet]
        [Route("GetBusinessDirectory")]
        public ActionResult GetBusinessDirectory()
        {
            try
            {



                List<ClassifiedBusiness> ClassifiedBusiness = new List<ClassifiedBusiness>();

                var Business = _dbContext.ClassifiedBusiness.Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    Mainpic = c.Mainpic,
                    Title = c.Title,
                    UseId = c.UseId,
                    ClassifiedBusinessId = c.ClassifiedBusinessId,

                    BusinessContents = _dbContext.BusinessContents.Include(e => e.BusinessTemplateConfigs).Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.BusinessTemplateConfigs.FieldTypeId == 14).Select(e => e.BusinessContentValues).ToList()
                }).ToList();


                //var Business = _dbContext.ClassifiedBusiness.Select(c => new
                //{
                //    BusinessCategoryId = c.BusinessCategoryId,
                //     Mainpic = c.Mainpic,
                //    Title = c.Title,
                //    UseId= c.UseId,
                //    ClassifiedBusinessId=  c.ClassifiedBusinessId,


                //BusinessContents = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
                //    {

                //        AdContentValues = l.BusinessContentValues.Select(k => new
                //        {
                //            BusinessContentValueId = k.BusinessContentValueId,
                //            ContentValue = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId&&e.FieldTypeId==14).FirstOrDefault(),
                //            //FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
                //            //BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
                //            //BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,
                //        })
                //    })
                //        .ToList(),
                //}).ToList();


                if (Business is null)
                {
                    return NotFound(new { Status = false, Message = "There are no Business" });
                }


                return Ok(new { Status = true, Business });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetBusinessCategoryByCategoryId
        [HttpGet]
        [Route("GetBusinessCategoryByCategoryId")]
        public ActionResult GetBusinessCategoryByCategoryId(int BusinessCategoryId)
        {
            try
            {

                var Category = _dbContext.BusinessCategories.Where(c => c.BusinessCategoryId == BusinessCategoryId).Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
                    BusinessCategorySortOrder = c.BusinessCategorySortOrder,
                    BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
                    BusinessCategoryIsActive = c.BusinessCategoryIsActive,
                    BusinessCategoryDescAr = c.BusinessCategoryDescAr,
                    BusinessCategoryDescEn = c.BusinessCategoryDescEn,
                    BusinessTemplateConfigs = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == c.BusinessCategoryId).OrderBy(a => a.SortOrder).ToList(),
                    BusinessTemplateOptions = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == c.BusinessCategoryId).Select(l => l.BusinessTemplateOptions).ToList(),
                }).ToList();
                if (Category is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Category" });

                }


                return Ok(new { Status = true, Category });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region AddBusiness

        [HttpPost]
        [Route("AddBusiness")]
        public async Task<ActionResult> AddBusiness([FromForm] BusinessVM BusinessVM, IFormFile MainPic, IFormFile Logo, IFormFile Reel, List<BusinessMediaVm> mediaVms, List<WorkingHoursVM> Workinghoursvm, IFormFileCollection Slider)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(BusinessVM.UseId);
                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });
                }
                var plan = _dbContext.BussinessPlans.Where(e => e.BussinessPlanId == BusinessVM.PlanId).FirstOrDefault();

                if (plan == null)
                {
                    return Ok(new { Status = false, Message = "Please,Select Plan" });
                }
                var BusinessCat = _dbContext.BusinessCategories.Find(BusinessVM.BusinessCategoryId);
                if (BusinessCat == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });
                }
                var cityObj = _dbContext.Cities.Find(BusinessVM.CityId);


                if (cityObj == null)
                {
                    return Ok(new { Status = false, Message = "City Not Found" });

                }
                var AreaObj = _dbContext.Areas.Where(e => e.AreaId == BusinessVM.AreaId && e.CityId == BusinessVM.CityId).FirstOrDefault();


                if (AreaObj == null)
                {
                    return Ok(new { Status = false, Message = "Area Not Found" });

                }
                if (Logo == null)
                {
                    return Ok(new { Status = false, Message = "Logo Is Required" });

                }

                ClassifiedBusiness classifiedBusiness = new ClassifiedBusiness()
                {
                    IsActive = BusinessVM.IsActive,
                    BusinessCategoryId = BusinessVM.BusinessCategoryId,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,
                    Title = BusinessVM.Title,
                    Description = BusinessVM.Description,
                    Email = BusinessVM.Email,
                    phone = BusinessVM.phone,
                    Address = BusinessVM.Address,
                    CityId = BusinessVM.CityId,
                    AreaId = BusinessVM.AreaId,
                    Location = BusinessVM.Location,

                };

                if (MainPic != null)
                {
                    string folder = "Images/BusinessMedia/";
                    classifiedBusiness.Mainpic = UploadImage(folder, MainPic);
                }
                if (Reel != null)
                {
                    string folder = "Images/BusinessMedia/";
                    classifiedBusiness.Reel = UploadImage(folder, Reel);
                }
                if (Logo != null)
                {
                    string folder = "Images/BusinessMedia/";
                    classifiedBusiness.Logo = UploadImage(folder, Logo);
                }
                if (Slider != null)
                {
                    List<BDImage> BDSlider = new List<BDImage>();
                    foreach (var item in Slider)
                    {
                        var BDImageObj = new BDImage();
                        BDImageObj.Image = UploadImage("Images/BusinessMedia/", item);
                        BDSlider.Add(BDImageObj);

                    }
                    classifiedBusiness.BDImages = BDSlider;

                }
                List<BusinessWorkingHours> workingHours = new List<BusinessWorkingHours>();
                if (Workinghoursvm != null)
                {
                    foreach (var item in Workinghoursvm)
                    {
                        Models.BusinessWorkingHours businessWorkingHoursobj = new Models.BusinessWorkingHours()
                        {
                            StartTime1 = item.StartTime1,
                            StartTime2 = item.StartTime2,
                            EndTime1 = item.EndTime1,
                            EndTime2 = item.EndTime2,
                            Day = item.Day,
                            Isclosed = item.Isclosed,
                            ClassifiedBusinessId = item.ClassifiedBusinessId
                        };

                        workingHours.Add(businessWorkingHoursobj);
                    }
                }
                classifiedBusiness.businessWorkingHours = workingHours;
                _dbContext.ClassifiedBusiness.Add(classifiedBusiness);
                _dbContext.SaveChanges();

                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var item in mediaVms)
                        {
                            var contentListMediaValue = new List<BusinessContentValue>();
                            for (int i = 0; i < item.Media.Count(); i++)
                            {
                                if (item.Media[i] != null)
                                {
                                    string folder = "Images/BusinessMedia/";
                                    var contentObj = new BusinessContentValue();
                                    contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                    contentListMediaValue.Add(contentObj);
                                }

                            }
                            var ContentValueObj = new BusinessContent()
                            {
                                ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId,
                                BusinessTemplateConfigId = item.BusinessTemplateConfigId,
                                BusinessContentValues = contentListMediaValue

                            };
                            _dbContext.BusinessContents.Add(ContentValueObj);
                            _dbContext.SaveChanges();

                        }
                    }
                }
                foreach (var item in BusinessVM.BusinessContentVMS)
                {
                    var contentList = new List<BusinessContentValue>();

                    foreach (var value in item.Values)
                    {

                        var contentObj = new BusinessContentValue()
                        {
                            ContentValue = value,
                        };
                        contentList.Add(contentObj);
                    }
                    var ContentValue = new BusinessContent()
                    {
                        ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId,
                        BusinessTemplateConfigId = item.BusinessTemplateConfigId,
                        BusinessContentValues = contentList
                    };
                    _dbContext.BusinessContents.Add(ContentValue);
                    _dbContext.SaveChanges();
                }
                double totalCost = plan.Price.Value;
                var subscription = new BusiniessSubscription();
                subscription.BussinessPlanId = BusinessVM.PlanId;
                subscription.ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId;
                subscription.Price = totalCost;
                subscription.StartDate = DateTime.Now;
                subscription.PaymentMethodId = BusinessVM.PaymentMethodId;
                subscription.EndDate = DateTime.Now.AddMonths(plan.DurationInMonth.Value);
                _dbContext.BusiniessSubscriptions.Add(subscription);
                _dbContext.SaveChanges();
                if (BusinessVM.PaymentMethodId == 1)
                {
                    bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                    var TestToken = _configuration["TestToken"];
                    var LiveToken = _configuration["LiveToken"];
                    if (Fattorahstatus) // fattorah live
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = classifiedBusiness.Title,
                            NotificationOption = "LNK",
                            InvoiceValue = subscription.Price,
                            CallBackUrl = "https://albaheth.me/FattorahBusinessPlantSuccess",
                            ErrorUrl = "https://albaheth.me/FattorahBusinessPlanFalied",
                            UserDefinedField = subscription.BusiniessSubscriptionId
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Ok(new { status = true, Message = "Business Subscription successfully!", paymentUrl = InvoiceRes.InvoiceURL });



                        }
                        else
                        {

                            _dbContext.BusiniessSubscriptions.Remove(subscription);

                            return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });
                        }
                    }
                    else               //fattorah test
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = classifiedBusiness.Title,
                            NotificationOption = "LNK",
                            InvoiceValue = subscription.Price,
                            CallBackUrl = "https://albaheth.me/FattorahBusinessPlantSuccess",
                            ErrorUrl = "https://albaheth.me/FattorahBusinessPlanFalied",
                            UserDefinedField = subscription.BusiniessSubscriptionId
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Ok(new { status = true, Message = "Business Added Successfully!", paymentUrl = InvoiceRes.InvoiceURL });
                        }
                        else
                        {

                            _dbContext.BusiniessSubscriptions.Remove(subscription);

                            return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });
                        }
                    }



                }

            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
            return Ok();
        }
        #endregion
        #region GetBusinessDetailsById
        [HttpGet]
        [Route("GetBusinessDetailsById")]
        public async Task<IActionResult> GetBusinessDetailsById(long ClassifiedBusinessId, string userId)
        {
            try
            {

                var classifiedBusinessVm = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == ClassifiedBusinessId).FirstOrDefault();
                if (classifiedBusinessVm == null)
                {
                    return Ok(new { Status = false, Message = "Business Not Found" });
                }
                var user = await _userManager.FindByIdAsync(classifiedBusinessVm.UseId);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });

                }
                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == ClassifiedBusinessId).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Reel = c.Reel,
                    Location = c.Location,
                    Businessworkinghours = _dbContext.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(a => new
                    {
                        a.Day,
                        a.EndTime1,
                        a.EndTime2,
                        a.Isclosed,
                        a.StartTime1,
                        a.StartTime2
                    }).ToList(),
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    FullName = user.FullName,
                    ProfilePicture = user.ProfilePicture,
                    Job = user.Job,
                    Views = c.Views,
                    Reviews = _dbContext.Reviews.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
                    {
                        ClassifiedBusinessId = l.ClassifiedBusinessId,
                        Name = l.Name,
                        Rating = l.Rating,
                        ReviewDate = l.ReviewDate,
                        ReviewId = l.ReviewId,
                        Title = l.Title,
                        Email = l.Email,
                    }).ToList(),
                    IsFavourite = _dbContext.FavouriteBusiness.Any(o => o.ClassifiedBusinessId == ClassifiedBusinessId && o.UserId == userId),
                    City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
                    {
                        k.CityId,
                        k.CityTlAr,
                        k.CityTlEn,

                    }).FirstOrDefault(),
                    Area = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
                    {
                        k.AreaId,
                        k.AreaTlAr,
                        k.AreaTlEn,

                    }).FirstOrDefault(),
                    Address = c.Address,
                    Logo = c.Logo,
                    phone = c.phone,
                    Email = c.Email,
                    BDImages = c.BDImages.Select(c => new
                    {
                        c.BDImageId,
                        c.Image,
                    }).ToList(),
                    BusinessContents = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
                    {
                        BusinessContentId = l.BusinessContentId,
                        BusinessTemplateConfigId = l.BusinessTemplateConfigId,
                        AdContentValues = l.BusinessContentValues.Select(k => new
                        {
                            BusinessContentValueId = k.BusinessContentValueId,
                            ContentValueEn = (_dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 6) ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            ContentValueAr = (_dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 6) ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,
                            FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
                            BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
                            BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,
                        })
                    })
                        .ToList(),

                }).FirstOrDefault();


                if (ClassifiedBusiness is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Business" });
                }
                classifiedBusinessVm.Views = classifiedBusinessVm.Views == null ? 0 : classifiedBusinessVm.Views + 1;
                _dbContext.SaveChanges();
                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetAllBusinessDetailsByCategoryId
        [HttpGet]
        [Route("GetAllBusinessDetailsByCategoryId")]
        public async Task<IActionResult> GetAllBusinessDetailsByCategoryId(int BusinessCategoryId, string userId, int page = 1, int pageSize = 10)
        {
            try
            {

                var BusinessCategory = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryId == BusinessCategoryId).FirstOrDefault();
                if (BusinessCategory == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });

                }
                var catList = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryParentId == BusinessCategoryId).Select(e => e.BusinessCategoryId).ToList();

                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Include(e => e.BusiniessSubscriptions).Where(c => c.IsActive == true && (c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().IsActive && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate >= DateTime.Now) && (c.BusinessCategoryId == BusinessCategoryId || catList.Contains(c.BusinessCategoryId.Value))).Include(c => c.BusinessContents).ThenInclude(c => c.BusinessContentValues).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Deliverycost = c.Deliverycost,
                    Rating = c.Rating,
                    Businessworkinghours = c.businessWorkingHours,
                    UseId = c.UseId,
                    Views = c.Views,
                    Logo = c.Logo,
                    IsFavourite = _dbContext.FavouriteBusiness.Any(o => o.ClassifiedBusinessId == c.ClassifiedBusinessId && o.UserId == userId),

                    Reviews = _dbContext.Reviews.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
                    {
                        ClassifiedBusinessId = l.ClassifiedBusinessId,
                        Name = l.Name,
                        Rating = l.Rating,
                        ReviewDate = l.ReviewDate,
                        ReviewId = l.ReviewId,
                        Title = l.Title,
                        Email = l.Email,
                    }).ToList(),
                    SubscriptionIsFinished = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault() == null ? true : _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate < DateTime.Now ? true : false,
                    BusinessContents = c.BusinessContents.Select(l => new
                    {
                        BusinessContentId = l.BusinessContentId,
                        BusinessTemplateConfigId = l.BusinessTemplateConfigId,

                        BusinessContentValues = l.BusinessContentValues.Select(k => new
                        {
                            BusinessContentValueId = k.BusinessContentValueId,
                            ContentValue = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
                            BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
                            BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,

                        }).ToList()


                    }).ToList(),

                }).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();




                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region RemoveProductFromFavourite
        #region DeleteBusiness
        [HttpDelete]
        [Route("DeleteBusiness")]
        public async Task<ActionResult> DeleteBusiness(long BusinessId)
        {
            try
            {
                var BusinessObj = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == BusinessId).FirstOrDefault();
                if (BusinessObj == null)
                {
                    return Ok(new { Status = false, Message = "Business Not Found" });
                }
                ///Delete Product CatagoryList With Its Products////
                var ProductCategoryList = _dbContext.ProductCategories.Where(e => e.ClassifiedBusinessId == BusinessId).ToList();
                if (ProductCategoryList == null)
                {
                    foreach (var ele in ProductCategoryList)
                    {
                        var productList = _dbContext.Products.Where(e => e.ProductCategoryId == ele.ProductCategoryId).ToList();
                        foreach (var item in productList)
                        {
                            var pricesList = _dbContext.ProductPrices.Where(e => e.ProductId == item.ProductId).ToList();
                            if (pricesList != null)
                            {
                                _dbContext.ProductPrices.RemoveRange(pricesList);
                            }
                            var extraList = _dbContext.ProductExtras.Where(e => e.ProductId == item.ProductId).ToList();
                            if (extraList != null)
                            {
                                _dbContext.ProductExtras.RemoveRange(extraList);
                            }
                            var AdContentList = _dbContext.ProductContents.Where(e => e.ProductId == item.ProductId).ToList();
                            var newAdContentList = AdContentList.Select(e => e.ProductContentId).ToList();
                            if (AdContentList != null)
                            {
                                var ContentValues = _dbContext.ProductContentValues.Where(e => newAdContentList.Contains(e.ProductContentId)).ToList();
                                _dbContext.ProductContentValues.RemoveRange(ContentValues);
                            }
                            _dbContext.ProductContents.RemoveRange(AdContentList);

                        }
                        _dbContext.Products.RemoveRange(productList);


                    }
                    _dbContext.ProductCategories.RemoveRange(ProductCategoryList);

                }
                ///Delete Service CatagoryList With Its Services////
                var serviceCategoryList = _dbContext.ServiceCatagories.Where(e => e.ClassifiedBusinessId == BusinessId).ToList();
                if (serviceCategoryList != null)
                {
                    foreach (var ele in serviceCategoryList)
                    {
                        var serviceList = _dbContext.Services.Where(e => e.ServiceCatagoryId == ele.ServiceCatagoryId).ToList();
                        foreach (var item in serviceList)
                        {
                            var AdContentList = _dbContext.ServiceContents.Where(e => e.ServiceContentId == item.ServiceId).ToList();
                            var newAdContentList = AdContentList.Select(e => e.ServiceContentId).ToList();
                            if (AdContentList != null)
                            {
                                var ContentValues = _dbContext.ServiceContentValues.Where(e => newAdContentList.Contains(e.ServiceContentId)).ToList();
                                _dbContext.ServiceContentValues.RemoveRange(ContentValues);
                            }
                            _dbContext.ServiceContents.RemoveRange(AdContentList);
                            var imageList = _dbContext.ServiceImages.Where(e => e.ServiceId == item.ServiceId).ToList();
                            if (imageList != null)
                            {
                                _dbContext.ServiceImages.RemoveRange(imageList);
                            }
                            _dbContext.Services.Remove(item);
                        }

                    }
                    _dbContext.ServiceCatagories.RemoveRange(serviceCategoryList);
                }




                var BusinessContentList = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == BusinessObj.ClassifiedBusinessId).ToList();
                var businesssubscription = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == BusinessObj.ClassifiedBusinessId).ToList();
                var Reviews = _dbContext.Reviews.Where(e => e.ClassifiedBusinessId == BusinessObj.ClassifiedBusinessId).ToList();
                var workingHourList = _dbContext.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == BusinessObj.ClassifiedBusinessId).ToList();

                if (BusinessContentList != null)
                {
                    var newBusinessContentList = BusinessContentList.Select(e => e.BusinessContentId).ToList();
                    if (BusinessContentList != null)
                    {
                        var ContentValues = _dbContext.BusinessContentValues.Where(e => newBusinessContentList.Contains(e.BusinessContentId)).ToList();
                        if (ContentValues != null)
                        {
                            _dbContext.BusinessContentValues.RemoveRange(ContentValues);

                        }
                    }
                    _dbContext.BusinessContents.RemoveRange(BusinessContentList);
                }
                if (businesssubscription != null)
                {
                    _dbContext.BusiniessSubscriptions.RemoveRange(businesssubscription);

                };
                if (Reviews != null)
                {
                    _dbContext.Reviews.RemoveRange(Reviews);

                };
                if (workingHourList != null)
                {
                    _dbContext.BusinessWorkingHours.RemoveRange(workingHourList);

                };
                _dbContext.ClassifiedBusiness.Remove(BusinessObj);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Business Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        #endregion

        #region GetBusinessDetailsByUserId
        [HttpGet]
        [Route("GetBusinessDetailsByUserId")]
        public async Task<IActionResult> GetBusinessDetailsByUserId(string userid)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userid);


                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });

                }
                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Include(c => c.BusinessContents).ThenInclude(c => c.BusinessContentValues).Where(c => c.UseId == user.Id).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Businessworkinghours = c.businessWorkingHours,
                    Views = c.Views,
                    SubscriptionIsFinished = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault() == null ? true : _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate < DateTime.Now ? true : false,
                    City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
                    {
                        k.CityId,
                        k.CityTlAr,
                        k.CityTlEn,

                    }).FirstOrDefault(),
                    Area = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
                    {
                        k.AreaId,
                        k.AreaTlAr,
                        k.AreaTlEn,

                    }).FirstOrDefault(),
                    Address = c.Address,
                    Logo = c.Logo,
                    phone = c.phone,
                    Email = c.Email,

                    BusinessContents = c.BusinessContents.Select(l => new
                    {
                        BusinessContentId = l.BusinessContentId,
                        BusinessTemplateConfigId = l.BusinessTemplateConfigId,

                        BusinessContentValues = l.BusinessContentValues.Select(k => new
                        {
                            BusinessContentValueId = k.BusinessContentValueId,
                            ContentValue = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
                            BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
                            BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,

                        }).ToList()


                    }).ToList(),


                }).ToList();


                if (ClassifiedBusiness is null)
                {
                    return Ok(new { Status = false, Message = "Business Not Found" });

                }


                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetFilterClassifiedBusiness
        [HttpGet]
        [Route("GetFilterClassifiedBusiness")]
        public IActionResult GetFilterClassifiedBusiness([FromBody] FilterBusinessModelVm businessFilterVm)
        {
            try
            {
                List<long> BusinessIds = new List<long>();
                var classifiedBusinessList = _dbContext.ClassifiedBusiness.Include(c => c.BusinessContents).ThenInclude(c => c.BusinessContentValues).Where(a => a.BusinessCategoryId == businessFilterVm.BusinessCategoryId).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    BusinessContents = c.BusinessContents.Select(l => new
                    {
                        BusinessContentId = l.BusinessContentId,
                        BusinessTemplateConfigId = l.BusinessTemplateConfigId,

                        BusinessContentValues = l.BusinessContentValues.Select(k => new
                        {
                            BusinessContentValueId = k.BusinessContentValueId,
                            ContentValue = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
                            BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
                            BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,
                        }).ToList()
                    }).ToList(),

                }).ToList();
                if (businessFilterVm.businessContentVMs != null)
                {
                    foreach (var item in businessFilterVm.businessContentVMs)
                    {
                        var Values = _dbContext.BusinessContentValues.Include(e => e.BusinessContent).Where(e => e.BusinessContent.BusinessTemplateConfigId == item.BusinessTemplateConfigId && e.ContentValue == item.Value).Select(e => e.ContentValue).ToList();
                        BusinessIds = _dbContext.BusinessContents.Include(e => e.BusinessContentValues).Where(e => Values.Contains(e.BusinessContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedBusinessId).ToList();
                        classifiedBusinessList = classifiedBusinessList.Where(e => BusinessIds.Contains(e.ClassifiedBusinessId)).ToList();
                    }
                }
                if (classifiedBusinessList is null)
                {
                    return NotFound();
                }
                return Ok(new { Status = true, classifiedBusinessList = classifiedBusinessList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region AddFavouriteBusiness
        [HttpPost]
        [Route("AddFavouriteBusiness")]
        public IActionResult AddFavouriteBusiness(long BusinessId, string userid)
        {
            try
            {

                var buisness = _dbContext.ClassifiedBusiness.Find(BusinessId);
                var user = _applicationDbContext.Users.Find(userid);
                if (buisness == null)
                {
                    return Ok(new { status = false, message = "Business Not Found." });

                }
                if (user == null)
                {
                    return Ok(new { status = false, message = "User Not Found." });

                }
                var favourite = _dbContext.FavouriteBusiness.Where(a => a.ClassifiedBusinessId == BusinessId && a.UserId == userid).FirstOrDefault();
                if (favourite != null)
                {
                    return Ok(new { status = false, message = "Business already added in favourite.." });
                }
                var favouriteobj = new FavouriteBusiness() { UserId = userid, ClassifiedBusinessId = BusinessId };
                _dbContext.FavouriteBusiness.Add(favouriteobj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Business Added To Favourite.." });
            }
            catch (Exception e)
            {
                return Ok(new { status = false, message = e.Message });
            }
        }
        #endregion
        #endregion
        #region GetBusinessCategoryChild
        [HttpGet]
        [Route("GetBusinessCategoryChild")]
        public async Task<ActionResult> GetBusinessCategoryChild()
        {
            try
            {
                var Data = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryParentId == null).Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
                    BusinessCategorySortOrder = c.BusinessCategorySortOrder,
                    BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
                    BusinessCategoryIsActive = c.BusinessCategoryIsActive,
                    BusinessCategoryDescAr = c.BusinessCategoryDescAr,
                    BusinessCategoryDescEn = c.BusinessCategoryDescEn,
                    BusinessCategoryParentId = c.BusinessCategoryParentId,
                    BusinessCategoryParent = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryParentId == c.BusinessCategoryId).ToList(),
                    HasChild = _dbContext.BusinessCategories.Any(x => x.BusinessCategoryParentId == c.BusinessCategoryId),

                }).ToListAsync();

                if (Data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        #endregion
        #region Product APIs
        #region GetProductChartByBussinessCategoryId
        [HttpGet]
        [Route("GetProductChartByProductTypeId")]
        public ActionResult GetProductChartByProductTypeId(int ProductTypeId)
        {
            try
            {

                var productType = _dbContext.ProductTypes.Where(c => c.ProductTypeId == ProductTypeId).FirstOrDefault();

                if (productType is null)
                {
                    return Ok(new { Status = false, Message = "Product Type Not Found" });

                }
                var config = _dbContext.ProductTemplateConfigs.Include(e => e.ProductTemplateOptions).Where(e => e.ProductTypeId == ProductTypeId).ToList();
                return Ok(new { Status = true, config });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetProductCategoryByCategoryId

        [HttpGet]
        [Route("GetProductCategoryByCategoryId")]
        public ActionResult GetProductCategoryByCategoryId(long ProductCategoryId)
        {
            try
            {

                var Category = _dbContext.ProductCategories.Where(c => c.ProductCategoryId == ProductCategoryId).Select(c => new
                {
                    ProductCategoryId = c.ProductCategoryId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    SortOrder = c.SortOrder,
                    Pic = c.Pic,
                    Isactive = c.Isactive,
                }).ToList();
                if (Category is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Category" });

                }


                return Ok(new { Status = true, Category });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }


        [HttpGet]
        [Route("GetBunsniessCategoryByBusinessId")]
        public ActionResult GetBunsniessCategoryByBusinessId(long ClassifiedBusinessId, string userId)
        {
            try
            {

                var Category = _dbContext.ProductCategories.Where(c => c.ClassifiedBusinessId == ClassifiedBusinessId).Select(c => new
                {
                    ProductCategoryId = c.ProductCategoryId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    SortOrder = c.SortOrder,
                    Pic = c.Pic,
                    Isactive = c.Isactive,
                    Products = c.Products.Select(k => new
                    {
                        ProductId = k.ProductId,
                        ProductCategoryId = k.ProductCategoryId,
                        TitleAr = k.TitleAr,
                        TitleEn = k.TitleEn,
                        Price = k.Price,
                        MainPic = k.MainPic,
                        Description = k.Description,
                        IsFavourite = _dbContext.FavouriteProducts.Any(o => o.ProductId == k.ProductId && o.UserId == userId),

                    }).ToList(),
                }).ToList();
                if (Category is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Category" });

                }


                return Ok(new { Status = true, Category });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }

        #endregion
        #region AddProductCategory
        [HttpPost]
        [Route("AddProductCategory")]
        public async Task<ActionResult> AddProductCategory([FromForm] ProductCategoryVm productCategoryVm, IFormFile productCatgoryPic)
        {
            try
            {
                var BDObj = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == productCategoryVm.BusinessId).FirstOrDefault();
                if (BDObj == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Directory Not Found" });
                }
                var productCategory = new ProductCategory()
                {
                    Isactive = productCategoryVm.Isactive,
                    SortOrder = productCategoryVm.SortOrder,
                    TitleAr = productCategoryVm.TitleAr,
                    TitleEn = productCategoryVm.TitleEn,
                    ClassifiedBusinessId = productCategoryVm.BusinessId,
                };
                if (productCatgoryPic != null)
                {
                    string folder = "Images/ProductCategory/";
                    productCategory.Pic = UploadImage(folder, productCatgoryPic);
                }
                _dbContext.ProductCategories.Add(productCategory);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Category Added Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        #endregion
        #region EditProductCategory
        [HttpPut]
        [Route("EditProductCategory")]
        public async Task<ActionResult> EditProductCategory([FromForm] EditProductCategoryVm productCategoryVm, IFormFile productCatgoryPic)
        {
            try
            {
                var ProductCategoryObj = _dbContext.ProductCategories.Where(e => e.ProductCategoryId == productCategoryVm.ProductCategoryId).FirstOrDefault();
                if (ProductCategoryObj == null)
                {
                    return Ok(new { Status = false, Message = "Product Category Not Found" });
                }


                ProductCategoryObj.Isactive = productCategoryVm.Isactive;
                ProductCategoryObj.SortOrder = productCategoryVm.SortOrder;
                ProductCategoryObj.TitleAr = productCategoryVm.TitleAr;
                ProductCategoryObj.TitleEn = productCategoryVm.TitleEn;


                if (productCatgoryPic != null)
                {
                    string folder = "Images/ProductCategory/";
                    ProductCategoryObj.Pic = UploadImage(folder, productCatgoryPic);
                }
                _dbContext.Attach(ProductCategoryObj).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Category Edited Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        #endregion
        #region DeleteProductCategory
        [HttpDelete]
        [Route("DeleteProductCategory")]
        public async Task<ActionResult> DeleteProductCategory(long ProductCategoryId)
        {
            try
            {
                var ProductCategoryObj = _dbContext.ProductCategories.Where(e => e.ProductCategoryId == ProductCategoryId).FirstOrDefault();
                if (ProductCategoryObj == null)
                {
                    return Ok(new { Status = false, Message = "Product Category Not Found" });
                }
                var productList = _dbContext.Products.Where(e => e.ProductCategoryId == ProductCategoryObj.ProductCategoryId).ToList();


                foreach (var item in productList)
                {
                    var pricesList = _dbContext.ProductPrices.Where(e => e.ProductId == item.ProductId).ToList();
                    if (pricesList != null)
                    {
                        _dbContext.ProductPrices.RemoveRange(pricesList);
                    }
                    var extraList = _dbContext.ProductExtras.Where(e => e.ProductId == item.ProductId).ToList();
                    if (extraList != null)
                    {
                        _dbContext.ProductExtras.RemoveRange(extraList);
                    }
                    var AdContentList = _dbContext.ProductContents.Where(e => e.ProductId == item.ProductId).ToList();
                    var newAdContentList = AdContentList.Select(e => e.ProductContentId).ToList();
                    if (AdContentList != null)
                    {
                        var ContentValues = _dbContext.ProductContentValues.Where(e => newAdContentList.Contains(e.ProductContentId)).ToList();
                        _dbContext.ProductContentValues.RemoveRange(ContentValues);
                    }
                    _dbContext.ProductContents.RemoveRange(AdContentList);
                    _dbContext.Products.Remove(item);
                }
                _dbContext.ProductCategories.Remove(ProductCategoryObj);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Product Category Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        #endregion
        #region DeleteProduct
        [HttpDelete]
        [Route("DeleteProduct")]
        public async Task<ActionResult> DeleteProduct(long ProductId)
        {
            try
            {
                var ProductObj = _dbContext.Products.Where(e => e.ProductId == ProductId).FirstOrDefault();
                if (ProductObj == null)
                {
                    return Ok(new { Status = false, Message = "Product Not Found" });
                }

                var pricesList = _dbContext.ProductPrices.Where(e => e.ProductId == ProductObj.ProductId).ToList();
                if (pricesList != null)
                {
                    _dbContext.ProductPrices.RemoveRange(pricesList);
                }
                var extraList = _dbContext.ProductExtras.Where(e => e.ProductId == ProductObj.ProductId).ToList();
                if (extraList != null)
                {
                    _dbContext.ProductExtras.RemoveRange(extraList);
                }
                var AdContentList = _dbContext.ProductContents.Where(e => e.ProductId == ProductObj.ProductId).ToList();
                var newAdContentList = AdContentList.Select(e => e.ProductContentId).ToList();
                if (AdContentList != null)
                {
                    var ContentValues = _dbContext.ProductContentValues.Where(e => newAdContentList.Contains(e.ProductContentId)).ToList();
                    _dbContext.ProductContentValues.RemoveRange(ContentValues);
                }
                _dbContext.ProductContents.RemoveRange(AdContentList);
                _dbContext.Products.Remove(ProductObj);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Product Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        #endregion

        #region GetAllProductCategoryByBussinessId
        [HttpGet]
        [Route("GetAllProductCategoryByBussinessId")]
        public ActionResult GetAllProductCategoryByBussinessId(long BussinessId)
        {
            try
            {

                var Category = _dbContext.ProductCategories.Include(e => e.Products).Where(c => c.ClassifiedBusinessId == BussinessId).Select(c => new
                {
                    ProductCategoryId = c.ProductCategoryId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    SortOrder = c.SortOrder,
                    Pic = c.Pic,
                    Isactive = c.Isactive,
                    Products = c.Products
                }).ToList();
                if (Category is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Category" });

                }


                return Ok(new { Status = true, Category });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetAllProductCategoryByBussinessId
        [HttpGet]
        [Route("GetAllProductType")]
        public ActionResult GetAllProductType()
        {
            try
            {

                var ProductTypes = _dbContext.ProductTypes.Select(c => new
                {
                    ProductTypeId = c.ProductTypeId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Pic = c.Pic,



                }).ToList();
                if (ProductTypes is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Product Types" });

                }


                return Ok(new { Status = true, ProductTypes = ProductTypes });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region AddProduct

        [HttpPost]
        [Route("AddProduct")]
        public async Task<ActionResult> AddProduct([FromForm] ProductVM ProductVM, List<ProductMediaVm> mediaVms, IFormFile mainPic, IFormFile Reel, List<ProductExtraVm> productExtraVms, List<ProductPriceVm> productPriceVms, IFormFileCollection ProductImages)
        {
            try
            {

                var ProductCat = _dbContext.ProductCategories.Find(ProductVM.ProductCategoryId);
                if (ProductCat == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });
                }
                var ProductType = _dbContext.ProductTypes.Find(ProductVM.ProductTypeId);
                if (ProductType == null)
                {
                    return Ok(new { Status = false, Message = "Product Type Not Found" });
                }

                Product product = new Product()
                {
                    IsActive = ProductVM.IsActive,
                    ProductCategoryId = ProductVM.ProductCategoryId,
                    Price = ProductVM.Price,
                    //ProductName = ProductVM.ProductName,
                    TitleAr = ProductVM.TitleAr,
                    TitleEn = ProductVM.TitleEn,
                    Description = ProductVM.Description,
                    SortOrder = ProductVM.SortOrder,
                    IsFixedPrice = ProductVM.IsFixedPrice,
                    ProductTypeId = ProductVM.ProductTypeId,
                    //Location = ProductVM.Location,



                };
                if (mainPic != null)
                {
                    string folder = "Images/ProductMedia/";
                    product.MainPic = UploadImage(folder, mainPic);

                }
                if (Reel != null)
                {
                    string folder = "Images/ProductMedia/";
                    product.Reel = UploadImage(folder, Reel);

                }
                if (ProductImages != null)
                {
                    List<ProductImage> prodImages = new List<ProductImage>();
                    foreach (var item in ProductImages)
                    {
                        var ProductImageObj = new ProductImage();
                        ProductImageObj.Image = UploadImage("Images/ProductMedia/", item);
                        prodImages.Add(ProductImageObj);

                    }
                    product.ProductImages = prodImages;

                }

                if (productExtraVms != null)
                {
                    List<ProductExtra> productExtras = new List<ProductExtra>();
                    foreach (var item in productExtraVms)
                    {
                        ProductExtra productExtraObj = new ProductExtra()
                        {
                            ExtraTitleAr = item.ExtraTitleAr,
                            ExtraTitleEn = item.ExtraTitleEn,
                            ExtraDes = item.ExtraDes,
                            Price = item.Price
                        };
                        productExtras.Add(productExtraObj);

                    }
                    product.ProductExtras = productExtras;
                }
                if (productPriceVms != null)
                {
                    List<ProductPrice> productPriceses = new List<ProductPrice>();
                    foreach (var item in productPriceVms)
                    {
                        ProductPrice productPriceObj = new ProductPrice()
                        {
                            ProductPriceTilteAr = item.ProductPriceTilteAr,
                            ProductPriceTilteEn = item.ProductPriceTilteEn,
                            ProductPriceDes = item.ProductPriceDes,
                            Price = item.Price
                        };
                        productPriceses.Add(productPriceObj);
                    }
                    product.ProductPrices = productPriceses;
                }

                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();
                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var item in mediaVms)
                        {
                            var contentListMediaValue = new List<ProductContentValue>();
                            for (int i = 0; i < item.Media?.Count(); i++)
                            {
                                if (item.Media[i] != null)
                                {
                                    string folder = "Images/ProductMedia/";
                                    var contentObj = new ProductContentValue();
                                    contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                    contentListMediaValue.Add(contentObj);
                                }

                            }
                            var ContentValueObj = new ProductContent()
                            {
                                ProductId = product.ProductId,
                                ProductTemplateConfigId = item.ProductTemplateConfigId,
                                ProductContentValues = contentListMediaValue

                            };
                            _dbContext.ProductContents.Add(ContentValueObj);
                            _dbContext.SaveChanges();

                        }
                    }
                }
                foreach (var item in ProductVM.ProductContentVMS)
                {
                    var contentList = new List<ProductContentValue>();

                    foreach (var value in item.Values)
                    {

                        var contentObj = new ProductContentValue()
                        {
                            ContentValue = value,
                        };
                        contentList.Add(contentObj);
                    }
                    var ContentValue = new ProductContent()
                    {
                        ProductId = product.ProductId,
                        ProductTemplateConfigId = item.ProductTemplateConfigId,
                        ProductContentValues = contentList
                    };
                    _dbContext.ProductContents.Add(ContentValue);
                    _dbContext.SaveChanges();
                }
                return Ok(new { Status = true, Message = "Product Added Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetProductDetailsById
        [HttpGet]
        [Route("GetProductDetailsById")]
        public async Task<IActionResult> GetProductDetailsById(int ProductId, string userId)
        {
            try
            {


                var Product = _dbContext.Products.Where(c => c.ProductId == ProductId).Include(c => c.ProductExtras).Include(c => c.ProductPrices).Include(c => c.ProductCategory).ThenInclude(e => e.ClassifiedBusiness).Select(c => new
                {
                    ProductId = c.ProductId,
                    ProductCategoryId = c.ProductCategoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    //ProductName = c.ProductName,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    IsFixedPrice = c.IsFixedPrice,
                    ProductTypeId = c.ProductTypeId,
                    MainPic = c.MainPic,
                    ProductExtras = c.ProductExtras,
                    ProductPrices = c.ProductPrices,
                    Reel = c.Reel,
                    ProductCategoryTitleAr = c.ProductCategory.TitleAr,
                    ProductCategoryTitleEn = c.ProductCategory.TitleEn,
                    UseId = c.ProductCategory.ClassifiedBusiness.UseId,
                    ServiceImages = c.ProductImages.Select(i => new
                    {
                        ProductImageId = i.ProductImageId,
                        Image = i.Image
                    }).ToList(),

                    IsFavourite = _dbContext.FavouriteProducts.Any(o => o.ProductId == ProductId && o.UserId == userId),


                    ProductContents = _dbContext.ProductContents.Where(e => e.ProductId == c.ProductId).Select(l => new
                    {
                        ProductContentId = l.ProductContentId,
                        ProductTemplateConfigId = l.ProductTemplateConfigId,
                        ProductContentValues = l.ProductContentValues.Select(k => new
                        {
                            ProductContentValueId = k.ProductContentValueId,
                            ContentValueEn = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _dbContext.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            ContentValueAr = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _dbContext.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,
                            FieldTypeId = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ProductTemplateFieldCaptionAr = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionAr,
                            ProductTemplateFieldCaptionEn = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionEn,
                        })
                    })
                          .ToList(),

                }).FirstOrDefault();


                if (Product is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Product" });
                }

                return Ok(new { Status = true, Product = Product });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion

        #region GetAllProductDetailsByCategoryId
        [HttpGet]
        [Route("GetAllProductDetailsByCategoryId")]
        public IActionResult GetAllProductDetailsByCategoryId(int ProductCategoryId)
        {
            try
            {
                var ProductCategory = _dbContext.ProductCategories.Where(e => e.ProductCategoryId == ProductCategoryId).FirstOrDefault();
                if (ProductCategory == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });

                }
                var Products = _dbContext.Products.Include(c => c.ProductExtras).Include(c => c.ProductPrices).Where(c => c.ProductCategoryId == ProductCategoryId).Include(c => c.ProductContents).ThenInclude(c => c.ProductContentValues).Select(c => new
                {
                    ProductId = c.ProductId,
                    ProductCategoryId = c.ProductCategoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    //ProductName = c.ProductName,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    IsFixedPrice = c.IsFixedPrice,
                    ProductTypeId = c.ProductTypeId,
                    MainPic = c.MainPic,
                    ProductExtras = c.ProductExtras,
                    ProductPrices = c.ProductPrices,

                    ProductContents = _dbContext.ProductContents.Where(e => e.ProductId == c.ProductId).Select(l => new
                    {
                        ProductContentId = l.ProductContentId,
                        ProductTemplateConfigId = l.ProductTemplateConfigId,
                        ProductContentValues = l.ProductContentValues.Select(k => new
                        {
                            ProductContentValueId = k.ProductContentValueId,
                            ContentValue = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ProductTemplateFieldCaptionAr = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionAr,
                            ProductTemplateFieldCaptionEn = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionEn,
                        })
                    })
                        .ToList(),

                }).ToListAsync();




                return Ok(new { Status = true, Products = Products });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion

        //#region GetFilterProduct
        //[HttpGet]
        //[Route("GetFilterProduct")]
        //public IActionResult GetFilterProduct([FromBody] FilterProductModelVm productFilterVm)
        //{
        //    try
        //    {
        //        List<long> ProductIds = new List<long>();
        //        var ProductList = _dbContext.Products.Include(c => c.ProductExtras).Include(c => c.ProductPrices).Include(c => c.ProductContents).ThenInclude(c => c.ProductContentValues).Where(a => a.ProductCategoryId == productFilterVm.ProductCategoryId).Select(c => new
        //        {
        //            ProductId = c.ProductId,
        //            ProductCategoryId = c.ProductCategoryId,
        //            IsActive = c.IsActive,
        //            Price = c.Price,
        //            SortOrder = c.SortOrder,
        //            ProductName = c.ProductName,
        //            IsFixedPrice = c.IsFixedPrice,
        //            ProductTypeId = c.ProductTypeId,
        //            MainPic = c.MainPic,
        //            ProductExtras = c.ProductExtras,
        //            ProductPrices = c.ProductPrices,
        //            ProductContents = c.ProductContents.Select(l => new
        //            {
        //                ProductContentId = l.ProductContentId,
        //                ProductTemplateConfigId = l.ProductTemplateConfigId,

        //                ProductContentValues = l.ProductContentValues.Select(k => new
        //                {
        //                    ProductContentValueId = k.ProductContentValueId,
        //                    ContentValue = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    ProductTemplateFieldCaptionAr = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionAr,
        //                    ProductTemplateFieldCaptionEn = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionEn,
        //                }).ToList()
        //            }).ToList(),

        //        }).ToList();
        //        if (productFilterVm.productContentVMs != null)
        //        {
        //            foreach (var item in productFilterVm.productContentVMs)
        //            {
        //                var Values = _dbContext.ProductContentValues.Include(e => e.ProductContent).Where(e => e.ProductContent.ProductTemplateConfigId == item.ProductTemplateConfigId && e.ContentValue == item.Value).Select(e => e.ContentValue).ToList();
        //                ProductIds = _dbContext.ProductContents.Include(e => e.ProductContentValues).Where(e => Values.Contains(e.ProductContentValues.FirstOrDefault().ContentValue)).Select(e => e.ProductId).ToList();
        //                ProductList = ProductList.Where(e => ProductIds.Contains(e.ProductId)).ToList();
        //            }
        //        }
        //        if (ProductList is null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(new { Status = true, ProductList = ProductList });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        //#endregion
        #endregion
        #region Wallet APIs
        #region GetAllWallets
        [HttpGet]
        [Route("GetAllWallets")]
        public ActionResult GetAllWallets()
        {
            try
            {

                var walletList = _dbContext.Wallets.Where(c => c.IsActive == true).OrderBy(e => e.SortOrder).Select(c => new
                {
                    Price = c.Price,
                    WalletId = c.WalletId,
                    WalletTitleAr = c.WalletTitleAr,
                    WalletTitleEn = c.WalletTitleEn,
                    NumberOfClassifed = c.NumberOfClassifed,

                }).ToList();
                if (walletList is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Wallet" });

                }


                return Ok(new { Status = true, walletList = walletList });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region GetWalletById
        [HttpGet]
        [Route("GetWalletById")]
        public ActionResult GetWalletById(int WalletId)
        {
            try
            {

                var wallet = _dbContext.Wallets.Where(c => c.WalletId == WalletId).Select(c => new
                {
                    Price = c.Price,
                    WalletId = c.WalletId,
                    WalletTitleAr = c.WalletTitleAr,
                    WalletTitleEn = c.WalletTitleEn,
                    NumberOfClassifed = c.NumberOfClassifed,

                }).FirstOrDefault();
                if (wallet is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Wallet" });

                }


                return Ok(new { Status = true, wallet = wallet });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #region SubscribeToWallet
        [HttpPost]
        [Route("SubscribeToWallet")]
        public async Task<ActionResult> SubscribeToWallet([FromForm] WalletCheckOutVm walletCheckOut)
        {
            var wallet = _dbContext.Wallets.Find(walletCheckOut.walletId);
            if (wallet == null)
            {
                return Ok(new { Status = false, Message = "wallet Not Found" });
            }
            var user = await _userManager.FindByIdAsync(walletCheckOut.userId);
            if (user == null)
            {
                return Ok(new { Status = false, Message = "User Not Found" });
            }
            var PM = _dbContext.PaymentMehods.Find(walletCheckOut.paymentMethodId);
            if (PM == null)
            {
                return Ok(new { Status = false, Message = "PaymentMethod Not Found" });
            }
            var walletSubscription = new WalletSubscription()
            {
                IsPaid = false,
                PaymentMethodId = walletCheckOut.paymentMethodId,
                SubscriptionDate = DateTime.Now,
                UserId = walletCheckOut.userId,
                WalletId = walletCheckOut.walletId,
                PaymentID = " ",

            };
            _dbContext.WalletSubscriptions.Add(walletSubscription);
            _dbContext.SaveChanges();
            if (walletCheckOut.paymentMethodId == 1)
            {
                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];
                if (Fattorahstatus) // fattorah live
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = wallet.Price,
                        CallBackUrl = "https://albaheth.me/FattorahWalletSuccess",
                        ErrorUrl = "https://albaheth.me/FattorahWalletError",

                        UserDefinedField = walletSubscription.WalletSubscriptionId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://api.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        return Ok(new { Status = true, paymentUrl = InvoiceRes.InvoiceURL });


                    }
                    else
                    {

                        _dbContext.WalletSubscriptions.Remove(walletSubscription);

                        return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });


                    }
                }
                else               //fattorah test
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = wallet.Price,
                        CallBackUrl = "https://albaheth.me/FattorahWalletSuccess",
                        ErrorUrl = "https://albaheth.me/FattorahWalletError",

                        UserDefinedField = walletSubscription.WalletSubscriptionId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        return Ok(new { Status = true, paymentUrl = InvoiceRes.InvoiceURL });



                    }
                    else
                    {

                        _dbContext.WalletSubscriptions.Remove(walletSubscription);

                        return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });


                    }
                }




            }
            return Ok();
        }
        #endregion
        #endregion
        #region Product Favourite APIS
        #region AddProductToFavourite
        [HttpPost]
        [Route("AddProductToFavourite")]
        public IActionResult AddProductToFavourite(long ProductId, string userid)
        {
            try
            {

                var Product = _dbContext.Products.Find(ProductId);
                var user = _applicationDbContext.Users.Find(userid);
                if (Product == null)
                {
                    return Ok(new { Status = false, Message = "Product Not Found." });

                }
                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found." });

                }
                var favourite = _dbContext.FavouriteProducts.Where(a => a.ProductId == ProductId && a.UserId == userid).FirstOrDefault();
                if (favourite != null)
                {
                    return Ok(new { Status = false, Message = "Product already added in favourite.." });
                }
                var favouriteobj = new FavouriteProduct() { UserId = userid, ProductId = ProductId };
                _dbContext.FavouriteProducts.Add(favouriteobj);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Added To Favourite.." });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, Message = e.Message });
            }
        }
        #endregion
        #region RemoveProductFromFavourite
        [HttpDelete]
        [Route("RemoveProductFromFavourite")]
        public IActionResult RemoveProductFromFavourite(long ProductId, string userid)
        {
            try
            {
                var favourite = _dbContext.FavouriteProducts.Where(e => e.ProductId == ProductId && e.UserId == userid).FirstOrDefault();
                if (favourite == null)
                {
                    return Ok(new { status = false, message = "Favourite Product Not Found.." });
                }
                _dbContext.FavouriteProducts.Remove(favourite);
                _dbContext.SaveChanges();
                return Ok(new { status = true, message = "Prdouct Deleted Successfully From Favourite.." });

            }
            catch (Exception e)
            {

                return Ok(new { status = false, message = e.Message });

            }
        }
        #endregion
        #endregion
        #endregion
        #region BusinessPlanSubscriptions
        #region 
        #region GetAllProductByBusinessId
        [HttpGet]
        [Route("GetAllProductByBusinessId")]
        public async Task<IActionResult> GetAllProductByBusinessId(int BusinessId)
        {
            try
            {
                var product = _dbContext.Products.Include(e => e.ProductCategory).Where(e => e.ProductCategory.ClassifiedBusinessId == BusinessId && e.IsActive == true).FirstOrDefault();
                if (product == null)
                {
                    return Ok(new { Status = false, Message = "Product Not Found" });

                }
                var ProductList = _dbContext.Products.Include(e => e.ProductCategory).Where(e => e.ProductCategory.ClassifiedBusinessId == BusinessId && e.IsActive == true).Select(c => new
                {
                    ProductCategoryId = c.ProductCategoryId,
                    ProductId = c.ProductId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    Price = c.Price,
                    MainPic = c.MainPic,

                }).ToListAsync();

                return Ok(new { Status = true, ProductList = ProductList });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        #endregion
        #region AddSubscription
        [HttpPost]
        [Route("AddSubscription")]
        public async Task<IActionResult> AddSubscription([FromBody] SubscriptionVM subscriptionVM)
        {
            try
            {
                var BusinessExists = _dbContext.ClassifiedBusiness.Find(subscriptionVM.ClassifiedBusinessId);
                if (BusinessExists == null)
                    return Ok(new { status = false, Message = "Business Not exists!" });
                var plan = _dbContext.BussinessPlans.Find(subscriptionVM.PlanId);

                if (plan == null)
                {

                    return Ok(new { status = false, Message = " Please Select Plan and try again." });

                }

                var userSubscription = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == subscriptionVM.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault();

                if (userSubscription != null)
                {

                    if (userSubscription.EndDate > DateTime.Now)
                    {
                        return Ok(new { status = false, Message = "Business Aleardy Subscriped In Plan" });

                    }
                }
                var subscriptionObj = new BusiniessSubscription()
                {
                    ClassifiedBusinessId = subscriptionVM.ClassifiedBusinessId,
                    BussinessPlanId = subscriptionVM.PlanId,
                    PaymentMethodId = subscriptionVM.PaymentMethodId,
                    Price = plan.Price,
                    IsActive = false,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(plan.DurationInMonth.Value)

                };
                _dbContext.BusiniessSubscriptions.Add(subscriptionObj);
                _dbContext.SaveChanges();


                if (subscriptionVM.PaymentMethodId == 1)
                {
                    bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                    var TestToken = _configuration["TestToken"];
                    var LiveToken = _configuration["LiveToken"];
                    if (Fattorahstatus) // fattorah live
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = BusinessExists.Title,
                            NotificationOption = "LNK",
                            InvoiceValue = subscriptionObj.Price,
                            CallBackUrl = "https://albaheth.me/FattorahBusinessPlantSuccess",
                            ErrorUrl = "https://albaheth.me/FattorahBusinessPlanFalied",
                            UserDefinedField = subscriptionObj.BusiniessSubscriptionId
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Ok(new { status = true, Message = "Business Subscription successfully!", paymentUrl = InvoiceRes.InvoiceURL, Subscription = subscriptionObj });



                        }
                        else
                        {

                            _dbContext.BusiniessSubscriptions.Remove(subscriptionObj);

                            return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });
                        }
                    }
                    else               //fattorah test
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = BusinessExists.Title,
                            NotificationOption = "LNK",
                            InvoiceValue = subscriptionObj.Price,
                            CallBackUrl = "https://albaheth.me/FattorahBusinessPlantSuccess",
                            ErrorUrl = "https://albaheth.me/FattorahBusinessPlanFalied",
                            UserDefinedField = subscriptionObj.BusiniessSubscriptionId
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Ok(new { status = true, Message = "Business Subscription successfully!", paymentUrl = InvoiceRes.InvoiceURL, Subscription = subscriptionObj });



                        }
                        else
                        {

                            _dbContext.BusiniessSubscriptions.Remove(subscriptionObj);

                            return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });
                        }
                    }



                }

            }
            catch (Exception e)
            {

                return Ok(new { status = false, Message = e.Message });
            }
            return Ok();

        }
        #endregion
        #region GetAllPlanList
        [HttpGet]
        [Route("GetPlansList")]
        public IActionResult GetPlansList()
        {
            try
            {
                var list = _dbContext.BussinessPlans.Where(c => c.IsActive != false && c.IsActive != null).ToList();
                return Ok(new { list });
            }
            catch (Exception)
            {
                return Ok(new { status = false, message = "Something went wrong" });
            }

        }
        #endregion

        #region GetUsersPlanSubscriptions
        [HttpGet]
        [Route("GetUserSubscribedPlans")]
        public async Task<IActionResult> GetUserSubscribedPlans([FromQuery] int BusinessId)
        {
            try
            {

                var PlanSubsciptionsList = await _dbContext.BusiniessSubscriptions.Include(e => e.BussinessPlan).Where(c => c.ClassifiedBusinessId == BusinessId && c.IsActive == true).Select(c => new
                {
                    BussinessPlanId = c.BussinessPlanId,
                    BusiniessSubscriptionId = c.BusiniessSubscriptionId,
                    Price = c.BussinessPlan.Price,
                    DurationInMonth = c.BussinessPlan.DurationInMonth,
                    PlanTlAr = c.BussinessPlan.PlanTlAr,
                    PlanTlEn = c.BussinessPlan.PlanTlEn,

                }).ToListAsync();
                return Ok(new { Status = true, PlanSubsciptionsList });
            }
            catch (Exception)
            {
                return Ok(new { Status = false, message = "Something went wrong" });
            }

        }
        #endregion
        #endregion
        #region Manage Product Orders
        #region AddCustomerAddress
        [HttpPost]
        [Route("AddCustomerAddress")]
        public async Task<IActionResult> AddCustomerAddress([FromForm] CustomerAddressVM customerAddressVM)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(customerAddressVM.UserId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }

                var customerAddress = new CustomerAddress()
                {

                    UserId = customerAddressVM.UserId,
                    Adress = customerAddressVM.Adress,
                    Governorate = customerAddressVM.Governorate,
                    Area = customerAddressVM.Area,
                    Piece = customerAddressVM.Piece,
                    Avenue = customerAddressVM.Avenue,
                    Street = customerAddressVM.Street,
                    Building = customerAddressVM.Building,
                    Floor = customerAddressVM.Floor,
                    ApartmentNumber = customerAddressVM.ApartmentNumber,
                    Lat = customerAddressVM.Lat,
                    Lng = customerAddressVM.Lng,
                };
                _dbContext.CustomerAddresses.Add(customerAddress);
                _dbContext.SaveChanges();
                return Ok(new { status = true, Message = "Address Added Successfully" });

            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region DeleteCustomerAddress
        [HttpDelete]
        [Route("DeleteCustomerAddress")]
        public IActionResult DeleteCustomerAddress(int CustomerAddressId)
        {
            try
            {
                var customerAddressObj = _dbContext.CustomerAddresses.Where(e => e.CustomerAddressId == CustomerAddressId).FirstOrDefault();
                if (customerAddressObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                }

                _dbContext.CustomerAddresses.Remove(customerAddressObj);
                _dbContext.SaveChanges();
                return Ok(new { status = true, Message = "Address Deleted Successfully" });

            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion

        #region EditCustomerAddress
        [HttpPut]
        [Route("EditCustomerAddress")]
        public async Task<IActionResult> EditCustomerAddress([FromBody] EditCustomerAddressVM editCustomerAddressVM)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(editCustomerAddressVM.UserId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }

                CustomerAddress customerAddress = new CustomerAddress()
                {
                    CustomerAddressId = editCustomerAddressVM.CustomerAddressId,
                    UserId = editCustomerAddressVM.UserId,
                    Adress = editCustomerAddressVM.Adress,
                    Governorate = editCustomerAddressVM.Governorate,
                    Area = editCustomerAddressVM.Area,
                    Piece = editCustomerAddressVM.Piece,
                    Avenue = editCustomerAddressVM.Avenue,
                    Street = editCustomerAddressVM.Street,
                    Building = editCustomerAddressVM.Building,
                    Floor = editCustomerAddressVM.Floor,
                    ApartmentNumber = editCustomerAddressVM.ApartmentNumber,
                    Lat = editCustomerAddressVM.Lat,
                    Lng = editCustomerAddressVM.Lng,
                };
                _dbContext.Attach(customerAddress).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(new { status = true, Message = "Address Edited Successfully", CustomerAddress = customerAddress });




            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region AddProductToCart
        //[HttpPost]
        //[Route("AddProductToCart")]
        //public async Task<IActionResult> AddProductToCart(ShoppingCartVM shoppingCartVM)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(shoppingCartVM.UserId);
        //        if (user == null)
        //        {
        //            return Ok(new { Status = false, message = "User Not Found" });

        //        }
        //        var itemObj = _dbContext.Products.FirstOrDefault(c => c.ProductId == shoppingCartVM.ProductId);

        //        if (itemObj == null)
        //        {
        //            return Ok(new { status = false, message = "Product  Object Not Found" });

        //        }



        //        var itemAlreadyExistInCustomerCart =
        //            _dbContext.ShoppingCarts.FirstOrDefault(s =>
        //            s.ProductId == shoppingCartVM.ProductId &&
        //            s.UserId == shoppingCartVM.UserId);

        //        var shopDeliveryCost = _dbContext.Products.Include(i => i.ProductCategory.ClassifiedBusiness).FirstOrDefault(i => i.ProductId == shoppingCartVM.ProductId).ProductCategory.ClassifiedBusiness.Deliverycost;

        //        if (shopDeliveryCost == null)
        //        {
        //            shopDeliveryCost = 0;
        //        }
        //        ShoppingCart shoppingItemObj = new ShoppingCart();



        //        if (itemAlreadyExistInCustomerCart != null)
        //        {
        //            if (shoppingCartVM.ProductPriceId == itemAlreadyExistInCustomerCart.ProductPriceId && shoppingCartVM.ProductPrice == itemAlreadyExistInCustomerCart.ItemPrice && shoppingCartVM.ShopingCartProductExtraFeatures == itemAlreadyExistInCustomerCart.ShopingCartProductExtraFeatures)
        //            {
        //                itemAlreadyExistInCustomerCart.ProductQty += shoppingCartVM.ProductQuantity;
        //                itemAlreadyExistInCustomerCart.ProductTotal += shoppingCartVM.ProductTotal;
        //                itemAlreadyExistInCustomerCart.ItemPrice = shoppingCartVM.ProductPrice;
        //                itemAlreadyExistInCustomerCart.ProductPriceId = shoppingCartVM.ProductPriceId;
        //                itemAlreadyExistInCustomerCart.Deliverycost = shopDeliveryCost;
        //                _dbContext.Attach(itemAlreadyExistInCustomerCart).State = EntityState.Modified;
        //                _dbContext.SaveChanges();

        //                if (shoppingCartVM.ShopingCartProductExtraFeatures != null)
        //                {
        //                    foreach (var shopcartVm in shoppingCartVM.ShopingCartProductExtraFeatures.ToList())
        //                    {
        //                        var shopcart = new ShopingCartProductExtraFeatures()
        //                        {

        //                            Price = shopcartVm.Price,
        //                            ProductId = shopcartVm.ProductId,
        //                            ProductExtraId = shopcartVm.ProductExtraId,
        //                            ShoppingCartId = itemAlreadyExistInCustomerCart.ShoppingCartId
        //                        };
        //                        _dbContext.ShopingCartProductExtraFeatures.Add(shopcart);
        //                    }

        //                    _dbContext.SaveChanges();
        //                }

        //                return Ok(new { Status = true, message = "Item Succesfuly Added" });

        //            }

        //        }


        //        shoppingItemObj = new ShoppingCart
        //        {
        //            UserId = shoppingCartVM.UserId,
        //            ProductId = shoppingCartVM.ProductId,
        //            ItemPrice = shoppingCartVM.ProductPrice,
        //            ProductQty = shoppingCartVM.ProductQuantity,
        //            ProductTotal = shoppingCartVM.ProductTotal,
        //            Deliverycost = shopDeliveryCost,
        //            ProductPriceId = shoppingCartVM.ProductPriceId,

        //        };
        //        _dbContext.ShoppingCarts.Add(shoppingItemObj);
        //        _dbContext.SaveChanges();
        //        if (shoppingCartVM.ShopingCartProductExtraFeatures != null)
        //        {
        //            foreach (var shopcartVm in shoppingCartVM.ShopingCartProductExtraFeatures)
        //            {
        //                var shopcart = new ShopingCartProductExtraFeatures()
        //                {
        //                    Price = shopcartVm.Price,
        //                    ProductId = shopcartVm.ProductId,
        //                    ProductExtraId = shopcartVm.ProductExtraId,
        //                    ShoppingCartId = shoppingItemObj.ShoppingCartId
        //                };
        //                _dbContext.ShopingCartProductExtraFeatures.Add(shopcart);
        //                _dbContext.SaveChanges();
        //            }
        //        }

        //        return Ok(new { Status = true, message = "Item Succesfuly Added", Obj = shoppingItemObj });

        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { status = false, message = ex.Message });

        //    }

        //}
        [HttpPost]
        [Route("AddProductToCart")]
        public async Task<IActionResult> AddProductToCart(ShoppingCartVM shoppingCartVM)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(shoppingCartVM.UserId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var itemObj = _dbContext.Products.FirstOrDefault(c => c.ProductId == shoppingCartVM.ProductId);

                if (itemObj == null)
                {
                    return Ok(new { status = false, message = "Product  Object Not Found" });

                }
                //if (shoppingCartVM.ShopingCartProductExtraFeatures == null)
                //{
                //    return Ok(new { status = false, message = "Extra Is Null" });

                //}



                var itemAlreadyExistInCustomerCart =
                    _dbContext.ShoppingCarts.FirstOrDefault(s =>
                    s.ProductId == shoppingCartVM.ProductId &&
                    s.UserId == shoppingCartVM.UserId);

                var shopDeliveryCost = _dbContext.Products.Include(i => i.ProductCategory.ClassifiedBusiness).FirstOrDefault(i => i.ProductId == shoppingCartVM.ProductId).ProductCategory.ClassifiedBusiness.Deliverycost;

                if (shopDeliveryCost == null)
                {
                    shopDeliveryCost = 0;
                }
                ShoppingCart shoppingItemObj = new ShoppingCart();



                if (itemAlreadyExistInCustomerCart != null)
                {
                    // if (shoppingCartVM.ProductPriceId == itemAlreadyExistInCustomerCart.ProductPriceId && shoppingCartVM.ProductPrice == itemAlreadyExistInCustomerCart.ItemPrice && shoppingCartVM.ShopingCartProductExtraFeatures == itemAlreadyExistInCustomerCart.ShopingCartProductExtraFeatures)
                    //{
                    itemAlreadyExistInCustomerCart.ProductQty += shoppingCartVM.ProductQuantity;
                    itemAlreadyExistInCustomerCart.ProductTotal += shoppingCartVM.ProductTotal;
                    itemAlreadyExistInCustomerCart.ItemPrice = shoppingCartVM.ProductPrice;
                    itemAlreadyExistInCustomerCart.ProductPriceId = shoppingCartVM.ProductPriceId;
                    itemAlreadyExistInCustomerCart.Deliverycost = shopDeliveryCost;
                    _dbContext.Attach(itemAlreadyExistInCustomerCart).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    if (shoppingCartVM.ShopingCartProductExtraFeatures != null)
                    {
                        List<ShopingCartProductExtraFeatures> ShopingCartProductExtraFeaturesNewList = new List<ShopingCartProductExtraFeatures>();

                        foreach (var shopcartVm in shoppingCartVM.ShopingCartProductExtraFeatures.ToList())
                        {
                            var shopcart = new ShopingCartProductExtraFeatures()
                            {

                                Price = shopcartVm.Price,
                                ProductId = shopcartVm.ProductId,
                                ProductExtraId = shopcartVm.ProductExtraId,
                                ShoppingCartId = itemAlreadyExistInCustomerCart.ShoppingCartId
                            };
                            ShopingCartProductExtraFeaturesNewList.Add(shopcart);
                        }
                        _dbContext.ShopingCartProductExtraFeatures.AddRange(ShopingCartProductExtraFeaturesNewList);


                        _dbContext.SaveChanges();
                    }

                    return Ok(new { Status = true, message = "Item Succesfuly Added" });

                    //}

                }

                else
                {

                    shoppingItemObj = new ShoppingCart
                    {
                        UserId = shoppingCartVM.UserId,
                        ProductId = shoppingCartVM.ProductId,
                        ItemPrice = shoppingCartVM.ProductPrice,
                        ProductQty = shoppingCartVM.ProductQuantity,
                        ProductTotal = shoppingCartVM.ProductTotal,
                        Deliverycost = shopDeliveryCost,
                        ProductPriceId = shoppingCartVM.ProductPriceId,

                    };

                    if (shoppingCartVM.ShopingCartProductExtraFeatures != null)
                    {
                        List<ShopingCartProductExtraFeatures> ShopingCartProductExtraFeaturesList = new List<ShopingCartProductExtraFeatures>();
                        foreach (var shopcartVm in shoppingCartVM.ShopingCartProductExtraFeatures)
                        {
                            var shopcart = new ShopingCartProductExtraFeatures()
                            {
                                Price = shopcartVm.Price,
                                ProductId = shopcartVm.ProductId,
                                ProductExtraId = shopcartVm.ProductExtraId,
                            };
                            ShopingCartProductExtraFeaturesList.Add(shopcart);

                        }
                        shoppingItemObj.ShopingCartProductExtraFeatures = ShopingCartProductExtraFeaturesList;
                    }
                    _dbContext.ShoppingCarts.Add(shoppingItemObj);
                    _dbContext.SaveChanges();

                }

                return Ok(new { Status = true, message = "Item Succesfuly Added", Obj = shoppingItemObj });

            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region GetCustomerAddressById
        [HttpGet]
        [Route("GetCustomerAddressById")]
        public IActionResult GetCustomerAddressById(int customerAddressId)
        {
            try
            {
                var customerAddress = _dbContext.CustomerAddresses.Where(e => e.CustomerAddressId == customerAddressId).FirstOrDefault();
                if (customerAddress == null)
                {
                    return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                }

                return Ok(new { Status = true, CustomerAddress = customerAddress });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #region CustomersAddress By CustomerId
        [HttpGet]
        [Route("GetAllCustomersAddressByUserId")]
        public async Task<IActionResult> GetAllCustomersAddressByUserId(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var customerAddressList = _dbContext.CustomerAddresses.Where(e => e.UserId == userId).ToList();
                return Ok(new { Status = true, CustomerAddressList = customerAddressList });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #region Change Quantity
        [HttpPost]
        [Route("ChangeProductQuantity")]
        public async Task<IActionResult> ChangeProductQuantity(int Productid, string userId, int Qty)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var itemObj = _dbContext.Products.FirstOrDefault(c => c.ProductId == Productid);
                if (itemObj == null)
                {
                    return Ok(new { Status = false, message = "Product  Object Not Found" });

                }

                var shoppingCartItem = _dbContext.ShoppingCarts.FirstOrDefault(c => c.ProductId == Productid && c.UserId == userId);
                if (shoppingCartItem == null)
                {
                    return Ok(new { Status = false, message = "Shopping Cart Item  Object Not Found" });
                }

                var singlePrice = shoppingCartItem.ProductTotal / shoppingCartItem.ProductQty;


                shoppingCartItem.ProductTotal = Qty * singlePrice;
                shoppingCartItem.ProductQty = Qty;
                _dbContext.Attach(shoppingCartItem).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return Ok(new { Status = true, message = "Item Qty Edited", ShoppingCartItem = shoppingCartItem });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }


        }
        //[HttpPost]
        //[Route("ChangeProductQuantity")]
        //public async Task<IActionResult> ChangeProductQuantity(int Productid, string userId, int Qty)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user == null)
        //        {
        //            return Ok(new { Status = false, message = "User Not Found" });

        //        }
        //        var itemObj = _dbContext.Products.FirstOrDefault(c => c.ProductId == Productid);
        //        if (itemObj == null)
        //        {
        //            return Ok(new { Status = false, message = "Product  Object Not Found" });

        //        }

        //        var shoppingCartItem = _dbContext.ShoppingCarts.FirstOrDefault(c => c.ProductId == Productid && c.UserId == userId);
        //        if (shoppingCartItem == null)
        //        {
        //            return Ok(new { Status = false, message = "Shopping Cart Item  Object Not Found" });
        //        }
        //        shoppingCartItem.ProductQty = Qty;

        //        if (Qty > shoppingCartItem.ProductQty)
        //        {
        //            shoppingCartItem.ProductTotal = (Qty * shoppingCartItem.ProductTotal) / (Qty - 1);
        //        }
        //        else
        //        {
        //            shoppingCartItem.ProductTotal = (Qty * shoppingCartItem.ProductTotal) / (Qty + 1);
        //        }
        //        _dbContext.Attach(shoppingCartItem).State = EntityState.Modified;
        //        _dbContext.SaveChanges();

        //        return Ok(new { Status = true, message = "Item Qty Edited", ShoppingCartItem = shoppingCartItem });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, message = ex.Message });

        //    }


        //}
        #endregion

        #region GetOrdersByUserId
        [HttpGet]
        [Route("GetOrdersByUserId")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var listOfIOrders = _dbContext.Orders.Include(e => e.OrderItems).Where(c => c.UserId == userId)

                    .Select(c => new
                    {
                        OrderId = c.OrderId,

                        UserId = c.UserId,
                        CustomerAddressId = c.CustomerAddressId != null ? c.CustomerAddressId : null,
                        ClassifiedBusinessId = c.ClassifiedBusinessId,
                        Discount = c.Discount,
                        OrderTotal = c.OrderTotal,
                        Deliverycost = c.Deliverycost,
                        IsDeliverd = c.IsDeliverd,
                        OrderSerial = c.OrderSerial,
                        IsCancelled = c.IsCancelled,
                        OrderDate = c.OrderDate,
                        OrderNet = c.OrderNet,
                        IsPaid = c.IsPaid,
                        Adress = c.Adress,
                        OrderItems = c.OrderItems


                    }).ToList();


                return Ok(new { Status = true, ListOfIOrders = listOfIOrders });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }

        #endregion
        #region Delete Product From Shopping Cart
        [HttpDelete]
        [Route("DeleteProductFromShoppingCart")]
        public async Task<IActionResult> DeleteProductFromShoppingCart(int productid, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var itemObj = _dbContext.Products.FirstOrDefault(c => c.ProductId == productid);
                if (itemObj == null)
                {
                    return Ok(new { status = false, message = "Product  Object Not Found" });

                }

                var shoppingCartItem = _dbContext.ShoppingCarts.Where(c => c.ProductId == productid && c.UserId == userId).FirstOrDefault();

                var shoppingCartextraProduct = _dbContext.ShopingCartProductExtraFeatures.Where(e => e.ShoppingCartId == shoppingCartItem.ShoppingCartId).ToList();

                if (shoppingCartItem == null)
                {
                    return Ok(new { status = false, message = "Shopping Cart Item Object Not Found" });

                }
                if (shoppingCartextraProduct != null)
                {
                    _dbContext.ShopingCartProductExtraFeatures.RemoveRange(shoppingCartextraProduct);
                }
                _dbContext.ShoppingCarts.Remove(shoppingCartItem);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, message = "Item Deleted Successfully" });
            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }


        }
        #endregion
        #region GetAllShoppingCartByUserId
        [HttpGet]
        [Route("GetShoppingCartByUserId")]
        public async Task<IActionResult> GetShoppingCartByUserId(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var shoppingCartList = _dbContext.ShoppingCarts
                                                    .Include(s => s.Product)
                                                    .ThenInclude(s => s.ProductCategory.ClassifiedBusiness)
                                                    .Where(c => c.UserId == userId)
                                                    .Select(c => new
                                                    {
                                                        ShoppingCartId = c.ShoppingCartId,
                                                        ProductId = c.Product.ProductId,
                                                        //ProductName = c.Product.ProductName,
                                                        TitleAr = c.Product.TitleAr,
                                                        TitleEn = c.Product.TitleEn,
                                                        Description = c.Product.Description,
                                                        MainPic = c.Product.MainPic,
                                                        IsFixedPrice = c.Product.IsFixedPrice,
                                                        ISPriceFixed = c.Product.IsFixedPrice == false ? c.Product.Price : _dbContext.ProductPrices.Where(e => e.ProductId == c.ProductId).FirstOrDefault().Price,
                                                        Price = c.Product.Price,
                                                        ItemQty = c.ProductQty,
                                                        ProdcutTotal = c.ProductTotal,
                                                        BussinessTitleAr = c.Product.ProductCategory.TitleAr,
                                                        BussinessTitleEn = c.Product.ProductCategory.TitleEn,
                                                        BusinessId = c.Product.ProductCategory.ClassifiedBusinessId,
                                                    }).ToList();

                double totalDeliveryCost = 0;

                var tdc = shoppingCartList.GroupBy(c => c.BusinessId).

                Select(g => new
                {
                    TotalDeliveryCost = _dbContext.ClassifiedBusiness.SingleOrDefault(s => s.ClassifiedBusinessId == g.Key).Deliverycost

                }).ToList();

                foreach (var item in tdc)
                {
                    totalDeliveryCost += item.TotalDeliveryCost;
                }

                return Ok(new { Status = true, ShoppingCartList = shoppingCartList, TotalDeliveryCost = totalDeliveryCost });

            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region CheckOut
        [HttpPost]
        [Route("CheckOut")]
        public async Task<IActionResult> CheckOut(CheckOutVM checkOutVM)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(checkOutVM.UserId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });
                }
                if (checkOutVM.FastOrder == false)
                {
                    var customerAddressObj = _dbContext.CustomerAddresses.FirstOrDefault(c => c.CustomerAddressId == checkOutVM.CustomerAddressId);
                    if (customerAddressObj == null)
                    {
                        return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                    }
                }


                double discount = 0;
                //Get Customer ShoppingCart Items List
                var customerShoppingCartList = _dbContext.ShoppingCarts
                    .Include(e => e.ShopingCartProductExtraFeatures)
                    .Include(e => e.ProductPrice)
                    .Include(s => s.Product)
                    .ThenInclude(s => s.ProductCategory).ThenInclude(s => s.ClassifiedBusiness)
                    .Where(c => c.UserId == checkOutVM.UserId).ToList();

                var customerShoppingCartExtraList = _dbContext.ShopingCartProductExtraFeatures.Where(e => e.ShoppingCartId == customerShoppingCartList.FirstOrDefault().ShoppingCartId).ToList();

                var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ProductTotal);


                int maxUniqe = 1;
                var newList = _dbContext.Orders.ToList();
                if (newList.Count > 0)
                {
                    maxUniqe = newList.Max(e => e.UniqeId);
                }
                var orders = customerShoppingCartList.AsEnumerable().GroupBy(c => c.Product.ProductCategory.ClassifiedBusinessId).

                Select(g => new Order
                {
                    OrderDate = DateTime.Now,
                    OrderSerial = Guid.NewGuid().ToString() + "/" + DateTime.Now.Year,
                    ClassifiedBusinessId = g.Key,
                    UserId = checkOutVM.UserId,
                    CustomerAddressId = checkOutVM.FastOrder == false ? checkOutVM.CustomerAddressId : null,
                    Adress = checkOutVM.FastOrder == true ? checkOutVM.Address : null,
                    IsPaid = false,
                    OrderTotal = g.Sum(c => c.ProductTotal),
                    Deliverycost = _dbContext.ClassifiedBusiness.FirstOrDefault(s => s.ClassifiedBusinessId == g.Key).Deliverycost,
                    OrderNet = g.Sum(c => c.ProductTotal) + _dbContext.ClassifiedBusiness.FirstOrDefault(s => s.ClassifiedBusinessId == g.Key).Deliverycost,
                    Discount = discount,
                    UniqeId = maxUniqe + 1
                }).ToList();


                foreach (var item in orders)
                {
                    _dbContext.Orders.Add(item);
                    _dbContext.SaveChanges();

                    //transfer shoppingcart to orderitems table and clear shoppingcart


                    var orderitemextrafeatures = new List<OrderItemExtraProduct>();

                    foreach (var itemshop in customerShoppingCartList)
                    {

                        if (itemshop.Product.ProductCategory.ClassifiedBusinessId == item.ClassifiedBusinessId)
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                ProductId = (int)itemshop.ProductId,
                                ItemPrice = itemshop.ItemPrice,
                                ProductPriceId = itemshop.ProductPriceId,
                                Total = itemshop.ProductTotal,
                                ProductQuantity = itemshop.ProductQty,
                                OrderId = item.OrderId
                            };
                            var customerShoppingExtra = _dbContext.ShopingCartProductExtraFeatures.Where(e => e.ShoppingCartId == itemshop.ShoppingCartId).ToList();
                            if (customerShoppingExtra != null)
                            {
                                if (customerShoppingExtra.Count != 0)
                                {
                                    List<OrderItemExtraProduct> OrderItemProductExtraFeaturesNewList = new List<OrderItemExtraProduct>();
                                    foreach (var itemsExtra in customerShoppingExtra)
                                    {

                                        OrderItemExtraProduct OrderItemExtraProductobj = new OrderItemExtraProduct
                                        {

                                            ProductExtraId = itemsExtra.ProductExtraId,
                                            Price = itemsExtra.Price,

                                        };
                                        OrderItemProductExtraFeaturesNewList.Add(OrderItemExtraProductobj);

                                    }

                                    orderItem.OrderItemExtraProducts = OrderItemProductExtraFeaturesNewList;

                                }
                            }
                            _dbContext.OrderItems.Add(orderItem);
                            _dbContext.SaveChanges();


                        }
                    }
                }

                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];
                if (Fattorahstatus) // fattorah live
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = orders.Sum(e => e.OrderNet),
                        CallBackUrl = "https://albaheth.me/FattorahOrderSuccess",
                        ErrorUrl = "https://albaheth.me/FattorahOrderFailed",

                        UserDefinedField = orders.FirstOrDefault().UniqeId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://api.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = true, paymentUrl = InvoiceRes.InvoiceURL });


                    }
                    else
                    {
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = false, Message = FattoraRes.Message });

                    }
                }
                else               //fattorah test
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = orders.Sum(e => e.OrderNet),
                        CallBackUrl = "https://albaheth.me/FattorahOrderSuccess",
                        ErrorUrl = "https://albaheth.me/FattorahOrderFailed",

                        UserDefinedField = orders.FirstOrDefault().UniqeId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = true, paymentUrl = InvoiceRes.InvoiceURL });


                    }
                    else
                    {
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = false, Message = FattoraRes.Message });

                    }
                }

            }

            catch (Exception ex)
            {

                return Ok(new { Status = false, message = ex.Message });

            }

        }
        #endregion
        #endregion

        #region GetBusinessParentCategory
        [HttpGet]
        [Route("GetBusinessParentCategory")]
        public async Task<ActionResult> GetBusinessParentCategory(int BusinessCategoryId)
        {
            try
            {
                var Data = _dbContext.BusinessCategories.Where(c => c.BusinessCategoryId == BusinessCategoryId && c.BusinessCategoryIsActive).Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
                    BusinessCategorySortOrder = c.BusinessCategorySortOrder,
                    BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
                    BusinessCategoryIsActive = c.BusinessCategoryIsActive,
                    BusinessCategoryDescAr = c.BusinessCategoryDescAr,
                    BusinessCategoryDescEn = c.BusinessCategoryDescEn,
                    BusinessCategoryParentId = c.BusinessCategoryParentId,
                    HasChild = _dbContext.BusinessCategories.Any(x => x.BusinessCategoryParentId == c.BusinessCategoryId),

                }).FirstOrDefaultAsync();

                if (Data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        #endregion
        #region  GetBusinessChildCategories
        [HttpGet]
        [Route("GetBusinessChildCategories")]
        public async Task<ActionResult> GetBusinessChildCategories(int BusinessCategoryId)
        {
            try
            {
                var Data = _dbContext.BusinessCategories.Where(c => c.BusinessCategoryParentId == BusinessCategoryId).Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
                    BusinessCategorySortOrder = c.BusinessCategorySortOrder,
                    BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
                    BusinessCategoryIsActive = c.BusinessCategoryIsActive,
                    BusinessCategoryDescAr = c.BusinessCategoryDescAr,
                    BusinessCategoryDescEn = c.BusinessCategoryDescEn,
                    BusinessCategoryParentId = c.BusinessCategoryParentId,
                    HasChild = _dbContext.BusinessCategories.Any(x => x.BusinessCategoryParentId == c.BusinessCategoryId),

                }).ToListAsync();

                if (Data is null)
                {
                    return NotFound();
                }

                return Ok(new { Status = true, Data, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        #endregion

        //#region GetBusinessCategoryChild
        //[HttpGet]
        //[Route("GetBusinessCategoryChild")]
        //public async Task<ActionResult> GetBusinessCategoryChild()
        //{
        //    try
        //    {
        //        var Data = _dbContext.BusinessCategories.Select(c => new
        //        {
        //            BusinessCategoryId = c.BusinessCategoryId,
        //            BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
        //            BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
        //            BusinessCategorySortOrder = c.BusinessCategorySortOrder,
        //            BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
        //            BusinessCategoryIsActive = c.BusinessCategoryIsActive,
        //            BusinessCategoryDescAr = c.BusinessCategoryDescAr,
        //            BusinessCategoryDescEn = c.BusinessCategoryDescEn,
        //            BusinessCategoryParentId = c.BusinessCategoryParentId,
        //            BusinessCategoryParent=_dbContext.BusinessCategories.Where(e=>e.BusinessCategoryParentId==c.BusinessCategoryId).ToList(),
        //            HasChild = _dbContext.BusinessCategories.Any(x => x.BusinessCategoryParentId == c.BusinessCategoryId),

        //        }).ToListAsync();

        //        if (Data is null)
        //        {
        //            return NotFound();
        //        }

        //        return Ok(new { Status = true, Data, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = "Something went wrong" });
        //    }

        //}
        //#endregion
        [HttpPost]
        [Route("PostBussinessReview")]
        public IActionResult PostBussinessReview(ReviewVM review)
        {
            var BD = _dbContext.ClassifiedBusiness.Where(a => a.ClassifiedBusinessId == review.BDId).FirstOrDefault();
            if (BD == null)
            {
                return Ok(new { Status = false, message = "BD Not Found" });
            }
            var reviewobj = new Review()
            {
                ClassifiedBusinessId = review.BDId,
                Email = review.Email,
                Name = review.Name,
                Rating = review.Rating,
                Title = review.Title,
                ReviewDate = DateTime.Now
            };
            try
            {
                _dbContext.Reviews.Add(reviewobj);
                _dbContext.SaveChanges();
                var ReviewList = _dbContext.Reviews.Where(a => a.ClassifiedBusinessId == review.BDId).ToList();
                var Avg = ReviewList.Count() / ReviewList.Sum(a => a.Rating);
                BD.Rating = Avg;
                _dbContext.Attach(BD).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(new { Status = true, message = "Thank You For Review" });

            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }


        }
        #region GetBusinessMapLocation
        [HttpGet]
        [Route("GetBusinessMapLocation")]
        public async Task<IActionResult> GetBusinessMapLocation()
        {
            try
            {

                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Include(e => e.BusinessCategory).Include(c => c.BusinessContents).ThenInclude(e => e.BusinessTemplateConfigs).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategory.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategory.BusinessCategoryTitleEn,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Deliverycost = c.Deliverycost,
                    Rating = c.Rating,
                    UseId = c.UseId,
                    Views = c.Views,
                    Map = _dbContext.BusinessContents.Include(e => e.BusinessContentValues).Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.BusinessTemplateConfigId == c.BusinessContents.Where(e => e.BusinessTemplateConfigs.FieldTypeId == 14 && e.BusinessTemplateConfigs.BusinessCategoryId == c.BusinessCategoryId).FirstOrDefault().BusinessTemplateConfigId).FirstOrDefault().BusinessContentValues.FirstOrDefault().ContentValue,


                }).ToListAsync();




                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        #endregion
        [HttpGet]
        [Route("GetAllSearchResult")]
        public async Task<IActionResult> GetAllSearchResult(string Search, int page = 1, int pageSize = 10)
        {
            if (Search == null)
            {
                return Ok(new { Status = false, message = "Enter Any Text To search." });
            }
            try
            {
                var users = _applicationDbContext.Users.Where(a => a.UserName.ToUpper().Contains(Search.ToUpper())).ToList();
                var buisness = _dbContext.ClassifiedBusiness.Where(a => a.IsActive == true).Include(a => a.BusinessCategory).Where(a => a.BusinessCategory.BusinessCategoryTitleEn.ToUpper().Contains(Search.ToUpper()) || a.BusinessCategory.BusinessCategoryTitleAr.ToUpper().Contains(Search.ToUpper()) || a.Title.ToUpper().Contains(Search.ToUpper())).ToList();
                var Products = _dbContext.Products.Include(a => a.ProductPrices).Include(a => a.ProductCategory).ThenInclude(a => a.ClassifiedBusiness).Where(a => a.ProductCategory.ClassifiedBusiness.IsActive == true && a.IsActive == true && a.ProductCategory.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.ProductCategory.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.TitleAr.ToUpper().Contains(Search.ToUpper())).ToList();
                var ClassifiedAds = _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Include(c => c.AdContents).ThenInclude(e => e.AdTemplateConfig).Where(a => a.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn.ToUpper().Contains(Search.ToUpper()) || a.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr.ToUpper().Contains(Search.ToUpper())).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                    Title = _dbContext.AdContents.Include(e => e.AdContentValues).Where(e => e.ClassifiedAdId == c.ClassifiedAdId && e.AdTemplateConfigId == c.AdContents.Where(e => e.AdTemplateConfig.FieldTypeId == 1 && e.AdTemplateConfig.ClassifiedAdsCategoryId == c.ClassifiedAdsCategoryId).FirstOrDefault().AdTemplateConfigId).FirstOrDefault().AdContentValues.FirstOrDefault().ContentValue,
                    UseId = c.UseId,
                    Price = c.Price,
                    PublishDate = c.PublishDate,
                    Pic = _dbContext.AdContents.Include(e => e.AdContentValues).Where(e => e.ClassifiedAdId == c.ClassifiedAdId && e.AdTemplateConfigId == c.AdContents.Where(e => e.AdTemplateConfig.FieldTypeId == 8 && e.AdTemplateConfig.ClassifiedAdsCategoryId == c.ClassifiedAdsCategoryId).FirstOrDefault().AdTemplateConfigId).FirstOrDefault().AdContentValues.FirstOrDefault().ContentValue,
                    Description=c.Description

                }).ToList();
                var Offers = _dbContext.BDOffers.Include(a => a.BDOfferImages).Include(a => a.ClassifiedBusiness).ThenInclude(a => a.BusinessCategory).Where(a => a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.ClassifiedBusiness.Title.ToUpper().Contains(Search.ToUpper())).ToList();

                var Searchresult = new List<SearchVM>();
                foreach (var item in users)
                {
                    var userssw = await _userManager.FindByEmailAsync(item.UserName);
                    var searchobj = new SearchVM() { UserId = item.Id, id = item.Id, Title = userssw.FullName, CategoryEn = userssw.Job, CategoryAr = userssw.Job, image = userssw.ProfilePicture, Price = 0, PublishData = DateTime.Now, Type = 1 };
                    Searchresult.Add(searchobj);
                }
                foreach (var item in buisness)
                {

                    var searchobj = new SearchVM() { UserId = item.UseId, id = item.ClassifiedBusinessId.ToString(), CategoryEn = item.BusinessCategory.BusinessCategoryTitleAr, CategoryAr = item.BusinessCategory.BusinessCategoryTitleEn, Title = item.Title, image = item.Mainpic, Price = 0, PublishData = item.PublishDate.Value,description=item.Description!=null?item.Description:"", Type = 2 };
                    Searchresult.Add(searchobj);
                }

                foreach (var ele in ClassifiedAds)
                {
                    var searchobj = new SearchVM() { UserId = ele.UseId, id = ele.ClassifiedAdId.ToString(), CategoryEn = ele.ClassifiedAdsCategoryTitleEn, CategoryAr = ele.ClassifiedAdsCategoryTitleAr, Title = ele.Title, image = ele.Pic, Price = ele.Price, description = ele.Description != null ? ele.Description : "" ,PublishData = ele.PublishDate.Value, Type = 3 };
                    Searchresult.Add(searchobj);
                }
                foreach (var item in Products)
                {
                    if (item.IsFixedPrice)
                    {
                        var searchobj = new SearchVM() { UserId = item.ProductCategory.ClassifiedBusiness.UseId, id = item.ProductId.ToString(), CategoryEn = item.ProductCategory.TitleEn, CategoryAr = item.ProductCategory.TitleAr, Title = item.TitleEn, image = item.MainPic, description = item.Description != null ? item.Description : "", PublishData = DateTime.Now, Price = item.Price.Value, Type = 4 };
                        Searchresult.Add(searchobj);
                    }
                    else
                    {
                        foreach (var newItem in item.ProductPrices)
                        {
                            var searchobj = new SearchVM() { UserId = item.ProductCategory.ClassifiedBusiness.UseId, id = newItem.ProductPriceId.ToString(), description = newItem.ProductPriceDes != null ? newItem.ProductPriceDes : "", CategoryEn = item.ProductCategory.TitleEn, CategoryAr = item.ProductCategory.TitleAr, Title = newItem.ProductPriceTilteEn, image = item.MainPic, PublishData = DateTime.Now, Price = newItem.Price, Type = 4 };
                            Searchresult.Add(searchobj);
                        }
                    }

                }
                foreach (var item in Offers)
                {
                    var searchobj = new SearchVM() { UserId = item.ClassifiedBusiness.UseId, id = item.BDOfferId.ToString(),description=item.OfferDescription!=null?item.OfferDescription:"", CategoryEn = item.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleEn, CategoryAr = item.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleAr, Title = item.TitleAr, image = item.Pic, Price = item.Price, PublishData = item.PublishDate, Type = 5 };
                    Searchresult.Add(searchobj);
                }
                if (Searchresult.Count() == 0)
                {
                    return Ok(new { Status = false, message = "Not Matched Result.. " });
                }
                var rnd = new Random();
                var RandomList = Searchresult.Skip((page - 1) * pageSize).Take(pageSize).OrderBy(x => rnd.Next());

                var model = new
                {
                    status = true,
                    searchresult = RandomList
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }

        //[HttpGet]
        //[Route("GetSearchForAds")]
        //public async Task<IActionResult> GetSearchForAds(string Search, int CatagoryId, int page = 1, int pageSize = 10)
        //{
        //    if (Search == null)
        //    {
        //        return Ok(new { Status = false, message = "Enter Any Text To search." });
        //    }
        //    var AdsCatagory = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == CatagoryId).FirstOrDefault();
        //    if (AdsCatagory == null)
        //    {
        //        return Ok(new { Status = false, message = "Catagory Not Found" });

        //    }
        //    //LoadListChildCatagory(int catagoryId);
        //    try
        //    {
        //        //var ClassifiedAds =await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(a => a.ClassifiedAdsCategoryId == CatagoryId).Select(c => new
        //        //{
        //        //    ClassifiedAdId = c.ClassifiedAdId,
        //        //    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //        //    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //        //    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //        //    TitleAr = c.TitleAr == null ? "T" : c.TitleAr,
        //        //    TitleEn = c.TitleEn == null ? "T" : c.TitleEn,
        //        //    UseId = c.UseId,
        //        //    Price = c.Price,
        //        //    PublishDate = c.PublishDate,
        //        //    Pic = c.MainPic,
        //        //    Description = c.Description == null ? "T" : c.Description,

        //        //}).ToListAsync();
        //        //var AllAds= await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(a => a.IsActive == true).Select(c => new
        //        //{
        //        //    ClassifiedAdId = c.ClassifiedAdId,
        //        //    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //        //    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //        //    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //        //    TitleAr = c.TitleAr == null ? "T" : c.TitleAr,
        //        //    TitleEn = c.TitleEn == null ? "T" : c.TitleEn,
        //        //    UseId = c.UseId,
        //        //    Price = c.Price,
        //        //    PublishDate = c.PublishDate,
        //        //    Pic = c.MainPic,
        //        //    Description = c.Description == null ? "T" : c.Description,

        //        //}).ToListAsync();
        //        var ClassifiedAds = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(a => a.ClassifiedAdsCategoryId == CatagoryId).ToListAsync();
        //        var AllAds = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(a => a.IsActive == true).ToListAsync();
        //        await LoadChildCategoriesAndAds(CatagoryId, ClassifiedAds, AllAds);
        //        var Ads = ClassifiedAds.Select(c => new
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //            ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //            ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //            TitleAr = c.TitleAr == null ? "T" : c.TitleAr,
        //            TitleEn = c.TitleEn == null ? "T" : c.TitleEn,
        //            UseId = c.UseId,
        //            Price = c.Price,
        //            PublishDate = c.PublishDate,
        //            Pic = c.MainPic,
        //            Description = c.Description == null ? "T" : c.Description,

        //        }).ToList();
        //        if (ClassifiedAds != null)
        //        {
        //            Ads = Ads.Where(a => a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.Description.ToUpper().Contains(Search.ToUpper())).ToList();
        //            if (Ads != null)
        //            {
        //                Ads = Ads.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //            }
        //        }
        //        return Ok(new { Status = true, ClassifiedAds = Ads, Message = "Process completed successfully" });

        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(new { Status = false, message = e.Message });
        //    }
        //}
        //[HttpGet]
        //[Route("GetSearchForAds")]
        //public async Task<IActionResult> GetSearchForAds(string Search, int CatagoryId, int page = 1, int pageSize = 10)
        //{
        //    if (Search == null)
        //    {
        //        return Ok(new { Status = false, message = "Enter Any Text To search." });
        //    }
        //    var AdsCatagory = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == CatagoryId).FirstOrDefault();
        //    if (AdsCatagory == null)
        //    {
        //        return Ok(new { Status = false, message = "Catagory Not Found" });

        //    }
        //    //var query = from c1 in _dbContext.ClassifiedAdsCategories
        //    //            where c1.ClassifiedAdsCategoryParentId == null
        //    //            join c2 in _dbContext.ClassifiedAdsCategories on c1.ClassifiedAdsCategoryId equals c2.ClassifiedAdsCategoryParentId
        //    //            select c2;
        //    //foreach(var item in query)
        //    //{

        //    //}
        //    try
        //    {
        //        var catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == CatagoryId).Select(e => e.SearchCatagoryLevel).ToList();
        //        var ClassifiedAds = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => catList.Contains(e.ClassifiedAdsCategoryId.Value)).Select(c => new
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //            ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //            ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //            TitleAr = c.TitleEn == null ? " " : c.TitleAr,
        //            TitleEn = c.TitleEn == null ? " " : c.TitleEn,
        //            UseId = c.UseId,
        //            Price = c.Price,
        //            PublishDate = c.PublishDate,
        //            Pic = c.MainPic,
        //            Description = c.Description == null ? " " : c.Description,
        //            user = _userManager.FindByIdAsync(c.UseId),
        //        }).ToListAsync();
        //        //List<int> catsAdsIds = new List<int>() { AdsCatagory.ClassifiedAdsCategoryId };
        //        //await LoadListChildCatagory(AdsCatagory.ClassifiedAdsCategoryId, catsAdsIds);
        //        //try
        //        //{
        //        //    var ClassifiedAds = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => catsAdsIds.Contains(e.ClassifiedAdsCategoryId.Value)).Select(c => new
        //        //    {
        //        //        ClassifiedAdId = c.ClassifiedAdId,
        //        //        ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //        //        ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //        //        ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //        //        TitleAr = c.TitleAr == null ? "T" : c.TitleAr,
        //        //        TitleEn = c.TitleEn == null ? "T" : c.TitleEn,
        //        //        UseId = c.UseId,
        //        //        Price = c.Price,
        //        //        PublishDate = c.PublishDate,
        //        //        Pic = c.MainPic,
        //        //        Description = c.Description == null ? "T" : c.Description,

        //        //    }).ToListAsync();
        //        //    if (ClassifiedAds != null)
        //        //    {
        //        //        ClassifiedAds = ClassifiedAds.Where(a => a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.Description.ToUpper().Contains(Search.ToUpper())).ToList();
        //        //        if (ClassifiedAds != null)
        //        //        {
        //        //            ClassifiedAds = ClassifiedAds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //        //        }
        //        //    }
        //        //    return Ok(new { Status = true, ClassifiedAds = ClassifiedAds, Message = "Process completed successfully" });
        //        if (ClassifiedAds != null)
        //        {
        //            ClassifiedAds = ClassifiedAds.Where(e => e.TitleAr.ToUpper().Contains(Search.ToUpper()) || e.TitleEn.ToUpper().Contains(Search.ToUpper()) || e.Description.ToUpper().Contains(Search.ToUpper())).ToList();
        //            ClassifiedAds = ClassifiedAds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //        }
        //        return Ok(new { Status = true, ClassifiedAds = ClassifiedAds, Message = "Process completed successfully" });

        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(new { Status = false, message = e.Message });
        //    }
        //}
        [HttpGet]
        [Route("GetSearchForAds")]
        public async Task<IActionResult> GetSearchForAds(string Search, int CatagoryId, int page = 1, int pageSize = 10)
        {
            if (Search == null)
            {
                return Ok(new { Status = false, message = "Enter Any Text To search." });
            }
            var AdsCatagory = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == CatagoryId).FirstOrDefault();
            if (AdsCatagory == null)
            {
                return Ok(new { Status = false, message = "Catagory Not Found" });

            }

            try
            {
                var ClassifiedAds = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                    TitleAr = c.TitleAr == null ? " " : c.TitleAr,
                    TitleEn = c.TitleEn == null ? " " : c.TitleEn,
                    Tags = c.Tags == null ? " " : c.Tags,
                    UseId = c.UseId,
                    Price = c.Price,
                    PublishDate = c.PublishDate,
                    Pic = c.MainPic,
                    Description = c.Description == null ? " " : c.Description,
                    user = _userManager.FindByIdAsync(c.UseId),
                }).ToListAsync();
                if (ClassifiedAds != null)
                {
                    ClassifiedAds = ClassifiedAds.Where(e => e.TitleAr.ToUpper().Contains(Search.ToUpper()) || e.TitleEn.ToUpper().Contains(Search.ToUpper()) || e.Description.ToUpper().Contains(Search.ToUpper()) || e.Tags.ToUpper().Contains(Search.ToUpper())).ToList();
                    ClassifiedAds = ClassifiedAds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return Ok(new { Status = true, ClassifiedAds = ClassifiedAds, Message = "Process completed successfully" });

            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }


        [HttpGet]
        [Route("GetSearchForAllAds")]
        public async Task<IActionResult> GetSearchForAllAds(string Search, int page = 1, int pageSize = 10)
        {
            if (Search == null)
            {
                return Ok(new { Status = false, message = "Enter Any Text To search." });
            }
            //ContentValueEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 5 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,

            // var AdContentId = _dbContext.AdContents.Include(e => e.AdTemplateConfig).Where(e => e.AdTemplateConfig.FieldTypeId == 3 || e.AdTemplateConfig.FieldTypeId == 6 || e.AdTemplateConfig.FieldTypeId == 5).Select(e => e.AdContentId);
            // var AdOptionsId = Convert.ToInt32(_dbContext.AdContentValues.Where(e => AdContentId.Contains(e.AdContentId)).Select(e => e.ContentValue));
            //var options = _dbContext.AdTemplateOptions.Where(e =>AdOptionsId.Contains(e.AdTemplateOptionId)).ToList();
            //var options = _dbContext.AdTemplateOptions.Where(e => Convert.ToInt32(_dbContext.AdContentValues.Where(e => _dbContext.AdContents.Include(e => e.AdTemplateConfig).Where(e => e.AdTemplateConfig.FieldTypeId == 3 || e.AdTemplateConfig.FieldTypeId == 6 || e.AdTemplateConfig.FieldTypeId == 5).Select(e => e.AdContentId).Contains(e.AdContentId)).Select(e => e.ContentValue)).Contains(e.AdTemplateOptionId)).ToList();
            //var options = _dbContext.AdTemplateOptions.Where(e => Convert.ToInt32(_dbContext.AdContentValues.Where(e => _dbContext.AdContents.Include(e => e.AdTemplateConfig).Where(e => e.AdTemplateConfig.FieldTypeId == 3 || e.AdTemplateConfig.FieldTypeId == 6 || e.AdTemplateConfig.FieldTypeId == 5).Select(e => e.AdContentId).Contains(e.AdContentId)).Select(e => e.ContentValue).Contains(e.AdTemplateOptionId))).ToList();
            //var InOne=
            //var ContentValues = _dbContext.AdContentValues.Include(e => e.AdContent).ThenInclude(e => e.AdTemplateConfig).Where(e => e.AdContent.AdTemplateConfig.FieldTypeId == 3 || e.AdContent.AdTemplateConfig.FieldTypeId == 6 || e.AdContent.AdTemplateConfig.FieldTypeId == 5).ToList();
            //.FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 5 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue
            //ContentValueAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 5 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,
            try
            {
                //var user = await _userManager.FindByIdAsync(UserId);
                //var ClassifiedAds = await _dbContext.ClassifiedAds.Include(e=>e.AdContents).ThenInclude(e=>e.AdContentValues).Where(a => a.IsActive == true && a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.Description.ToUpper().Contains(Search.ToUpper())||a.AdContents.Where(e=> e.AdContentValues.ToList().Select(e.ContentValue).Contains(Search.ToUpper()).AdContentValues.Select(e=>e.ContentValue).FirstOrDefault().ToUpper().Contains(Search.ToUpper())).Include(e => e.ClassifiedAdsCategory).Select(c => new
                //var ClassifiedAds = await _dbContext.ClassifiedAds.Include(e=>e.AdContents).ThenInclude(e=>e.AdContentValues).Where(a => a.IsActive == true && a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.Description.ToUpper().Contains(Search.ToUpper())||a.AdContents.Where(e=> e.AdTemplateConfig.FieldTypeId==3|| e.AdTemplateConfig.FieldTypeId==6|| e.AdTemplateConfig.FieldTypeId==5|| e.AdTemplateConfig.FieldTypeId==13).Contains(Search.ToUpper())).Include(e => e.ClassifiedAdsCategory).Select(c => new
                //var ClassifiedAds = await _dbContext.ClassifiedAds.Include(e=>e.AdContents).ThenInclude(e=>e.AdContentValues).Where(a => a.IsActive == true && a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.Description.ToUpper().Contains(Search.ToUpper())||a.AdContents.Where(e=> e.AdTemplateConfig.FieldTypeId==3|| e.AdTemplateConfig.FieldTypeId==6|| e.AdTemplateConfig.FieldTypeId==5|| e.AdTemplateConfig.FieldTypeId==13).Any(k=>k.AdContentValues.Where(b => b.ContentValue.ToUpper().Contains(Search.ToUpper())).FirstOrDefault().AdContentValues.Where(b=>b.ContentValue.ToUpper().Contains(Search.ToUpper()))).Include(e => e.ClassifiedAdsCategory).Select(c => new
                var ClassifiedAds = await _dbContext.ClassifiedAds.Include(e => e.AdContents).ThenInclude(e => e.AdContentValues).Where(a => a.IsActive == true && a.TitleAr.ToUpper().Contains(Search.ToUpper()) || a.TitleEn.ToUpper().Contains(Search.ToUpper()) || a.Description.ToUpper().Contains(Search.ToUpper()) || a.AdContents.Where(e => e.AdTemplateConfig.FieldTypeId == 3 || e.AdTemplateConfig.FieldTypeId == 6 || e.AdTemplateConfig.FieldTypeId == 5 || e.AdTemplateConfig.FieldTypeId == 13).Any(k => k.AdContentValues.Any(l => l.ContentValue.ToUpper().Contains(Search.ToUpper())))).Include(e => e.ClassifiedAdsCategory).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    UseId = c.UseId,
                    Price = c.Price,
                    PublishDate = c.PublishDate,
                    Pic = c.MainPic,
                    Description = c.Description,
                    user = _userManager.FindByIdAsync(c.UseId),
                    Map = _dbContext.AdContents.Include(e => e.AdContentValues).Where(e => e.ClassifiedAdId == c.ClassifiedAdId && e.AdTemplateConfigId == c.AdContents.Where(e => e.AdTemplateConfig.FieldTypeId == 14 && e.AdTemplateConfig.ClassifiedAdsCategoryId == c.ClassifiedAdsCategoryId).FirstOrDefault().AdTemplateConfigId).FirstOrDefault().AdContentValues.FirstOrDefault().ContentValue,

                }).ToListAsync();
                if (ClassifiedAds != null)
                {
                    ClassifiedAds = ClassifiedAds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return Ok(new { Status = true, ClassifiedAds = ClassifiedAds, Message = "Process completed successfully" });

            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetAllUserFavourite")]
        public async Task<IActionResult> GetAllUserFavourite(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return Ok(new { Status = false, message = "User Not Found" });

            }


            try
            {
                var ProfileFavourites = _dbContext.FavouriteProfiles.Where(e => e.UserId == UserId).ToList();
                var buisnessFavourites = _dbContext.FavouriteBusiness.Where(e => e.UserId == UserId).ToList();
                var ProductFavourites = _dbContext.FavouriteProducts.Where(e => e.UserId == UserId).ToList();
                var ClassifiedFavourites = _dbContext.FavouriteClassifieds.Where(e => e.UserId == UserId).ToList();
                var ServiceFavourites = _dbContext.ServiceFavourites.Where(e => e.UserId == UserId).ToList();
                var Searchresult = new List<SearchVM>();
                foreach (var item in ProfileFavourites)
                {

                    var userssw = await _userManager.FindByIdAsync(item.Id);
                    var searchobj = new SearchVM() { id = item.Id, Title = userssw.FullName, CategoryEn = userssw.Job, CategoryAr = userssw.Job, image = userssw.ProfilePicture, Price = 0, PublishData = DateTime.Now, Type = 1 };
                    Searchresult.Add(searchobj);
                }
                foreach (var item in buisnessFavourites)
                {
                    var buisness = _dbContext.ClassifiedBusiness.Where(a => a.ClassifiedBusinessId == item.ClassifiedBusinessId).Include(a => a.BusinessCategory).FirstOrDefault();
                    var searchobj = new SearchVM() { id = item.ClassifiedBusinessId.ToString(), CategoryEn = buisness.BusinessCategory.BusinessCategoryTitleAr, CategoryAr = buisness.BusinessCategory.BusinessCategoryTitleEn, Title = buisness.Title, image = buisness.Mainpic, Price = 0, PublishData = buisness.PublishDate.Value, Type = 2 };
                    Searchresult.Add(searchobj);
                }

                foreach (var ele in ClassifiedFavourites)
                {
                    var ClassifiedAds = _dbContext.ClassifiedAds.Where(a => a.ClassifiedAdId == ele.ClassifiedAdId).Include(e => e.ClassifiedAdsCategory).Select(c => new
                    {
                        ClassifiedAdId = c.ClassifiedAdId,
                        ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                        ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                        ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                        Title = c.TitleEn,
                        UseId = c.UseId,
                        Price = c.Price,
                        PublishDate = c.PublishDate,
                        Pic = c.MainPic

                    }).FirstOrDefault();
                    var searchobj = new SearchVM() { id = ele.ClassifiedAdId.ToString(), CategoryEn = ClassifiedAds.ClassifiedAdsCategoryTitleEn, CategoryAr = ClassifiedAds.ClassifiedAdsCategoryTitleAr, Title = ClassifiedAds.Title, image = ClassifiedAds.Pic, Price = ClassifiedAds.Price, PublishData = ClassifiedAds.PublishDate.Value, Type = 3 };
                    Searchresult.Add(searchobj);
                }
                foreach (var item in ProductFavourites)
                {
                    var Product = _dbContext.Products.Include(a => a.ProductPrices).Include(a => a.ProductCategory).ThenInclude(a => a.ClassifiedBusiness).Where(a => a.ProductId == item.ProductId).FirstOrDefault();

                    if (Product.IsFixedPrice)
                    {
                        var searchobj = new SearchVM() { id = Product.ProductId.ToString(), CategoryEn = Product.ProductCategory.TitleEn, CategoryAr = Product.ProductCategory.TitleAr, Title = Product.TitleEn, image = Product.MainPic, PublishData = DateTime.Now, Price = Product.Price.Value, Type = 4 };
                        Searchresult.Add(searchobj);
                    }
                    else
                    {
                        foreach (var newItem in Product.ProductPrices)
                        {
                            var searchobj = new SearchVM() { id = newItem.ProductPriceId.ToString(), CategoryEn = Product.ProductCategory.TitleEn, CategoryAr = Product.ProductCategory.TitleAr, Title = newItem.ProductPriceTilteEn, image = Product.MainPic, PublishData = DateTime.Now, Price = newItem.Price, Type = 4 };
                            Searchresult.Add(searchobj);
                        }
                    }

                }
                foreach (var item in ServiceFavourites)
                {
                    var service = _dbContext.Services.Where(a => a.ServiceId == item.ServiceId).Include(a => a.ServiceCatagory).FirstOrDefault();
                    var searchobj = new SearchVM() { id = item.ServiceId.ToString(), CategoryEn = service.ServiceCatagory.ServiceCatagoryTitleEn, CategoryAr = service.ServiceCatagory.ServiceCatagoryTitleAr, Title = service.ServiceTitleEn, image = service.MainPic, Price = service.Price, PublishData = DateTime.Now, Type = 5 };
                    Searchresult.Add(searchobj);
                }

                if (Searchresult.Count() == 0)
                {
                    return Ok(new { Status = false, message = "Not Favourite " });
                }
                var rnd = new Random();
                var RandomList = Searchresult.OrderBy(x => rnd.Next());

                var model = new
                {
                    status = true,
                    searchresult = RandomList
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }
        [HttpPut]
        [Route("DeActivateBusinessDirectory")]
        public async Task<IActionResult> DeActivateBusinessDirectory(long BDId)
        {
            var BD = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == BDId).FirstOrDefault();
            if (BD == null)
            {
                return Ok(new { Status = false, message = "Bussiness Directory Not Found" });
            }
            try
            {
                BD.IsActive = false;
                _dbContext.Attach(BD).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = false, message = "Bussiness Directory DeActivated" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = true, message = ex.Message });

            }

        }
        [HttpPut]
        [Route("DeActivateBusinessCategory")]
        public async Task<IActionResult> DeActivateBusinessCategory(long ProductCategoryId)
        {
            var BDProductCategory = _dbContext.ProductCategories.Where(c => c.ProductCategoryId == ProductCategoryId).FirstOrDefault();
            if (BDProductCategory == null)
            {
                return Ok(new { Status = false, message = "Bussiness Directory Product Category Not Found" });
            }
            try
            {
                BDProductCategory.Isactive = false;
                _dbContext.Attach(BDProductCategory).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = true, message = "Bussiness Directory Product Category DeActivated" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpPut]
        [Route("ActivateBusinessDirectory")]
        public async Task<IActionResult> ActivateBusinessDirectory(long BDId)
        {
            var BD = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == BDId).FirstOrDefault();
            if (BD == null)
            {
                return Ok(new { Status = false, message = "Bussiness Directory Not Found" });
            }
            try
            {
                BD.IsActive = true;
                _dbContext.Attach(BD).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = true, message = "Bussiness Directory Activated" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpPut]
        [Route("ActivateBusinessCategory")]
        public async Task<IActionResult> ActivateBusinessCategory(long ProductCategoryId)
        {
            var BDProductCategory = _dbContext.ProductCategories.Where(c => c.ProductCategoryId == ProductCategoryId).FirstOrDefault();
            if (BDProductCategory == null)
            {
                return Ok(new { Status = false, message = "Bussiness Directory Product Category Not Found" });
            }
            try
            {
                BDProductCategory.Isactive = true;
                _dbContext.Attach(BDProductCategory).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = true, message = "Bussiness Directory Product Category Activated" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetAllBDReviews")]
        public async Task<IActionResult> GetAllBDReviews(long BDId)
        {
            try
            {
                var BDObj = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == BDId).FirstOrDefault();
                if (BDObj == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Not Found" });

                }
                var BDReviews = _dbContext.Reviews.Where(c => c.ClassifiedBusinessId == BDId).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    Name = c.Name,
                    Rating = c.Rating,
                    Title = c.Title,
                    ReviewDate = c.ReviewDate,
                    ReviewId = c.ReviewId,
                    Email = c.Email,


                }).ToList();




                return Ok(new { Status = true, BDReviews = BDReviews });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpPut]
        //[Route("EditBusinessDirectory")]
        //public async Task<ActionResult> EditBusinessDirectory([FromForm] BusinessVM BusinessVM, IFormFile MainPic, List<BusinessMediaVm> mediaVms, List<WorkingHoursVM> Workinghoursvm)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(BusinessVM.UseId);
        //        if (user == null)
        //        {
        //            return Ok(new { Status = false, Message = "User Not Found" });
        //        }
        //        var plan = _dbContext.BussinessPlans.Where(e => e.BussinessPlanId == BusinessVM.PlanId).FirstOrDefault();

        //        if (plan == null)
        //        {
        //            return Ok(new { Status = false, Message = "Please,Select Plan" });
        //        }
        //        var BusinessCat = _dbContext.BusinessCategories.Find(BusinessVM.BusinessCategoryId);
        //        if (BusinessCat == null)
        //        {
        //            return Ok(new { Status = false, Message = "Category Not Found" });
        //        }
        //        ClassifiedBusiness classifiedBusiness = new ClassifiedBusiness()
        //        {
        //            IsActive = BusinessVM.IsActive,
        //            BusinessCategoryId = BusinessVM.BusinessCategoryId,
        //            PublishDate = DateTime.Now,
        //            UseId = user.Id,
        //            Title = BusinessVM.Title,
        //            Description = BusinessVM.Description
        //        };
        //        if (MainPic != null)
        //        {
        //            string folder = "Images/BusinessMedia/";
        //            classifiedBusiness.Mainpic = UploadImage(folder, MainPic);
        //        }
        //        List<BusinessWorkingHours> workingHours = new List<BusinessWorkingHours>();
        //        if (Workinghoursvm != null)
        //        {
        //            foreach (var item in Workinghoursvm)
        //            {
        //                Models.BusinessWorkingHours businessWorkingHoursobj = new Models.BusinessWorkingHours()
        //                {
        //                    StartTime1 = item.StartTime1,
        //                    StartTime2 = item.StartTime2,
        //                    EndTime1 = item.EndTime1,
        //                    EndTime2 = item.EndTime2,
        //                    Day = item.Day,
        //                    Isclosed = item.Isclosed,
        //                    ClassifiedBusinessId = item.ClassifiedBusinessId
        //                };

        //                workingHours.Add(businessWorkingHoursobj);
        //            }
        //        }
        //        classifiedBusiness.businessWorkingHours = workingHours;
        //        _dbContext.ClassifiedBusiness.Add(classifiedBusiness);
        //        _dbContext.SaveChanges();

        //        if (mediaVms != null)
        //        {
        //            if (mediaVms.Count != 0)
        //            {
        //                foreach (var item in mediaVms)
        //                {
        //                    var contentListMediaValue = new List<BusinessContentValue>();
        //                    for (int i = 0; i < item.Media.Count(); i++)
        //                    {
        //                        if (item.Media[i] != null)
        //                        {
        //                            string folder = "Images/BusinessMedia/";
        //                            var contentObj = new BusinessContentValue();
        //                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
        //                            contentListMediaValue.Add(contentObj);
        //                        }

        //                    }
        //                    var ContentValueObj = new BusinessContent()
        //                    {
        //                        ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId,
        //                        BusinessTemplateConfigId = item.BusinessTemplateConfigId,
        //                        BusinessContentValues = contentListMediaValue

        //                    };
        //                    _dbContext.BusinessContents.Add(ContentValueObj);
        //                    _dbContext.SaveChanges();

        //                }
        //            }
        //        }
        //        foreach (var item in BusinessVM.BusinessContentVMS)
        //        {
        //            var contentList = new List<BusinessContentValue>();

        //            foreach (var value in item.Values)
        //            {

        //                var contentObj = new BusinessContentValue()
        //                {
        //                    ContentValue = value,
        //                };
        //                contentList.Add(contentObj);
        //            }
        //            var ContentValue = new BusinessContent()
        //            {
        //                ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId,
        //                BusinessTemplateConfigId = item.BusinessTemplateConfigId,
        //                BusinessContentValues = contentList
        //            };
        //            _dbContext.BusinessContents.Add(ContentValue);
        //            _dbContext.SaveChanges();
        //        }
        //        double totalCost = plan.Price.Value;
        //        var subscription = new BusiniessSubscription();
        //        subscription.BussinessPlanId = BusinessVM.PlanId;
        //        subscription.ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId;
        //        subscription.Price = totalCost;
        //        subscription.StartDate = DateTime.Now;
        //        subscription.PaymentMethodId = BusinessVM.PaymentMethodId;
        //        subscription.EndDate = DateTime.Now.AddMonths(plan.DurationInMonth.Value);
        //        _dbContext.BusiniessSubscriptions.Add(subscription);
        //        _dbContext.SaveChanges();
        //        if (BusinessVM.PaymentMethodId == 1)
        //        {
        //            bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
        //            var TestToken = _configuration["TestToken"];
        //            var LiveToken = _configuration["LiveToken"];
        //            if (Fattorahstatus) // fattorah live
        //            {
        //                var sendPaymentRequest = new
        //                {

        //                    CustomerName = classifiedBusiness.Title,
        //                    NotificationOption = "LNK",
        //                    InvoiceValue = subscription.Price,
        //                    CallBackUrl = "https://albaheth.me/FattorahBusinessPlantSuccess",
        //                    ErrorUrl = "https://albaheth.me/FattorahBusinessPlanFalied",
        //                    UserDefinedField = subscription.BusiniessSubscriptionId
        //                };
        //                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //                string url = "https://api.myfatoorah.com/v2/SendPayment";
        //                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
        //                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //                var responseMessage = httpClient.PostAsync(url, httpContent);
        //                var res = await responseMessage.Result.Content.ReadAsStringAsync();
        //                var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


        //                if (FattoraRes.IsSuccess == true)
        //                {
        //                    Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
        //                    var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
        //                    return Ok(new { status = true, Message = "Business Subscription successfully!", paymentUrl = InvoiceRes.InvoiceURL });



        //                }
        //                else
        //                {

        //                    _dbContext.BusiniessSubscriptions.Remove(subscription);

        //                    return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });
        //                }
        //            }
        //            else               //fattorah test
        //            {
        //                var sendPaymentRequest = new
        //                {

        //                    CustomerName = classifiedBusiness.Title,
        //                    NotificationOption = "LNK",
        //                    InvoiceValue = subscription.Price,
        //                    CallBackUrl = "https://albaheth.me/FattorahBusinessPlantSuccess",
        //                    ErrorUrl = "https://albaheth.me/FattorahBusinessPlanFalied",
        //                    UserDefinedField = subscription.BusiniessSubscriptionId
        //                };
        //                var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

        //                string url = "https://apitest.myfatoorah.com/v2/SendPayment";
        //                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
        //                var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
        //                var responseMessage = httpClient.PostAsync(url, httpContent);
        //                var res = await responseMessage.Result.Content.ReadAsStringAsync();
        //                var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


        //                if (FattoraRes.IsSuccess == true)
        //                {
        //                    Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
        //                    var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
        //                    return Ok(new { status = true, Message = "Business Added Successfully!", paymentUrl = InvoiceRes.InvoiceURL });
        //                }
        //                else
        //                {

        //                    _dbContext.BusiniessSubscriptions.Remove(subscription);

        //                    return Ok(new { status = false, Message = "SomeThing went Error while Rquesting Payment Gateway !" });
        //                }
        //            }



        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }
        //    return Ok();
        //}


        [HttpGet]
        [Route("GetBannerList")]
        public IActionResult GetBannerList()
        {
            try
            {
                var BannerList = _dbContext.Banners.OrderBy(e => e.BannerOrderIndex).Where(c => c.BannerIsActive == true).Select(i => new
                {
                    BannerId = i.BannerId,
                    BannerPic = i.BannerPic,
                    EntityTypeId = i.EntityTypeId,
                    EntityId = i.EntityId,
                    LargePic = i.LargePic,

                }
                ).ToList();
                return Ok(new { Status = true, BannerList = BannerList });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("AddBDOffer")]
        public async Task<IActionResult> AddBDOffer(IFormFile Pic, IFormFileCollection Photos, [FromForm] BDOfferVm bDOfferVm)
        {
            try
            {
                var BDObj = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == bDOfferVm.ClassifiedBusinessId).FirstOrDefault();
                if (BDObj == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Not Found" });

                }
                bool SubscriptionIsFinished = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == bDOfferVm.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault() == null ? true : _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == bDOfferVm.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate < DateTime.Now ? true : false;
                if (SubscriptionIsFinished)
                {
                    return Ok(new { Status = false, Message = "You Must Subscribe ...Firstly" });

                }
                if (Pic == null)
                {
                    return Ok(new { Status = false, Message = "Offer Pic Is Required" });
                }
                var BDOffer = new BDOffer()
                {
                    ClassifiedBusinessId = bDOfferVm.ClassifiedBusinessId,
                    TitleAr = bDOfferVm.TitleAr,
                    TitleEn = bDOfferVm.TitleEn,
                    Price = bDOfferVm.Price,
                    OfferDescription = bDOfferVm.OfferDescription,
                    PublishDate = DateTime.Now,
                };
                BDOffer.Pic = UploadImage("Images/BDOffer/", Pic);

                if (Photos != null)
                {
                    List<BDOfferImage> BDOfferImages = new List<BDOfferImage>();
                    foreach (var item in Photos)
                    {
                        var BDOfferImageObj = new BDOfferImage();
                        BDOfferImageObj.Image = UploadImage("Images/BDOffer/", item);
                        BDOfferImages.Add(BDOfferImageObj);

                    }
                    BDOffer.BDOfferImages = BDOfferImages;

                }
                _dbContext.BDOffers.Add(BDOffer);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Offer Added Successfully" });



            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }

        [HttpPut]
        [Route("EditBDOffer")]
        public async Task<IActionResult> EditBDOffer(IFormFile Pic, IFormFileCollection MorePhotos, [FromForm] EditBDOfferVm editBDOfferVm)
        {
            try
            {
                var BDOfferObj = _dbContext.BDOffers.Where(e => e.BDOfferId == editBDOfferVm.BDOfferId).FirstOrDefault();
                if (BDOfferObj == null)
                {
                    return Ok(new { Status = false, Message = "Offer Not Found" });

                }
                BDOfferObj.Price = editBDOfferVm.Price;
                BDOfferObj.TitleAr = editBDOfferVm.TitleAr;
                BDOfferObj.TitleEn = editBDOfferVm.TitleEn;
                BDOfferObj.OfferDescription = editBDOfferVm.OfferDescription;


                if (Pic != null)
                {
                    BDOfferObj.Pic = UploadImage("Images/BDOffer/", Pic);
                }
                _dbContext.Attach(BDOfferObj).State = EntityState.Modified;
                if (MorePhotos != null)
                {
                    List<BDOfferImage> BDOfferImages = new List<BDOfferImage>();
                    foreach (var item in MorePhotos)
                    {
                        var BDOfferImageObj = new BDOfferImage();
                        BDOfferImageObj.Image = UploadImage("Images/BDOffer/", item);
                        BDOfferImageObj.BDOfferId = editBDOfferVm.BDOfferId;
                        BDOfferImages.Add(BDOfferImageObj);

                    }
                    _dbContext.BDOfferImages.AddRange(BDOfferImages);

                }

                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Offer Edited Successfully" });



            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteBDOffer")]
        public async Task<IActionResult> DeleteBDOffer(int BDOfferId)
        {
            try
            {
                var BDOfferObj = _dbContext.BDOffers.Where(e => e.BDOfferId == BDOfferId).FirstOrDefault();
                if (BDOfferObj == null)
                {
                    return Ok(new { Status = false, Message = "Offer Not Found" });

                }
                _dbContext.BDOffers.Remove(BDOfferObj);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Offer Deleted Successfully" });



            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteBDOfferImage")]
        public async Task<IActionResult> DeleteBDOfferImage(int BDOfferImageId)
        {
            try
            {
                var BDOfferImageObj = _dbContext.BDOfferImages.Where(e => e.BDOfferImageId == BDOfferImageId).FirstOrDefault();
                if (BDOfferImageObj == null)
                {
                    return Ok(new { Status = false, Message = "Offer Image Not Found" });

                }
                _dbContext.BDOfferImages.Remove(BDOfferImageObj);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Offer Image Deleted Successfully" });



            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetBDOfferById")]
        public IActionResult GetBDOfferById(int BDOfferId)
        {
            try
            {
                var BDOfferObj = _dbContext.BDOffers.Where(e => e.BDOfferId == BDOfferId).FirstOrDefault();
                if (BDOfferObj == null)
                {
                    return Ok(new { Status = false, Message = "Offer Not Found" });

                }
                var BDOffer = _dbContext.BDOffers.Include(e => e.ClassifiedBusiness).Include(e => e.BDOfferImages).Where(c => c.BDOfferId == BDOfferId).Select(i => new
                {
                    BDOfferId = i.BDOfferId,
                    TitleAr = i.TitleAr,
                    TitleEn = i.TitleEn,
                    OfferDescription = i.OfferDescription,
                    Price = i.Price,
                    PublishDate = i.PublishDate,
                    Pic = i.Pic,
                    Phone = i.ClassifiedBusiness.phone,
                    BDOfferImages = i.BDOfferImages.Select(c => new
                    {
                        c.BDOfferImageId,
                        c.Image,
                    }).ToList(),

                }
                ).FirstOrDefault();
                return Ok(new { Status = true, BDOffer = BDOffer });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetAllBDOffers")]
        public IActionResult GetAllBDOffers()
        {
            try
            {

                var BDOffers = _dbContext.BDOffers.Include(e => e.BDOfferImages).Select(i => new
                {
                    BDOfferId = i.BDOfferId,
                    TitleAr = i.TitleAr,
                    TitleEn = i.TitleEn,
                    OfferDescription = i.OfferDescription,
                    Price = i.Price,
                    PublishDate = i.PublishDate,
                    Pic = i.Pic,
                    BDOfferImages = i.BDOfferImages.Select(c => new
                    {
                        c.BDOfferImageId,
                        c.Image,
                    }).ToList(),

                }
                ).ToList();
                return Ok(new { Status = true, BDOffers = BDOffers });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetAllBDOffersByBDId")]
        public IActionResult GetAllBDOffersByBDId(long BDId)
        {
            try
            {
                var BDObj = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == BDId).FirstOrDefault();
                if (BDObj == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Not Found" });

                }

                var BDOffers = _dbContext.BDOffers.Include(e => e.BDOfferImages).Where(e => e.ClassifiedBusinessId == BDId).Select(i => new
                {
                    BDOfferId = i.BDOfferId,
                    TitleAr = i.TitleAr,
                    TitleEn = i.TitleEn,
                    OfferDescription = i.OfferDescription,
                    Price = i.Price,
                    PublishDate = i.PublishDate,
                    Pic = i.Pic,
                    BDOfferImages = i.BDOfferImages.Select(c => new
                    {
                        c.BDOfferImageId,
                        c.Image,
                    }).ToList(),

                }
                ).ToList();
                return Ok(new { Status = true, BDOffers = BDOffers });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetAllBDOffersByBDCatagoryId")]
        public IActionResult GetAllBDOffersByBDCatagoryId(int BDcatagoryId)
        {
            try
            {
                var catagoryObj = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryId == BDcatagoryId).FirstOrDefault();
                if (catagoryObj == null)
                {
                    return Ok(new { Status = false, Message = "Catagory Not Found" });

                }
                var catListChild = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryParentId == BDcatagoryId).Select(e => e.BusinessCategoryId).ToList();

                //catListChild.Contains(e.ClassifiedAdsCategoryId.Value)
                var BDOffers = _dbContext.BDOffers.Include(e => e.BDOfferImages).Include(e => e.ClassifiedBusiness).ThenInclude(e => e.BusinessCategory).Where(e => e.ClassifiedBusiness.BusinessCategoryId == BDcatagoryId || catListChild.Contains(e.ClassifiedBusiness.BusinessCategoryId.Value)).OrderByDescending(e => e.BDOfferId).Take(50).Select(i => new
                {
                    BDOfferId = i.BDOfferId,
                    TitleAr = i.TitleAr,
                    TitleEn = i.TitleEn,
                    OfferDescription = i.OfferDescription,
                    Price = i.Price,
                    PublishDate = i.PublishDate,
                    Pic = i.Pic,
                    BusinessCategoryId = i.ClassifiedBusiness.BusinessCategoryId,
                    BusinessCategoryTitleAr = i.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = i.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleEn,

                    BDOfferImages = i.BDOfferImages.Select(c => new
                    {
                        c.BDOfferImageId,
                        c.Image,
                    }).ToList(),

                }
                ).ToList();
                return Ok(new { Status = true, BDOffers = BDOffers });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetTopBDOffers")]
        public IActionResult GetTopBDOffers()
        {
            try
            {


                var BDOffers = _dbContext.BDOffers.Include(e => e.BDOfferImages).Include(e => e.ClassifiedBusiness).ThenInclude(e => e.BusinessCategory).OrderByDescending(e => e.BDOfferId).Take(50).Select(i => new
                {
                    BDOfferId = i.BDOfferId,
                    TitleAr = i.TitleAr,
                    TitleEn = i.TitleEn,
                    OfferDescription = i.OfferDescription,
                    Price = i.Price,
                    PublishDate = i.PublishDate,
                    Pic = i.Pic,
                    BusinessCategoryId = i.ClassifiedBusiness.BusinessCategoryId,
                    BusinessCategoryTitleAr = i.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = i.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleEn,

                    BDOfferImages = i.BDOfferImages.Select(c => new
                    {
                        c.BDOfferImageId,
                        c.Image,
                    }).ToList(),

                }
                ).ToList();
                return Ok(new { Status = true, BDOffers = BDOffers });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetLatestBusiness")]
        public async Task<IActionResult> GetLatestBusiness()
        {
            try
            {

                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Where(c => c.IsActive == true).Include(c => c.BusinessContents).ThenInclude(c => c.BusinessContentValues).OrderByDescending(c => c.ClassifiedBusinessId).Take(10).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Deliverycost = c.Deliverycost,
                    Rating = c.Rating,

                    UseId = c.UseId,
                    Views = c.Views,
                    SubscriptionIsFinished = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault() == null ? true : _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate < DateTime.Now ? true : false,


                }).ToListAsync();




                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetBusinessMostViews")]
        public async Task<IActionResult> GetBusinessMostViews()
        {
            try
            {

                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Where(c => c.IsActive == true).Include(c => c.BusinessContents).ThenInclude(c => c.BusinessContentValues).OrderByDescending(c => c.Views).Take(10).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Deliverycost = c.Deliverycost,
                    Rating = c.Rating,

                    UseId = c.UseId,
                    Views = c.Views,
                    SubscriptionIsFinished = _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault() == null ? true : _dbContext.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate < DateTime.Now ? true : false,


                }).ToListAsync();




                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetLatestClassifieds")]
        public async Task<IActionResult> GetLatestClassifieds()
        {
            try
            {

                var ClassifiedAds = _dbContext.ClassifiedAds.Where(c => c.IsActive == true).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).OrderByDescending(c => c.ClassifiedAdId).Take(10).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    MainPic = c.MainPic,
                    Description = c.Description,

                }).ToListAsync();




                return Ok(new { Status = true, ClassifiedAds = ClassifiedAds });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetClassifiedMostViews")]
        public async Task<IActionResult> GetClassifiedMostViews()
        {
            try
            {

                var ClassifiedAds = _dbContext.ClassifiedAds.Where(c => c.IsActive == true).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).OrderByDescending(c => c.Views).Take(10).Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    Views = c.Views,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Price = c.Price,
                    MainPic = c.MainPic,
                    Description = c.Description,


                }).ToListAsync();




                return Ok(new { Status = true, ClassifiedAds = ClassifiedAds });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetCategoriesLatestAds")]
        public async Task<IActionResult> GetCategoriesLatestAds()
        {
            try
            {
                //var catagoriesWithNoParent = _dbContext.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryParentId == null&&c.ClassifiedAdsCategoryIsActive==true).Select(e => e.ClassifiedAdsCategoryId).ToList();
                //var catList = _dbContext.SearchEntities.Where(e => catagoriesWithNoParent.Contains(e.ClassifiedAdsCatagoryId)).Select(e => e.SearchCatagoryLevel).ToList();
                //var classified = _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => catList.Contains(e.ClassifiedAdsCategoryId.Value)).Select(c => new
                //{
                //    ClassifiedAdId = c.ClassifiedAdId,
                //    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                //    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                //    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                //    IsActive = c.IsActive,
                //    PublishDate = c.PublishDate,
                //    UseId = c.UseId,

                //    Views = c.Views,
                //    TitleAr = c.TitleAr,
                //    TitleEn = c.TitleEn,
                //    Price = c.Price,
                //    MainPic = c.MainPic,
                //    Description = c.Description,
                //    City = c.CityId,
                //    Area = c.AreaId

                //}).ToList();

                var CategoriesLatestAds = _dbContext.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryParentId == null && c.ClassifiedAdsCategoryIsActive == true).OrderBy(c => c.ClassifiedAdsCategorySortOrder).Select(c => new
                {

                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryArabic = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryEnglish = c.ClassifiedAdsCategoryTitleEn,
                    //catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId==c.ClassifiedAdsCategoryId).Select(e => e.SearchCatagoryLevel).ToList()
                    Ads = _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Where(e => (_dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == c.ClassifiedAdsCategoryId).Select(e => e.SearchCatagoryLevel).ToList()).Contains(e.ClassifiedAdsCategoryId.Value)).OrderByDescending(c => c.PublishDate).Take(20).ToList()


                }).ToListAsync();

                return Ok(new { Status = true, CategoriesLatestAds = CategoriesLatestAds });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteAds")]
        public async Task<ActionResult> DeleteAds(long AdsId, string userid)
        {
            try
            {
                var AdsObj = _dbContext.ClassifiedAds.Where(e => e.ClassifiedAdId == AdsId && e.UseId == userid).FirstOrDefault();
                if (AdsObj == null)
                {
                    return Ok(new { Status = false, Message = "Ads Not Found" });
                }


                var AdsContentList = _dbContext.AdContents.Where(e => e.ClassifiedAdId == AdsObj.ClassifiedAdId).ToList();
                var AdsNewContentList = AdsContentList.Select(e => e.AdContentId).ToList();
                if (AdsContentList != null)
                {
                    var ContentValues = _dbContext.AdContentValues.Where(e => AdsNewContentList.Contains(e.AdContentId)).ToList();
                    _dbContext.AdContentValues.RemoveRange(ContentValues);
                }
                var ImageList = _dbContext.AdsImages.Where(e => e.ClassifiedAdId == AdsObj.ClassifiedAdId).ToList();
                if (ImageList != null)
                {
                    _dbContext.AdsImages.RemoveRange(ImageList);
                }

                _dbContext.AdContents.RemoveRange(AdsContentList);
                _dbContext.ClassifiedAds.Remove(AdsObj);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Classified Ads Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }


        [HttpGet]
        [Route("GetNewClassifiedAdzCategories")]
        public async Task<ActionResult> GetNewClassifiedAdzCategories()
        {
            try
            {

                var categories = _dbContext.ClassifiedAdsCategories.ToList();

                // get top-level categories
                var parents = categories.Where(c => c.ClassifiedAdsCategoryParentId == null);

                // map top-level categories and their children to DTOs
                var dtos = parents.Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                    ClassifiedAdsCategoryDescEn = c.ClassifiedAdsCategoryDescEn,
                    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                    Children = categories.Where(a => a.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId)
                                         .Select(b => new
                                         {
                                             ClassifiedAdsCategoryId = b.ClassifiedAdsCategoryId,
                                             ClassifiedAdsCategoryTitleAr = b.ClassifiedAdsCategoryTitleAr,
                                             ClassifiedAdsCategoryTitleEn = b.ClassifiedAdsCategoryTitleEn,
                                             ClassifiedAdsCategorySortOrder = b.ClassifiedAdsCategorySortOrder,
                                             ClassifiedAdsCategoryPic = b.ClassifiedAdsCategoryPic,
                                             ClassifiedAdsCategoryIsActive = b.ClassifiedAdsCategoryIsActive,
                                             ClassifiedAdsCategoryDescAr = b.ClassifiedAdsCategoryDescAr,
                                             ClassifiedAdsCategoryDescEn = b.ClassifiedAdsCategoryDescEn,
                                             ClassifiedAdsCategoryParentId = b.ClassifiedAdsCategoryParentId,
                                             Children = categories.Where(M => M.ClassifiedAdsCategoryParentId == b.ClassifiedAdsCategoryId)
                                                                  .Select(gc => new
                                                                  {
                                                                      ClassifiedAdsCategoryId = gc.ClassifiedAdsCategoryId,
                                                                      ClassifiedAdsCategoryTitleAr = gc.ClassifiedAdsCategoryTitleAr,
                                                                      ClassifiedAdsCategoryTitleEn = gc.ClassifiedAdsCategoryTitleEn,
                                                                      ClassifiedAdsCategorySortOrder = gc.ClassifiedAdsCategorySortOrder,
                                                                      ClassifiedAdsCategoryPic = gc.ClassifiedAdsCategoryPic,
                                                                      ClassifiedAdsCategoryIsActive = gc.ClassifiedAdsCategoryIsActive,
                                                                      ClassifiedAdsCategoryDescAr = gc.ClassifiedAdsCategoryDescAr,
                                                                      ClassifiedAdsCategoryDescEn = gc.ClassifiedAdsCategoryDescEn,
                                                                      ClassifiedAdsCategoryParentId = gc.ClassifiedAdsCategoryParentId,
                                                                      Children = categories.Where(K => K.ClassifiedAdsCategoryParentId == gc.ClassifiedAdsCategoryId)
                                                                  .Select(gcM => new
                                                                  {
                                                                      ClassifiedAdsCategoryId = gcM.ClassifiedAdsCategoryId,
                                                                      ClassifiedAdsCategoryTitleAr = gcM.ClassifiedAdsCategoryTitleAr,
                                                                      ClassifiedAdsCategoryTitleEn = gcM.ClassifiedAdsCategoryTitleEn,
                                                                      ClassifiedAdsCategorySortOrder = gcM.ClassifiedAdsCategorySortOrder,
                                                                      ClassifiedAdsCategoryPic = gcM.ClassifiedAdsCategoryPic,
                                                                      ClassifiedAdsCategoryIsActive = gcM.ClassifiedAdsCategoryIsActive,
                                                                      ClassifiedAdsCategoryDescAr = gcM.ClassifiedAdsCategoryDescAr,
                                                                      ClassifiedAdsCategoryDescEn = gcM.ClassifiedAdsCategoryDescEn,
                                                                      ClassifiedAdsCategoryParentId = gcM.ClassifiedAdsCategoryParentId,
                                                                      //    Children = categories.Where(FM => FM.ClassifiedAdsCategoryParentId == gcM.ClassifiedAdsCategoryId)
                                                                      //.Select(gcMF => new
                                                                      //{
                                                                      //    ClassifiedAdsCategoryId = gcMF.ClassifiedAdsCategoryId,
                                                                      //    ClassifiedAdsCategoryTitleAr = gcMF.ClassifiedAdsCategoryTitleAr,
                                                                      //    ClassifiedAdsCategoryTitleEn = gcMF.ClassifiedAdsCategoryTitleEn,
                                                                      //    ClassifiedAdsCategorySortOrder = gcMF.ClassifiedAdsCategorySortOrder,
                                                                      //    ClassifiedAdsCategoryPic = gcMF.ClassifiedAdsCategoryPic,
                                                                      //    ClassifiedAdsCategoryIsActive = gcMF.ClassifiedAdsCategoryIsActive,
                                                                      //    ClassifiedAdsCategoryDescAr = gcMF.ClassifiedAdsCategoryDescAr,
                                                                      //    ClassifiedAdsCategoryDescEn = gcMF.ClassifiedAdsCategoryDescEn,
                                                                      //    ClassifiedAdsCategoryParentId = gcMF.ClassifiedAdsCategoryParentId,


                                                                      //})


                                                                  })

                                                                  })

                                         })
                });



                //var Data = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == null).ToList();
                //foreach (var item in Data)
                //{

                //};

                //var Data = _dbContext.ClassifiedAdsCategories.Where(e=>e.ClassifiedAdsCategoryParentId==null).Select(c => new
                //{
                //    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                //    ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategoryTitleAr,
                //    ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategoryTitleEn,
                //    ClassifiedAdsCategorySortOrder = c.ClassifiedAdsCategorySortOrder,
                //    ClassifiedAdsCategoryPic = c.ClassifiedAdsCategoryPic,
                //    ClassifiedAdsCategoryIsActive = c.ClassifiedAdsCategoryIsActive,
                //    ClassifiedAdsCategoryDescAr = c.ClassifiedAdsCategoryDescAr,
                //    ClassifiedAdsCategoryDescEn = c.ClassifiedAdsCategoryDescEn,
                //    ClassifiedAdsCategoryParentId = c.ClassifiedAdsCategoryParentId,
                //    InverseClassifiedAdsCategoryParent = _dbContext.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId ).ToList(),
                //    //HasChild = _dbContext.ClassifiedAdsCategories.Any(x => x.ClassifiedAdsCategoryParentId == c.ClassifiedAdsCategoryId),

                //}).ToListAsync();

                //if (Data is null)
                //{
                //    return NotFound();
                //}

                return Ok(new { Status = true, dtos, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        [HttpGet]
        [Route("GetAllCities")]
        public IActionResult GetAllCities()
        {
            try
            {
                var Cities = _dbContext.Cities.Include(c => c.Area.Where(c => c.AreaIsActive == true)).Where(c => c.CityIsActive == true).OrderBy(c => c.CityOrderIndex).ToList();
                return Ok(new { Status = true, Cities = Cities });
            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetAllAreas")]
        public IActionResult GetAllAreas()
        {
            try
            {
                var Areas = _dbContext.Areas.Where(c => c.AreaIsActive == true && c.City.CityIsActive == true).OrderBy(c => c.AreaOrderIndex).ToList();
                return Ok(new { Status = true, Areas = Areas });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        [HttpDelete]
        [Route("DeletePhoto")]
        public IActionResult DeletePhoto(int PhotoId)
        {
            try
            {
                var Photo = _applicationDbContext.Photos.Where(a => a.PhotoID == PhotoId).FirstOrDefault();
                _applicationDbContext.Photos.Remove(Photo);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = true, message = "Photo Succesfully deleted" });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteVideo")]
        public IActionResult DeleteVideo(int VideoId)
        {
            try
            {
                var Video = _applicationDbContext.Videos.Where(a => a.VideoID == VideoId).FirstOrDefault();
                _applicationDbContext.Videos.Remove(Video);
                _applicationDbContext.SaveChanges();
                return Ok(new { Status = true, message = "Video Succesfully deleted" });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }
        [HttpGet]
        [Route("GetOptionChilds")]
        public async Task<IActionResult> GetOptionChilds(int AdTemplateOptionId)
        {
            try
            {

                var Options = _dbContext.AdTemplateOptions.Where(c => c.ParentId == AdTemplateOptionId).Select(c => new
                {

                    AdTemplateOptionId = c.AdTemplateOptionId,
                    AdTemplateConfigId = c.AdTemplateConfigId,
                    OptionEn = c.OptionEn,
                    OptionAr = c.OptionAr,
                    ParentId = c.ParentId,


                }).ToListAsync();




                return Ok(new { Status = true, Options = Options });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetCheckVersion")]
        public async Task<IActionResult> GetCheckVersion(string version)
        {
            if (version == null)
            {
                return Ok(new { Status = false, message = "Enter Valid Version" });
            }

            try
            {
                //var version = _dbContext.AppVersions.OrderByDescending(e => e.AppVersionId).FirstOrDefault();
                var versionCheck = _dbContext.AppVersions.OrderByDescending(e => e.AppVersionId).FirstOrDefault();
                bool IsUpdated = false;
                if (versionCheck != null)
                {
                    if (versionCheck.Version == version)
                    {
                        IsUpdated = true;
                        return Ok(new { Status = true, IsUpdated = IsUpdated, Skip = versionCheck.Skip, Message = "App Currently Updated" });
                    }

                    return Ok(new { Status = false, IsUpdated = IsUpdated, Skip = versionCheck.Skip, Message = "App Need To Update To Latest Version" });

                }
                return Ok(new { Status = false, Message = "App Need To Update To Latest Version" });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }
        }

        #region BDAdminDashboard
        [HttpGet]
        [Route("GetBDCards")]
        public async Task<IActionResult> GetBDCards(long BDId)
        {
            var BD = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == BDId).FirstOrDefault();
            if (BD == null)
            {
                return Ok(new { Status = false, message = "Bussiness Directory Not Found" });
            }
            try
            {
                var ProductCatagoryCount = _dbContext.ProductCategories.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var OffersCount = _dbContext.BDOffers.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var ProductsCount = _dbContext.Products.Include(e => e.ProductCategory).Where(e => e.ProductCategory.ClassifiedBusinessId == BDId).Count();
                var OrdersCount = _dbContext.Orders.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var OrdersAmount = _dbContext.Orders.Where(e => e.ClassifiedBusinessId == BDId).Sum(e => e.OrderTotal);
                return Ok(new { Status = true, BDViews = BD.Views, ProductCatagoryCount = ProductCatagoryCount, OffersCount = OffersCount, ProductsCount = ProductsCount, OrdersCount = OrdersCount, OrdersAmount = OrdersAmount });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = true, message = ex.Message });

            }

        }
        #region Services
        [HttpPost]
        [Route("AddServiceCategory")]
        public async Task<ActionResult> AddServiceCategory([FromForm] ServiceCatagoryVm serviceCatagoryVm, IFormFile catgoryPic)
        {
            try
            {
                var BD = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == serviceCatagoryVm.ClassifiedBusinessId).FirstOrDefault();
                if (BD == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Directory Not Found" });
                }
                var serviceCatagory = new ServiceCatagory()
                {

                    ServiceCatagoryTitleAr = serviceCatagoryVm.ServiceCatagoryTitleAr,
                    ServiceCatagoryTitleEn = serviceCatagoryVm.ServiceCatagoryTitleEn,
                    ClassifiedBusinessId = serviceCatagoryVm.ClassifiedBusinessId,
                    IsActive = serviceCatagoryVm.IsActive,
                    SortOrder = serviceCatagoryVm.SortOrder
                };
                if (catgoryPic != null)
                {
                    string folder = "Images/ServiceCategory/";
                    serviceCatagory.Pic = UploadImage(folder, catgoryPic);
                }
                _dbContext.ServiceCatagories.Add(serviceCatagory);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Category Added Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpPut]
        [Route("EditServiceCategory")]
        public async Task<ActionResult> EditServiceCategory([FromForm] EditServiceCatagoryVm editServiceCatagoryVm, IFormFile CatgoryPic)
        {
            try
            {
                var serviceCategory = _dbContext.ServiceCatagories.Where(e => e.ServiceCatagoryId == editServiceCatagoryVm.ServiceCatagoryId).FirstOrDefault();
                if (serviceCategory == null)
                {
                    return Ok(new { Status = false, Message = "Service Category Not Found" });
                }


                serviceCategory.ServiceCatagoryTitleAr = editServiceCatagoryVm.ServiceCatagoryTitleAr;
                serviceCategory.ServiceCatagoryTitleEn = editServiceCatagoryVm.ServiceCatagoryTitleEn;
                serviceCategory.SortOrder = editServiceCatagoryVm.SortOrder;
                serviceCategory.IsActive = editServiceCatagoryVm.IsActive;


                if (CatgoryPic != null)
                {
                    string folder = "Images/ServiceCategory/";
                    serviceCategory.Pic = UploadImage(folder, CatgoryPic);
                }
                _dbContext.Attach(serviceCategory).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Category Edited Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteServiceCategory")]
        public async Task<ActionResult> DeleteServiceCategory(long serviceCategoryId)
        {
            try
            {
                var serviceCategory = _dbContext.ServiceCatagories.Where(e => e.ServiceCatagoryId == serviceCategoryId).FirstOrDefault();
                if (serviceCategory == null)
                {
                    return Ok(new { Status = false, Message = "Service Category Not Found" });
                }
                var serviceList = _dbContext.Services.Where(e => e.ServiceCatagoryId == serviceCategoryId).ToList();


                foreach (var item in serviceList)
                {

                    var AdContentList = _dbContext.ServiceContents.Where(e => e.ServiceContentId == item.ServiceId).ToList();
                    var newAdContentList = AdContentList.Select(e => e.ServiceContentId).ToList();
                    if (AdContentList != null)
                    {
                        var ContentValues = _dbContext.ServiceContentValues.Where(e => newAdContentList.Contains(e.ServiceContentId)).ToList();
                        _dbContext.ServiceContentValues.RemoveRange(ContentValues);
                    }
                    _dbContext.ServiceContents.RemoveRange(AdContentList);
                    var imageList = _dbContext.ServiceImages.Where(e => e.ServiceId == item.ServiceId).ToList();
                    if (imageList != null)
                    {
                        _dbContext.ServiceImages.RemoveRange(imageList);
                    }
                    _dbContext.Services.Remove(item);
                }
                _dbContext.ServiceCatagories.Remove(serviceCategory);
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Service Category Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetServiceCategoryById")]
        public ActionResult GetServiceCategoryById(long serviceCategoryId)
        {
            try
            {

                var serviceCategory = _dbContext.ServiceCatagories.Where(c => c.ServiceCatagoryId == serviceCategoryId).Select(c => new
                {
                    ServiceCatagoryId = c.ServiceCatagoryId,
                    ServiceCatagoryTitleAr = c.ServiceCatagoryTitleAr,
                    ServiceCatagoryTitleEn = c.ServiceCatagoryTitleEn,
                    SortOrder = c.SortOrder,
                    Pic = c.Pic,
                    IsActive = c.IsActive,
                }).FirstOrDefault();
                if (serviceCategory is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Category" });

                }


                return Ok(new { Status = true, ServiceCategory = serviceCategory });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetAllServiceCategoriesByBussinessId")]
        public ActionResult GetAllServiceCategoriesByBussinessId(long BussinessId, string userId)
        {
            try
            {

                var Category = _dbContext.ServiceCatagories.Include(e => e.Services).Where(c => c.ClassifiedBusinessId == BussinessId && c.IsActive == true).OrderBy(e => e.SortOrder).Select(c => new
                {
                    ServiceCatagoryId = c.ServiceCatagoryId,
                    ServiceCatagoryTitleAr = c.ServiceCatagoryTitleAr,
                    ServiceCatagoryTitleEn = c.ServiceCatagoryTitleEn,
                    SortOrder = c.SortOrder,
                    Pic = c.Pic,
                    IsActive = c.IsActive,
                    Services = c.Services.Select(k => new
                    {
                        ServiceId = k.ServiceId,
                        ServiceCatagoryId = k.ServiceCatagoryId,
                        ServiceTitleAr = k.ServiceTitleAr,
                        ServiceTitleEn = k.ServiceTitleEn,
                        Price = k.Price,
                        MainPic = k.MainPic,
                        Description = k.Description,
                        IsFavourite = _dbContext.ServiceFavourites.Any(o => o.ServiceId == k.ServiceId && o.UserId == userId),

                    }).ToList(),
                }).ToList();
                if (Category is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Category" });

                }


                return Ok(new { Status = true, Category });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetAllServiceTypes")]
        public ActionResult GetAllServiceTypes()
        {
            try
            {

                var serviceTypes = _dbContext.ServiceTypes.Select(c => new
                {
                    ServiceTypeId = c.ServiceTypeId,
                    ServiceTypeTitleAr = c.ServiceTypeTitleAr,
                    ServiceTypeTitleEn = c.ServiceTypeTitleEn,
                    Pic = c.Pic,
                }).ToList();
                if (serviceTypes is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Service Types" });

                }


                return Ok(new { Status = true, ServiceTypes = serviceTypes });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetServiceCofigByServiceTypeId")]
        public ActionResult GetServiceCofigByServiceTypeId(int ServiceTypeId)
        {
            try
            {

                var serviceType = _dbContext.ServiceTypes.Where(c => c.ServiceTypeId == ServiceTypeId).FirstOrDefault();

                if (serviceType is null)
                {
                    return Ok(new { Status = false, Message = "Servcie Type Not Found" });

                }
                var config = _dbContext.ServiceTemplateConfigs.Include(e => e.ServiceTemplateOptions).Where(e => e.ServiceTypeId == ServiceTypeId).ToList();
                return Ok(new { Status = true, config });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPost]
        [Route("AddService")]
        public async Task<ActionResult> AddService([FromForm] ServiceVm serviceVm, List<ServiceMediaVm> mediaVms, IFormFile mainPic, IFormFile Reel, IFormFileCollection ServiceImages)
        {
            try
            {

                var serviceCatagory = _dbContext.ServiceCatagories.Find(serviceVm.ServiceCatagoryId);
                if (serviceCatagory == null)
                {
                    return Ok(new { Status = false, Message = "Service Category Not Found" });
                }
                var ServiceType = _dbContext.ServiceTypes.Find(serviceVm.ServiceTypeId);
                if (ServiceType == null)
                {
                    return Ok(new { Status = false, Message = "Service Type Not Found" });
                }

                Service service = new Service()
                {
                    ServiceTitleAr = serviceVm.ServiceTitleAr,
                    ServiceTitleEn = serviceVm.ServiceTitleEn,
                    Price = serviceVm.Price,
                    ServiceCatagoryId = serviceVm.ServiceCatagoryId,
                    SortOrder = serviceVm.SortOrder,
                    ServiceTypeId = serviceVm.ServiceTypeId,
                    IsActive = serviceVm.IsActive,
                    Description = serviceVm.Description,
                };
                if (mainPic != null)
                {
                    string folder = "Images/ServiceMedia/";
                    service.MainPic = UploadImage(folder, mainPic);

                }
                if (Reel != null)
                {
                    string folder = "Images/ServiceMedia/";
                    service.Reel = UploadImage(folder, Reel);

                }
                if (ServiceImages != null)
                {
                    List<ServiceImage> sevicesImages = new List<ServiceImage>();
                    foreach (var item in ServiceImages)
                    {
                        var ServiceImageObj = new ServiceImage();
                        ServiceImageObj.Image = UploadImage("Images/ServiceMedia/", item);
                        sevicesImages.Add(ServiceImageObj);

                    }
                    service.ServiceImages = sevicesImages;

                }


                _dbContext.Services.Add(service);
                _dbContext.SaveChanges();
                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var item in mediaVms)
                        {
                            var contentListMediaValue = new List<ServiceContentValue>();
                            for (int i = 0; i < item.Media.Count(); i++)
                            {
                                if (item.Media[i] != null)
                                {
                                    string folder = "Images/ServiceMedia/";
                                    var contentObj = new ServiceContentValue();
                                    contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                    contentListMediaValue.Add(contentObj);
                                }

                            }
                            var ServiceContentObj = new ServiceContent()
                            {
                                ServiceId = service.ServiceId,
                                ServiceTemplateConfigId = item.ServiceTemplateConfigId,
                                ServiceContentValues = contentListMediaValue

                            };
                            _dbContext.ServiceContents.Add(ServiceContentObj);
                            _dbContext.SaveChanges();

                        }
                    }
                }
                if (serviceVm.serviceContentVms != null)
                {
                    foreach (var item in serviceVm.serviceContentVms)
                    {
                        var contentValList = new List<ServiceContentValue>();

                        foreach (var value in item.Values)
                        {

                            var contentValObj = new ServiceContentValue()
                            {
                                ContentValue = value,
                            };
                            contentValList.Add(contentValObj);
                        }
                        var ServiceContentObj = new ServiceContent()
                        {
                            ServiceId = service.ServiceId,
                            ServiceTemplateConfigId = item.ServiceTemplateConfigId,
                            ServiceContentValues = contentValList
                        };
                        _dbContext.ServiceContents.Add(ServiceContentObj);
                        _dbContext.SaveChanges();
                    }
                }

                return Ok(new { Status = true, Message = "Service Added Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPut]
        [Route("EditService")]
        public async Task<ActionResult> EditService([FromForm] EditServiceVm serviceVm, List<ServiceMediaVm> mediaVms, IFormFile mainPic, IFormFile Reel, IFormFileCollection ServiceImages)
        {
            try
            {
                var serviceModel = _dbContext.Services.Where(e => e.ServiceId == serviceVm.ServiceId).FirstOrDefault();
                if (serviceModel == null)
                {
                    return Ok(new { Status = false, Message = "Service Not Found" });
                }
                var serviceCatagory = _dbContext.ServiceCatagories.Find(serviceVm.ServiceCatagoryId);
                if (serviceCatagory == null)
                {
                    return Ok(new { Status = false, Message = "Service Category Not Found" });
                }
                var ServiceType = _dbContext.ServiceTypes.Find(serviceVm.ServiceTypeId);
                if (ServiceType == null)
                {
                    return Ok(new { Status = false, Message = "Service Type Not Found" });
                }

                serviceModel.ServiceTitleAr = serviceVm.ServiceTitleAr;
                serviceModel.ServiceTitleEn = serviceVm.ServiceTitleEn;
                serviceModel.Description = serviceVm.Description;
                serviceModel.Price = serviceVm.Price;
                serviceModel.ServiceCatagoryId = serviceVm.ServiceCatagoryId;
                serviceModel.SortOrder = serviceVm.SortOrder;
                serviceModel.ServiceTypeId = serviceVm.ServiceTypeId;
                serviceModel.IsActive = serviceVm.IsActive;

                if (mainPic != null)
                {
                    string folder = "Images/ServiceMedia/";
                    serviceModel.MainPic = UploadImage(folder, mainPic);

                }
                if (Reel != null)
                {
                    string folder = "Images/ServiceMedia/";
                    serviceModel.Reel = UploadImage(folder, Reel);

                }
                _dbContext.Attach(serviceModel).State = EntityState.Modified;
                if (ServiceImages != null)
                {
                    List<ServiceImage> sevicesImages = new List<ServiceImage>();
                    foreach (var item in ServiceImages)
                    {
                        var ServiceImageObj = new ServiceImage();
                        ServiceImageObj.Image = UploadImage("Images/ServiceMedia/", item);
                        ServiceImageObj.ServiceId = serviceVm.ServiceId;
                        sevicesImages.Add(ServiceImageObj);

                    }
                    _dbContext.ServiceImages.AddRange(sevicesImages);

                }
                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var item in mediaVms)
                        {
                            var serviceContentModel = _dbContext.ServiceContents.Where(e => e.ServiceId == serviceVm.ServiceId && e.ServiceTemplateConfigId == item.ServiceTemplateConfigId).FirstOrDefault();
                            if (serviceContentModel != null)
                            {
                                if (item.Media.Count() != 0)
                                {
                                    var contentListMediaValue = new List<ServiceContentValue>();
                                    for (int i = 0; i < item.Media.Count(); i++)
                                    {
                                        if (item.Media[i] != null)
                                        {
                                            string folder = "Images/ServiceMedia/";
                                            var contentObj = new ServiceContentValue();
                                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                            contentObj.ServiceContentId = serviceContentModel.ServiceContentId;
                                            contentListMediaValue.Add(contentObj);
                                        }

                                    }

                                    _dbContext.ServiceContentValues.AddRange(contentListMediaValue);

                                    //var serContentValues = _dbContext.ServiceContentValues.Where(e => e.ServiceContentId == serviceContentModel.ServiceContentId).ToList();
                                    //if (serContentValues != null)
                                    //{

                                    //    _dbContext.RemoveRange(serContentValues);
                                    //}

                                }


                            }
                            else
                            {
                                if (item.Media.Count() != 0)
                                {
                                    var contentListMediaValue = new List<ServiceContentValue>();
                                    for (int i = 0; i < item.Media.Count(); i++)
                                    {
                                        if (item.Media[i] != null)
                                        {
                                            string folder = "Images/ServiceMedia/";
                                            var contentObj = new ServiceContentValue();
                                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                            contentListMediaValue.Add(contentObj);
                                        }

                                    }
                                    var ServiceContentObj = new ServiceContent()
                                    {
                                        ServiceId = serviceVm.ServiceId,
                                        ServiceTemplateConfigId = item.ServiceTemplateConfigId,
                                        ServiceContentValues = contentListMediaValue

                                    };
                                    _dbContext.ServiceContents.Add(ServiceContentObj);
                                }

                            }


                        }
                    }
                }
                if (serviceVm.serviceContentVms != null)
                {
                    if (serviceVm.serviceContentVms.Count != 0)
                    {
                        foreach (var item in serviceVm.serviceContentVms)
                        {
                            var serviceContentModel = _dbContext.ServiceContents.Where(e => e.ServiceId == serviceVm.ServiceId && e.ServiceTemplateConfigId == item.ServiceTemplateConfigId).FirstOrDefault();
                            if (serviceContentModel != null)
                            {
                                var fieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == serviceContentModel.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId;
                                if (fieldTypeId == 6)
                                {
                                    var serContentValues = _dbContext.ServiceContentValues.Where(e => e.ServiceContentId == serviceContentModel.ServiceContentId).ToList();
                                    if (serContentValues != null)
                                    {
                                        _dbContext.RemoveRange(serContentValues);
                                    }
                                    if (item.Values != null)
                                    {
                                        if (item.Values.Count != 0)
                                        {
                                            var contentValList = new List<ServiceContentValue>();

                                            foreach (var value in item.Values)
                                            {

                                                var contentValObj = new ServiceContentValue()
                                                {
                                                    ContentValue = value,
                                                    ServiceContentId = serviceContentModel.ServiceContentId
                                                };
                                                contentValList.Add(contentValObj);
                                            }

                                            _dbContext.ServiceContentValues.AddRange(contentValList);

                                        }

                                    }



                                }
                                else
                                {
                                    var serContentValueModel = _dbContext.ServiceContentValues.Where(e => e.ServiceContentId == serviceContentModel.ServiceContentId).FirstOrDefault();
                                    serContentValueModel.ContentValue = item.Values.FirstOrDefault();
                                    _dbContext.Attach(serContentValueModel).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                var contentValList = new List<ServiceContentValue>();

                                foreach (var value in item.Values)
                                {

                                    var contentValObj = new ServiceContentValue()
                                    {
                                        ContentValue = value,
                                    };
                                    contentValList.Add(contentValObj);
                                }
                                var ServiceContentObj = new ServiceContent()
                                {
                                    ServiceId = serviceVm.ServiceId,
                                    ServiceTemplateConfigId = item.ServiceTemplateConfigId,
                                    ServiceContentValues = contentValList
                                };
                                _dbContext.ServiceContents.Add(ServiceContentObj);
                                //var serContentValues = _dbContext.ServiceContentValues.Where(e => e.ServiceContentId == serviceContentModel.ServiceContentId).ToList();
                                //if (serContentValues != null)
                                //{
                                //    _dbContext.RemoveRange(serContentValues);
                                //}
                            }





                        }


                    }
                }
                _dbContext.SaveChanges();

                return Ok(new { Status = true, Message = "Service Updated Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpDelete]
        [Route("DeleteServiceImage")]
        public async Task<ActionResult> DeleteServiceImage(long serviceImageId)
        {
            try
            {
                var serviceImage = _dbContext.ServiceImages.Where(e => e.ServiceImageId == serviceImageId).FirstOrDefault();
                if (serviceImage == null)
                {
                    return Ok(new { Status = false, Message = "Service Image Not Found" });
                }
                _dbContext.ServiceImages.Remove(serviceImage);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Image Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteServiceDynamicMedia")]
        public async Task<ActionResult> DeleteServiceDynamicMedia(int contentValueId)
        {
            try
            {
                var serviceContentValue = _dbContext.ServiceContentValues.Where(e => e.ServiceContentValueId == contentValueId).FirstOrDefault();
                if (serviceContentValue == null)
                {
                    return Ok(new { Status = false, Message = "Media Not Found" });

                }
                _dbContext.ServiceContentValues.Remove(serviceContentValue);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Media Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteBDDynamicMedia")]
        public async Task<ActionResult> DeleteBDDynamicMedia(int contentValueId)
        {
            try
            {
                var BDContentValue = _dbContext.BusinessContentValues.Where(e => e.BusinessContentValueId == contentValueId).FirstOrDefault();
                if (BDContentValue == null)
                {
                    return Ok(new { Status = false, Message = "Media Not Found" });

                }
                _dbContext.BusinessContentValues.Remove(BDContentValue);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Business Media Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteService")]
        public async Task<ActionResult> DeleteService(long serviceId)
        {
            try
            {
                var service = _dbContext.Services.Where(e => e.ServiceId == serviceId).FirstOrDefault();
                if (service == null)
                {
                    return Ok(new { Status = false, Message = "Service Not Found" });
                }


                var AdContentList = _dbContext.ServiceContents.Where(e => e.ServiceContentId == service.ServiceId).ToList();
                var newAdContentList = AdContentList.Select(e => e.ServiceContentId).ToList();
                if (AdContentList != null)
                {
                    var ContentValues = _dbContext.ServiceContentValues.Where(e => newAdContentList.Contains(e.ServiceContentId)).ToList();
                    _dbContext.ServiceContentValues.RemoveRange(ContentValues);
                }
                _dbContext.ServiceContents.RemoveRange(AdContentList);
                var imageList = _dbContext.ServiceImages.Where(e => e.ServiceId == service.ServiceId).ToList();
                if (imageList != null)
                {
                    _dbContext.ServiceImages.RemoveRange(imageList);
                }
                //var serviceQuotations = _dbContext.ServiceQuotations.Where(e => e.ServiceId == service.ServiceId).ToList();
                //if (serviceQuotations != null)
                //{
                //    _dbContext.ServiceQuotations.RemoveRange(serviceQuotations);
                //}
                _dbContext.Services.Remove(service);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetServiceDetailsById")]
        public async Task<IActionResult> GetServiceDetailsById(int ServiceId, string userId)
        {
            try
            {


                var service = _dbContext.Services.Include(c => c.ServiceImages).Include(c => c.ServiceCatagory).ThenInclude(c => c.ClassifiedBusiness).Where(c => c.ServiceId == ServiceId).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceCatagoryId = c.ServiceCatagoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    ServiceTitleAr = c.ServiceTitleAr,
                    ServiceTitleEn = c.ServiceTitleEn,
                    ServiceTypeId = c.ServiceTypeId,
                    MainPic = c.MainPic,
                    Description = c.Description,
                    Reel = c.Reel,
                    ServiceCatagoryTitleAr = c.ServiceCatagory.ServiceCatagoryTitleAr,
                    ServiceCatagoryTitleEn = c.ServiceCatagory.ServiceCatagoryTitleEn,
                    UseId = c.ServiceCatagory.ClassifiedBusiness.UseId,
                    BDId = c.ServiceCatagory.ClassifiedBusinessId,

                    IsFavourite = _dbContext.ServiceFavourites.Any(o => o.ServiceId == ServiceId && o.UserId == userId),

                    ServiceImages = c.ServiceImages.Select(i => new
                    {
                        ServiceImageId = i.ServiceImageId,
                        Image = i.Image
                    }).ToList(),


                    ServiceContents = _dbContext.ServiceContents.Where(e => e.ServiceId == c.ServiceId).Select(l => new
                    {
                        ServiceContentId = l.ServiceContentId,
                        ServiceTemplateConfigId = l.ServiceTemplateConfigId,
                        ServiceContentValues = l.ServiceContentValues.Select(k => new
                        {
                            ServiceContentValueId = k.ServiceContentValueId,
                            //ContentOptionId = k.ContentValue,
                            ContentValueEn = (_dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 6) ? _dbContext.ServiceTemplateOptions.Where(e => e.ServiceTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            ContentValueAr = (_dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 6) ? _dbContext.ServiceTemplateOptions.Where(e => e.ServiceTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,
                            FieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ServiceTemplateFieldCaptionAr = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionAr,
                            ServiceTemplateFieldCaptionEn = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionEn,
                        })
                    })
                            .ToList(),

                }).FirstOrDefault();


                if (service is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Service" });
                }

                return Ok(new { Status = true, Service = service });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetServiceDetailsNewById")]
        //public async Task<IActionResult> GetServiceDetailsNewById(int ServiceId, string userId)
        //{
        //    try
        //    {


        //        var service = _dbContext.Services.Include(c => c.ServiceImages).Where(c => c.ServiceId == ServiceId).Select(c => new
        //        {
        //            ServiceId = c.ServiceId,
        //            ServiceCatagoryId = c.ServiceCatagoryId,
        //            IsActive = c.IsActive,
        //            Price = c.Price,
        //            SortOrder = c.SortOrder,
        //            ServiceTitleAr = c.ServiceTitleAr,
        //            ServiceTitleEn = c.ServiceTitleEn,
        //            ServiceTypeId = c.ServiceTypeId,
        //            MainPic = c.MainPic,
        //            IsFavourite = _dbContext.ServiceFavourites.Any(o => o.ServiceId == ServiceId && o.UserId == userId),

        //            ServiceImages = c.ServiceImages.Select(i => new
        //            {
        //                ServiceImageId = i.ServiceImageId,
        //                Image = i.Image
        //            }).ToList(),


        //            ServiceContents = _dbContext.ServiceContents.Where(e => e.ServiceId == c.ServiceId).Select(l => new
        //            {
        //                ServiceContentId = l.ServiceContentId,
        //                ServiceTemplateConfigId = l.ServiceTemplateConfigId,
        //                ServiceContentValues = l.ServiceContentValues.Select(k => new
        //                {
        //                    ServiceContentValueId = k.ServiceContentValueId,
        //                    ContentValue = (_dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 5) ? _dbContext.ServiceTemplateOptions.Where(e => e.ServiceTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    ServiceTemplateFieldCaptionAr = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionAr,
        //                    ServiceTemplateFieldCaptionEn = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionEn,
        //                })
        //            })
        //                    .ToList(),

        //        }).FirstOrDefault();


        //        if (service is null)
        //        {
        //            return NotFound(new { Status = false, Message = "There is no Service" });
        //        }

        //        return Ok(new { Status = true, Service = service });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetServiceDetailsByIdForEdit")]
        public async Task<IActionResult> GetServiceDetailsByIdForEdit(int ServiceId)
        {
            try
            {


                var service = _dbContext.Services.Include(c => c.ServiceImages).Where(c => c.ServiceId == ServiceId).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceCatagoryId = c.ServiceCatagoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    ServiceTitleAr = c.ServiceTitleAr,
                    ServiceTitleEn = c.ServiceTitleEn,
                    ServiceTypeId = c.ServiceTypeId,
                    MainPic = c.MainPic,
                    Reel = c.Reel,
                    Description = c.Description,
                    ServiceImages = c.ServiceImages.Select(i => new
                    {
                        ServiceImageId = i.ServiceImageId,
                        Image = i.Image
                    }).ToList(),


                    ServiceContents = _dbContext.ServiceContents.Where(e => e.ServiceId == c.ServiceId).Select(l => new
                    {
                        ServiceContentId = l.ServiceContentId,
                        ServiceTemplateConfigId = l.ServiceTemplateConfigId,
                        ServiceContentValues = l.ServiceContentValues.Select(k => new
                        {
                            ServiceContentValueId = k.ServiceContentValueId,
                            ContentValue = k.ContentValue,
                            FieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ServiceTemplateFieldCaptionAr = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionAr,
                            ServiceTemplateFieldCaptionEn = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionEn,
                        })
                    })
                            .ToList(),

                }).FirstOrDefault();


                if (service is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Service" });
                }

                return Ok(new { Status = true, Service = service });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetAllServiceDetailsByServiceCatagoryId")]
        //public async Task<IActionResult> GetAllServiceDetailsByServiceCatagoryId(long ServiceCatagoryId)
        //{
        //    try
        //    {


        //        var service = _dbContext.Services.Include(c => c.ServiceImages).Where(c => c.ServiceCatagoryId == ServiceCatagoryId).Select(c => new
        //        {
        //            ServiceId = c.ServiceId,
        //            ServiceCatagoryId = c.ServiceCatagoryId,
        //            IsActive = c.IsActive,
        //            Price = c.Price,
        //            SortOrder = c.SortOrder,
        //            ServiceTitleAr = c.ServiceTitleAr,
        //            ServiceTitleEn = c.ServiceTitleEn,
        //            ServiceTypeId = c.ServiceTypeId,
        //            MainPic = c.MainPic,
        //            ServiceImages = c.ServiceImages.Select(i => new
        //            {
        //                ServiceImageId = i.ServiceImageId,
        //                Image = i.Image
        //            }).ToList(),


        //            ServiceContents = _dbContext.ServiceContents.Where(e => e.ServiceId == c.ServiceId).Select(l => new
        //            {
        //                ServiceContentId = l.ServiceContentId,
        //                ServiceTemplateConfigId = l.ServiceTemplateConfigId,
        //                ServiceContentValues = l.ServiceContentValues.Select(k => new
        //                {
        //                    ServiceContentValueId = k.ServiceContentValueId,
        //                    ContentValue = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.ServiceTemplateOptions.Where(e => e.ServiceTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    ServiceTemplateFieldCaptionAr = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionAr,
        //                    ServiceTemplateFieldCaptionEn = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionEn,
        //                })
        //            })
        //                    .ToList(),

        //        }).ToList();


        //        if (service is null)
        //        {
        //            return NotFound(new { Status = false, Message = "There is no Services" });
        //        }

        //        return Ok(new { Status = true, Service = service });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetAllServicesDetailsByServiceCategoryId")]
        public IActionResult GetAllServicesDetailsByServiceCategoryId(int serviceCatagoryId, string userId)
        {
            try
            {
                var serviceCategory = _dbContext.ServiceCatagories.Where(e => e.ServiceCatagoryId == serviceCatagoryId).FirstOrDefault();
                if (serviceCategory == null)
                {
                    return Ok(new { Status = false, Message = "Service Category Not Found" });

                }
                var services = _dbContext.Services.Include(c => c.ServiceImages).Where(c => c.ServiceCatagoryId == serviceCatagoryId && c.IsActive == true).OrderBy(e => e.SortOrder).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceCatagoryId = c.ServiceCatagoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    ServiceTitleAr = c.ServiceTitleAr,
                    ServiceTitleEn = c.ServiceTitleEn,
                    ServiceTypeId = c.ServiceTypeId,
                    MainPic = c.MainPic,
                    Description = c.Description,
                    Reel = c.Reel,
                    IsFavourite = _dbContext.ServiceFavourites.Any(o => o.ServiceId == c.ServiceId && o.UserId == userId),

                    ServiceImages = c.ServiceImages.Select(i => new
                    {
                        ServiceImageId = i.ServiceImageId,
                        Image = i.Image
                    }).ToList(),


                    ServiceContents = _dbContext.ServiceContents.Where(e => e.ServiceId == c.ServiceId).Select(l => new
                    {
                        ServiceContentId = l.ServiceContentId,
                        ServiceTemplateConfigId = l.ServiceTemplateConfigId,
                        ServiceContentValues = l.ServiceContentValues.Select(k => new
                        {
                            ServiceContentValueId = k.ServiceContentValueId,
                            ContentValue = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.ServiceTemplateOptions.Where(e => e.ServiceTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ServiceTemplateFieldCaptionAr = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionAr,
                            ServiceTemplateFieldCaptionEn = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionEn,
                        })
                    })
                                  .ToList(),

                }).ToList();



                return Ok(new { Status = true, Services = services });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPost]
        [Route("AddServiceToFavourite")]
        public IActionResult AddServiceToFavourite(long ServiceId, string userId)
        {
            try
            {

                var service = _dbContext.Services.Find(ServiceId);
                var user = _applicationDbContext.Users.Find(userId);
                if (service == null)
                {
                    return Ok(new { Status = false, Message = "Service Not Found." });

                }
                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found." });

                }
                var ServiceFavourite = _dbContext.ServiceFavourites.Where(a => a.ServiceId == ServiceId && a.UserId == userId).FirstOrDefault();
                if (ServiceFavourite != null)
                {
                    return Ok(new { Status = false, Message = "Service Already Added To Favourites" });
                }
                var serviceFav = new ServiceFavourite() { UserId = userId, ServiceId = ServiceId };
                _dbContext.ServiceFavourites.Add(serviceFav);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Added To Favourite" });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, Message = e.Message });
            }
        }

        [HttpDelete]
        [Route("RemoveServiceFromFavourite")]
        public IActionResult RemoveServiceFromFavourite(long ServiceId, string userId)
        {
            try
            {
                var serviceFavourite = _dbContext.ServiceFavourites.Where(e => e.ServiceId == ServiceId && e.UserId == userId).FirstOrDefault();
                if (serviceFavourite == null)
                {
                    return Ok(new { Status = false, Message = "Service Favourite Not Found.." });
                }
                _dbContext.ServiceFavourites.Remove(serviceFavourite);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Deleted Successfully From Favourite" });

            }
            catch (Exception e)
            {

                return Ok(new { Status = false, Message = e.Message });

            }
        }
        #endregion
        #region Orders
        [HttpGet]
        [Route("GetOrdersByBDId")]
        public async Task<IActionResult> GetOrdersByBDId(long BDId)
        {
            try
            {
                var BDObj = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == BDId).FirstOrDefault();
                if (BDObj == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Not Found" });

                }
                var listOfIOrders = _dbContext.Orders.Where(c => c.ClassifiedBusinessId == BDId)

                    .Select(c => new
                    {
                        OrderId = c.OrderId,
                        UserId = c.UserId,
                        CustomerAddressId = c.CustomerAddressId != null ? c.CustomerAddressId : null,
                        ClassifiedBusinessId = c.ClassifiedBusinessId,
                        Discount = c.Discount,
                        OrderTotal = c.OrderTotal,
                        Deliverycost = c.Deliverycost,
                        IsDeliverd = c.IsDeliverd,
                        OrderSerial = c.OrderSerial,
                        IsCancelled = c.IsCancelled,
                        OrderDate = c.OrderDate,
                        OrderNet = c.OrderNet,
                        IsPaid = c.IsPaid,
                        Adress = c.Adress,
                        OrderItems = _dbContext.OrderItems
                        .Include(w => w.Product).Include(c => c.Product.ProductCategory.ClassifiedBusiness)
                        .Where(o => o.OrderId == c.OrderId)
                        .Select(s => new
                        {
                            ProductTitle = s.Product.TitleEn,
                            ProductPic = s.Product.MainPic,
                            Productprice = s.ItemPrice,
                            ProductPriceId = s.ProductPriceId,
                            NotFixedproductPrice = _dbContext.ProductPrices.Where(e => e.ProductPriceId == s.ProductPriceId).FirstOrDefault() == null ? 0 : _dbContext.ProductPrices.Where(e => e.ProductPriceId == s.ProductPriceId).FirstOrDefault().Price,
                            ProductQuantity = s.ProductQuantity,
                            extraFeatures = (s.OrderItemExtraProducts != null && s.OrderItemExtraProducts.Count != 0) ? s.OrderItemExtraProducts.Select(kn => new
                            {
                                Price = kn.Price,
                                OrderItemExtraProductId = kn.OrderItemExtraProductId,
                                ProductExtraId = kn.ProductExtraId,
                                OrderItemId = kn.OrderItemId,
                                ExtraTitleAr = kn.ProductExtra.ExtraTitleAr,
                                ExtraTitleEn = kn.ProductExtra.ExtraTitleEn,
                            }).ToList() : null,

                        }).ToList()

                    }).ToList();


                return Ok(new { Status = true, ListOfIOrders = listOfIOrders });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<IActionResult> GetOrderById(long orderId)
        {
            try
            {
                var order = _dbContext.Orders.Where(e => e.OrderId == orderId).FirstOrDefault();
                if (order == null)
                {
                    return Ok(new { Status = false, Message = "Order Not Found" });

                }
                var RequiredOrder = _dbContext.Orders.Where(c => c.OrderId == orderId)

                    .Select(c => new
                    {
                        OrderId = c.OrderId,
                        UserId = c.UserId,
                        CustomerAddressId = c.CustomerAddressId != null ? c.CustomerAddressId : null,
                        ClassifiedBusinessId = c.ClassifiedBusinessId,
                        Discount = c.Discount,
                        OrderTotal = c.OrderTotal,
                        Deliverycost = c.Deliverycost,
                        IsDeliverd = c.IsDeliverd,
                        OrderSerial = c.OrderSerial,
                        IsCancelled = c.IsCancelled,
                        OrderDate = c.OrderDate,
                        OrderNet = c.OrderNet,
                        IsPaid = c.IsPaid,
                        Adress = c.Adress,
                        OrderItems = _dbContext.OrderItems
                        .Include(w => w.Product).Include(c => c.Product.ProductCategory.ClassifiedBusiness)
                        .Where(o => o.OrderId == c.OrderId)
                        .Select(s => new
                        {
                            ProductDetails = s.Product,
                            Productprice = s.ItemPrice,
                            ProductPriceId = s.ProductPriceId,
                            NotFixedproductPrice = _dbContext.ProductPrices.Where(e => e.ProductPriceId == s.ProductPriceId).FirstOrDefault() == null ? 0 : _dbContext.ProductPrices.Where(e => e.ProductPriceId == s.ProductPriceId).FirstOrDefault().Price,
                            ProductQuantity = s.ProductQuantity,
                            extraFeatures = (s.OrderItemExtraProducts != null && s.OrderItemExtraProducts.Count != 0) ? s.OrderItemExtraProducts.Select(kn => new
                            {
                                Price = kn.Price,
                                OrderItemExtraProductId = kn.OrderItemExtraProductId,
                                ProductExtraId = kn.ProductExtraId,
                                OrderItemId = kn.OrderItemId,
                                ExtraTitleAr = kn.ProductExtra.ExtraTitleAr,
                                ExtraTitleEn = kn.ProductExtra.ExtraTitleEn,
                            }).ToList() : null,
                        }).ToList()

                    }).FirstOrDefault();


                return Ok(new { Status = true, Order = RequiredOrder });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        #endregion
        #endregion
        [HttpGet]
        [Route("GetBusinessDetailsByIdForEdit")]
        public async Task<IActionResult> GetBusinessDetailsByIdForEdit(int ClassifiedBusinessId)
        {
            try
            {

                var classifiedBusinessVm = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == ClassifiedBusinessId).FirstOrDefault();
                if (classifiedBusinessVm == null)
                {
                    return Ok(new { Status = false, Message = "Business Not Found" });
                }
                var user = await _userManager.FindByIdAsync(classifiedBusinessVm.UseId);

                if (user == null)
                {
                    return Ok(new { Status = false, Message = "User Not Found" });

                }
                var ClassifiedBusiness = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == ClassifiedBusinessId).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Businessworkinghours = _dbContext.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(a => new
                    {
                        a.BusinessWorkingHoursId,
                        a.Day,
                        a.EndTime1,
                        a.EndTime2,
                        a.Isclosed,
                        a.StartTime1,
                        a.StartTime2
                    }).ToList(),
                    PublishDate = c.PublishDate,
                    UseId = c.UseId,
                    FullName = user.FullName,
                    ProfilePicture = user.ProfilePicture,
                    Job = user.Job,
                    Views = c.Views,
                    Reviews = _dbContext.Reviews.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
                    {
                        ClassifiedBusinessId = l.ClassifiedBusinessId,
                        Name = l.Name,
                        Rating = l.Rating,
                        ReviewDate = l.ReviewDate,
                        ReviewId = l.ReviewId,
                        Title = l.Title,
                        Email = l.Email,
                    }).ToList(),
                    BusinessContents = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
                    {
                        BusinessContentId = l.BusinessContentId,
                        BusinessTemplateConfigId = l.BusinessTemplateConfigId,
                        AdContentValues = l.BusinessContentValues.Select(k => new
                        {
                            BusinessContentValueId = k.BusinessContentValueId,
                            ContentValue = k.ContentValue,
                            FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
                            BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
                            BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,
                        })
                    })
                        .ToList(),

                }).FirstOrDefault();


                if (ClassifiedBusiness is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Business" });
                }
                _dbContext.SaveChanges();
                return Ok(new { Status = true, ClassifiedBusiness = ClassifiedBusiness });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }


        [HttpDelete]
        [Route("DeleteProductExtra")]
        public async Task<ActionResult> DeleteProductExtra(long productExtraId)
        {
            try
            {
                var productExt = _dbContext.ProductExtras.Where(e => e.ProductExtraId == productExtraId).FirstOrDefault();
                if (productExt == null)
                {
                    return Ok(new { Status = false, Message = "Product Extra Not Found" });
                }
                _dbContext.ProductExtras.Remove(productExt);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Extra Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteProductPrice")]
        public async Task<ActionResult> DeleteProductPrice(long productPriceId)
        {
            try
            {
                var productPrice = _dbContext.ProductPrices.Where(e => e.ProductPriceId == productPriceId).FirstOrDefault();
                if (productPrice == null)
                {
                    return Ok(new { Status = false, Message = "Product Price Not Found" });
                }
                _dbContext.ProductPrices.Remove(productPrice);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Price Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteProductImage")]
        public async Task<ActionResult> DeleteProductImage(long ProductImageId)
        {
            try
            {
                var productImage = _dbContext.ProductImages.Where(e => e.ProductImageId == ProductImageId).FirstOrDefault();
                if (productImage == null)
                {
                    return Ok(new { Status = false, Message = "Product Image Not Found" });
                }
                _dbContext.ProductImages.Remove(productImage);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Image Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        //[HttpPut]
        //[Route("EditProduct")]
        //public async Task<ActionResult> EditProduct([FromForm] EditProductVm ProductVM, List<ProductMediaVm> mediaVms, IFormFile mainPic, List<ProductExtraVm> productExtraVms, List<ProductPriceVm> productPriceVms)
        //{
        //    try
        //    {
        //        var ProductModel = _dbContext.Products.Find(ProductVM.ProductId);
        //        if (ProductModel == null)
        //        {
        //            return Ok(new { Status = false, Message = "Product Not Found" });
        //        }
        //        var ProductCat = _dbContext.ProductCategories.Find(ProductVM.ProductCategoryId);
        //        if (ProductCat == null)
        //        {
        //            return Ok(new { Status = false, Message = "Category Not Found" });
        //        }
        //        var ProductType = _dbContext.ProductTypes.Find(ProductVM.ProductTypeId);
        //        if (ProductType == null)
        //        {
        //            return Ok(new { Status = false, Message = "Product Type Not Found" });
        //        }


        //        ProductModel.IsActive = ProductVM.IsActive;
        //        ProductModel.ProductCategoryId = ProductVM.ProductCategoryId;
        //        ProductModel.Price = ProductVM.Price;
        //        ProductModel.ProductName = ProductVM.ProductName;
        //        ProductModel.SortOrder = ProductVM.SortOrder;
        //        ProductModel.IsFixedPrice = ProductVM.IsFixedPrice;
        //        ProductModel.ProductTypeId = ProductVM.ProductTypeId;
        //        if (ProductVM.IsFixedPrice == false)
        //        {
        //            var ppList = _dbContext.ProductPrices.Where(e => e.ProductId == ProductVM.ProductId).ToList();
        //            if (ppList != null)
        //            {
        //                _dbContext.ProductPrices.RemoveRange(ppList);
        //            }
        //        }
        //        if (mainPic != null)
        //        {
        //            string folder = "Images/ProductMedia/";
        //            ProductModel.MainPic = UploadImage(folder, mainPic);

        //        }
        //        _dbContext.Attach(ProductModel).State = EntityState.Modified;
        //        if (productExtraVms != null)
        //        {
        //            List<ProductExtra> productExtras = new List<ProductExtra>();
        //            foreach (var item in productExtraVms)
        //            {
        //                ProductExtra productExtraObj = new ProductExtra()
        //                {
        //                    ExtraTitleAr = item.ExtraTitleAr,
        //                    ExtraTitleEn = item.ExtraTitleEn,
        //                    ExtraDes = item.ExtraDes,
        //                    Price = item.Price
        //                };
        //                productExtras.Add(productExtraObj);

        //            }
        //            _dbContext.ProductExtras.AddRange(productExtras);
        //        }
        //        if (productPriceVms != null)
        //        {
        //            List<ProductPrice> productPriceses = new List<ProductPrice>();
        //            foreach (var item in productPriceVms)
        //            {
        //                ProductPrice productPriceObj = new ProductPrice()
        //                {
        //                    ProductPriceTilteAr = item.ProductPriceTilteAr,
        //                    ProductPriceTilteEn = item.ProductPriceTilteEn,
        //                    ProductPriceDes = item.ProductPriceDes,
        //                    Price = item.Price
        //                };
        //                productPriceses.Add(productPriceObj);
        //            }
        //            _dbContext.ProductPrices.AddRange(productPriceses);
        //        }
        //        if (mediaVms != null)
        //        {
        //            if (mediaVms.Count != 0)
        //            {
        //                foreach (var item in mediaVms)
        //                {
        //                    var productContentModel = _dbContext.ProductContents.Where(e => e.ProductId == ProductVM.ProductId && e.ProductTemplateConfigId == item.ProductTemplateConfigId).FirstOrDefault();
        //                    if (productContentModel != null)
        //                    {
        //                        if (item.Media.Count() != 0)
        //                        {
        //                            var contentListMediaValue = new List<ProductContentValue>();
        //                            for (int i = 0; i < item.Media.Count(); i++)
        //                            {
        //                                if (item.Media[i] != null)
        //                                {
        //                                    string folder = "Images/ProductMedia/";
        //                                    var contentObj = new ProductContentValue();
        //                                    contentObj.ContentValue = UploadImage(folder, item.Media[i]);
        //                                    contentObj.ProductContentId = productContentModel.ProductContentId;
        //                                    contentListMediaValue.Add(contentObj);
        //                                }

        //                            }
        //                            _dbContext.ProductContentValues.AddRange(contentListMediaValue);
        //                        }
        //                        //    var proContentValues = _dbContext.ProductContentValues.Where(e => e.ProductContentId == productContentModel.ProductContentId).ToList();
        //                        //if (proContentValues != null)
        //                        //{
        //                        //    _dbContext.RemoveRange(proContentValues);
        //                        //}

        //                    }
        //                    else
        //                    {
        //                        if (item.Media.Count() != 0)
        //                        {
        //                            var contentListMediaValue = new List<ProductContentValue>();
        //                            for (int i = 0; i < item.Media.Count(); i++)
        //                            {
        //                                if (item.Media[i] != null)
        //                                {
        //                                    string folder = "Images/ProductMedia/";
        //                                    var contentObj = new ProductContentValue();
        //                                    contentObj.ContentValue = UploadImage(folder, item.Media[i]);
        //                                    contentListMediaValue.Add(contentObj);
        //                                }

        //                            }
        //                            var ProductContentObj = new ProductContent()
        //                            {
        //                                ProductId = ProductVM.ProductId,
        //                                ProductTemplateConfigId = item.ProductTemplateConfigId,
        //                                ProductContentValues = contentListMediaValue

        //                            };
        //                            _dbContext.ProductContents.Add(ProductContentObj);


        //                        }

        //                    }

        //                }

        //            }
        //        }
        //        if (ProductVM.ProductContentVMS != null)
        //        {
        //            if (ProductVM.ProductContentVMS.Count != 0)
        //            {
        //                foreach (var item in ProductVM.ProductContentVMS)
        //                {
        //                    var productContentModel = _dbContext.ProductContents.Where(e => e.ProductId == ProductVM.ProductId && e.ProductTemplateConfigId == item.ProductTemplateConfigId).FirstOrDefault();
        //                    if (productContentModel != null)
        //                    {
        //                        var proContentValues = _dbContext.ProductContentValues.Where(e => e.ProductContentId == productContentModel.ProductContentId).ToList();
        //                        if (proContentValues != null)
        //                        {
        //                            _dbContext.RemoveRange(proContentValues);
        //                        }
        //                    }
        //                    var contentValList = new List<ProductContentValue>();

        //                    foreach (var value in item.Values)
        //                    {

        //                        var contentValObj = new ProductContentValue()
        //                        {
        //                            ContentValue = value,
        //                        };
        //                        contentValList.Add(contentValObj);
        //                    }
        //                    var ProductContentObj = new ProductContent()
        //                    {
        //                        ProductContentId = ProductVM.ProductId,
        //                        ProductTemplateConfigId = item.ProductTemplateConfigId,
        //                        ProductContentValues = contentValList
        //                    };
        //                    _dbContext.ProductContents.Add(ProductContentObj);

        //                }
        //            }
        //        }
        //        _dbContext.SaveChanges();
        //        return Ok(new { Status = true, Message = "Product Edited Successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpDelete]
        [Route("DeleteProductDynamicMedia")]
        public async Task<ActionResult> DeleteProductDynamicMedia(int contentValueId)
        {
            try
            {
                var ProductContentValue = _dbContext.ProductContentValues.Where(e => e.ProductContentValueId == contentValueId).FirstOrDefault();
                if (ProductContentValue == null)
                {
                    return Ok(new { Status = false, Message = "Media Not Found" });

                }
                _dbContext.ProductContentValues.Remove(ProductContentValue);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Media Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteBDImageById")]
        public async Task<ActionResult> DeleteBDImageById(int BDImageId)
        {
            try
            {
                var BDImage = _dbContext.BDImages.Where(e => e.BDImageId == BDImageId).FirstOrDefault();
                if (BDImage == null)
                {
                    return Ok(new { Status = false, Message = "Image Not Found" });

                }
                _dbContext.BDImages.Remove(BDImage);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Image Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPut]
        [Route("EditBusiness")]
        public async Task<ActionResult> EditBusiness([FromForm] EditBDVm BusinessVM, IFormFile MainPic, IFormFile Logo, IFormFile Reel, List<BusinessMediaVm> mediaVms, List<WorkingHoursVM> Workinghoursvm, IFormFileCollection Slider)
        {
            try
            {

                var Bussiness = _dbContext.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == BusinessVM.BDId).FirstOrDefault();

                if (Bussiness == null)
                {
                    return Ok(new { Status = false, Message = "Bussiness Not Found" });
                }

                var cityObj = _dbContext.Cities.Find(BusinessVM.CityId);


                if (cityObj == null)
                {
                    return Ok(new { Status = false, Message = "City Not Found" });

                }
                var AreaObj = _dbContext.Areas.Where(e => e.AreaId == BusinessVM.AreaId && e.CityId == BusinessVM.CityId).FirstOrDefault();


                if (AreaObj == null)
                {
                    return Ok(new { Status = false, Message = "Area Not Found" });

                }

                Bussiness.IsActive = BusinessVM.IsActive;
                Bussiness.PublishDate = DateTime.Now;
                Bussiness.Title = BusinessVM.Title;
                Bussiness.Description = BusinessVM.Description;
                Bussiness.Email = BusinessVM.Email;
                Bussiness.Address = BusinessVM.Address;
                Bussiness.phone = BusinessVM.phone;
                Bussiness.CityId = BusinessVM.CityId;
                Bussiness.AreaId = BusinessVM.AreaId;
                Bussiness.Location = BusinessVM.Location;
                if (MainPic != null)
                {
                    string folder = "Images/BusinessMedia/";
                    Bussiness.Mainpic = UploadImage(folder, MainPic);
                }
                if (Logo != null)
                {
                    string folder = "Images/BusinessMedia/";
                    Bussiness.Logo = UploadImage(folder, Logo);
                }
                if (Reel != null)
                {
                    string folder = "Images/BusinessMedia/";
                    Bussiness.Reel = UploadImage(folder, Reel);
                }
                _dbContext.Attach(Bussiness).State = EntityState.Modified;
                if (Workinghoursvm != null)
                {
                    List<BusinessWorkingHours> workingHours = new List<BusinessWorkingHours>();
                    var BWH = _dbContext.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == Bussiness.ClassifiedBusinessId).ToList();
                    if (BWH != null)
                    {
                        _dbContext.BusinessWorkingHours.RemoveRange(BWH);
                    }
                    foreach (var item in Workinghoursvm)
                    {
                        Models.BusinessWorkingHours businessWorkingHoursobj = new Models.BusinessWorkingHours()
                        {
                            StartTime1 = item.StartTime1,
                            StartTime2 = item.StartTime2,
                            EndTime1 = item.EndTime1,
                            EndTime2 = item.EndTime2,
                            Day = item.Day,
                            Isclosed = item.Isclosed,
                            ClassifiedBusinessId = Bussiness.ClassifiedBusinessId
                        };

                        workingHours.Add(businessWorkingHoursobj);
                    }
                    _dbContext.BusinessWorkingHours.AddRange(workingHours);
                }
                if (Slider != null)
                {
                    List<BDImage> BDSlider = new List<BDImage>();
                    foreach (var item in Slider)
                    {
                        var BDImageObj = new BDImage();
                        BDImageObj.Image = UploadImage("Images/BusinessMedia/", item);
                        BDImageObj.ClassifiedBusinessId = Bussiness.ClassifiedBusinessId;
                        BDSlider.Add(BDImageObj);

                    }
                    _dbContext.BDImages.AddRange(BDSlider);

                }
                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var item in mediaVms)
                        {
                            var BDContentModel = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == BusinessVM.BDId && e.BusinessTemplateConfigId == item.BusinessTemplateConfigId).FirstOrDefault();
                            if (BDContentModel != null)
                            {
                                if (item.Media.Count() != 0)
                                {
                                    var contentListMediaValue = new List<BusinessContentValue>();
                                    for (int i = 0; i < item.Media.Count(); i++)
                                    {
                                        if (item.Media[i] != null)
                                        {
                                            string folder = "Images/BusinessMedia/";
                                            var contentObj = new BusinessContentValue();
                                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                            contentObj.BusinessContentId = BDContentModel.BusinessContentId;
                                            contentListMediaValue.Add(contentObj);
                                        }

                                    }
                                    _dbContext.BusinessContentValues.AddRange(contentListMediaValue);

                                }


                            }
                            else
                            {
                                if (item.Media.Count() != 0)
                                {
                                    var contentListMediaValue = new List<BusinessContentValue>();
                                    for (int i = 0; i < item.Media.Count(); i++)
                                    {
                                        if (item.Media[i] != null)
                                        {
                                            string folder = "Images/BusinessMedia/";
                                            var contentObj = new BusinessContentValue();
                                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                            contentListMediaValue.Add(contentObj);
                                        }

                                    }
                                    var BDContentObj = new BusinessContent()
                                    {
                                        ClassifiedBusinessId = BusinessVM.BDId,
                                        BusinessTemplateConfigId = item.BusinessTemplateConfigId,
                                        BusinessContentValues = contentListMediaValue

                                    };
                                    _dbContext.BusinessContents.Add(BDContentObj);

                                }

                            }

                        }
                    }
                }
                if (BusinessVM.BusinessContentVMS != null)
                {
                    if (BusinessVM.BusinessContentVMS.Count != 0)
                    {
                        foreach (var item in BusinessVM.BusinessContentVMS)
                        {
                            var businessContentModel = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == BusinessVM.BDId && e.BusinessTemplateConfigId == item.BusinessTemplateConfigId).FirstOrDefault();
                            if (businessContentModel != null)
                            {
                                var fieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == businessContentModel.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId;
                                if (fieldTypeId == 6)
                                {
                                    var BDContentValues = _dbContext.BusinessContentValues.Where(e => e.BusinessContentId == businessContentModel.BusinessContentId).ToList();
                                    if (BDContentValues != null)
                                    {
                                        _dbContext.BusinessContentValues.RemoveRange(BDContentValues);
                                        // _dbContext.BusinessContents(businessContentModel);
                                    }
                                    if (item.Values != null)
                                    {
                                        if (item.Values.Count != 0)
                                        {
                                            var contentValList = new List<BusinessContentValue>();

                                            foreach (var value in item.Values)
                                            {

                                                var contentValObj = new BusinessContentValue()
                                                {
                                                    ContentValue = value,
                                                    BusinessContentId = businessContentModel.BusinessContentId
                                                };
                                                contentValList.Add(contentValObj);
                                            }

                                            _dbContext.BusinessContentValues.AddRange(contentValList);

                                        }
                                    }


                                }
                                else
                                {
                                    var BDContentValueModel = _dbContext.BusinessContentValues.Where(e => e.BusinessContentId == businessContentModel.BusinessContentId).FirstOrDefault();
                                    if (BDContentValueModel != null)
                                    {
                                        BDContentValueModel.ContentValue = item.Values.FirstOrDefault();
                                        _dbContext.Attach(BDContentValueModel).State = EntityState.Modified;
                                    }

                                }
                            }
                            else
                            {
                                var contentValList = new List<BusinessContentValue>();

                                foreach (var value in item.Values)
                                {

                                    var contentValObj = new BusinessContentValue()
                                    {
                                        ContentValue = value,
                                    };
                                    contentValList.Add(contentValObj);
                                }
                                var BusinessContentObj = new BusinessContent()
                                {
                                    ClassifiedBusinessId = BusinessVM.BDId,
                                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
                                    BusinessContentValues = contentValList
                                };
                                _dbContext.BusinessContents.Add(BusinessContentObj);

                            }





                        }


                    }
                }
                //if (BusinessVM.BusinessContentVMS != null)
                //{
                //    if (BusinessVM.BusinessContentVMS.Count != 0)
                //    {
                //        foreach (var item in BusinessVM.BusinessContentVMS)
                //        {
                //            var BDContentModel = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == BusinessVM.BDId && e.BusinessTemplateConfigId == item.BusinessTemplateConfigId).FirstOrDefault();
                //            if (BDContentModel != null)
                //            {
                //                var BDContentValues = _dbContext.BusinessContentValues.Where(e => e.BusinessContentId == BDContentModel.BusinessContentId).ToList();
                //                if (BDContentValues != null)
                //                {
                //                    _dbContext.RemoveRange(BDContentValues);
                //                }
                //            }
                //            var contentValList = new List<BusinessContentValue>();

                //            foreach (var value in item.Values)
                //            {

                //                var contentValObj = new BusinessContentValue()
                //                {
                //                    ContentValue = value,
                //                };
                //                contentValList.Add(contentValObj);
                //            }
                //            var BDContentObj = new BusinessContent()
                //            {
                //                ClassifiedBusinessId = BusinessVM.BDId,
                //                BusinessTemplateConfigId = item.BusinessTemplateConfigId,
                //                BusinessContentValues = contentValList
                //            };
                //            _dbContext.BusinessContents.Add(BDContentObj);

                //        }
                //    }
                //}
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "BD Updated Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPut]
        [Route("EditProduct")]
        public async Task<ActionResult> EditProduct([FromForm] EditProductVm ProductVM, List<ProductMediaVm> mediaVms, IFormFile mainPic, IFormFile Reel, List<ProductExtraVm> productExtraVms, List<ProductPriceVm> productPriceVms, IFormFileCollection ProductImages)
        {
            try
            {
                var ProductModel = _dbContext.Products.Find(ProductVM.ProductId);
                if (ProductModel == null)
                {
                    return Ok(new { Status = false, Message = "Product Not Found" });
                }
                var ProductCat = _dbContext.ProductCategories.Find(ProductVM.ProductCategoryId);
                if (ProductCat == null)
                {
                    return Ok(new { Status = false, Message = "Category Not Found" });
                }
                var ProductType = _dbContext.ProductTypes.Find(ProductVM.ProductTypeId);
                if (ProductType == null)
                {
                    return Ok(new { Status = false, Message = "Product Type Not Found" });
                }
                ProductModel.IsActive = ProductVM.IsActive;
                ProductModel.ProductCategoryId = ProductVM.ProductCategoryId;
                ProductModel.Price = ProductVM.Price;
                //ProductModel.ProductName = ProductVM.ProductName;
                ProductModel.TitleAr = ProductVM.TitleAr;
                ProductModel.TitleEn = ProductVM.TitleEn;
                ProductModel.Description = ProductVM.Description;
                ProductModel.SortOrder = ProductVM.SortOrder;
                ProductModel.IsFixedPrice = ProductVM.IsFixedPrice;
                ProductModel.ProductTypeId = ProductVM.ProductTypeId;
                if (ProductVM.IsFixedPrice == false)
                {
                    var ppList = _dbContext.ProductPrices.Where(e => e.ProductId == ProductVM.ProductId).ToList();
                    if (ppList != null)
                    {
                        if (ppList.Count != 0)
                        {
                            //foreach (var item in ppList)
                            //{
                            //    var OrderItems = _dbContext.OrderItems.Where(e => e.ProductPriceId == item.ProductPriceId).ToList();
                            //    if (OrderItems != null)
                            //    {
                            //        foreach (var Oritem in OrderItems)
                            //        {
                            //            var orderItemEx = _dbContext.OrderItemExtraProducts.Where(e => e.OrderItemId == Oritem.OrderItemId).ToList();
                            //            if (orderItemEx != null)
                            //            {
                            //                _dbContext.OrderItemExtraProducts.RemoveRange(orderItemEx);
                            //            }
                            //        }
                            //        _dbContext.OrderItems.RemoveRange(OrderItems);

                            //    }
                            //}

                            //_dbContext.ProductPrices.RemoveRange(ppList);
                        }

                    }
                }
                if (mainPic != null)
                {
                    string folder = "Images/ProductMedia/";
                    ProductModel.MainPic = UploadImage(folder, mainPic);

                }
                if (Reel != null)
                {
                    string folder = "Images/ProductMedia/";
                    ProductModel.Reel = UploadImage(folder, Reel);

                }
                if (ProductImages != null)
                {
                    if (ProductImages.Count != 0)
                    {
                        List<ProductImage> prodImages = new List<ProductImage>();
                        foreach (var item in ProductImages)
                        {
                            var ProductImageObj = new ProductImage();
                            ProductImageObj.Image = UploadImage("Images/ProductMedia/", item);
                            ProductImageObj.ProductId = ProductVM.ProductId;
                            prodImages.Add(ProductImageObj);

                        }
                        _dbContext.ProductImages.AddRange(prodImages);
                    }


                }
                _dbContext.Attach(ProductModel).State = EntityState.Modified;
                if (productExtraVms != null)
                {
                    var pExtraList = _dbContext.ProductExtras.Where(e => e.ProductId == ProductVM.ProductId).ToList();
                    if (pExtraList != null)
                    {
                        _dbContext.ProductExtras.RemoveRange(pExtraList);
                    }
                    List<ProductExtra> productExtras = new List<ProductExtra>();
                    foreach (var item in productExtraVms)
                    {
                        ProductExtra productExtraObj = new ProductExtra()
                        {
                            ExtraTitleAr = item.ExtraTitleAr,
                            ExtraTitleEn = item.ExtraTitleEn,
                            ExtraDes = item.ExtraDes,
                            Price = item.Price,
                            ProductId = ProductModel.ProductId
                        };
                        productExtras.Add(productExtraObj);

                    }
                    _dbContext.ProductExtras.AddRange(productExtras);
                }
                if (productPriceVms != null)
                {
                    var ppList = _dbContext.ProductPrices.Where(e => e.ProductId == ProductVM.ProductId).ToList();
                    if (ppList != null)
                    {
                        _dbContext.ProductPrices.RemoveRange(ppList);
                    }
                    List<ProductPrice> productPriceses = new List<ProductPrice>();
                    foreach (var item in productPriceVms)
                    {
                        ProductPrice productPriceObj = new ProductPrice()
                        {
                            ProductPriceTilteAr = item.ProductPriceTilteAr,
                            ProductPriceTilteEn = item.ProductPriceTilteEn,
                            ProductPriceDes = item.ProductPriceDes,
                            Price = item.Price,
                            ProductId = ProductModel.ProductId
                        };
                        productPriceses.Add(productPriceObj);
                    }
                    _dbContext.ProductPrices.AddRange(productPriceses);
                }
                if (mediaVms != null)
                {
                    if (mediaVms.Count != 0)
                    {
                        foreach (var item in mediaVms)
                        {
                            var productContentModel = _dbContext.ProductContents.Where(e => e.ProductId == ProductVM.ProductId && e.ProductTemplateConfigId == item.ProductTemplateConfigId).FirstOrDefault();
                            if (productContentModel != null)
                            {
                                if (item.Media.Count() != 0)
                                {
                                    var contentListMediaValue = new List<ProductContentValue>();
                                    for (int i = 0; i < item.Media.Count(); i++)
                                    {
                                        if (item.Media[i] != null)
                                        {
                                            string folder = "Images/ProductMedia/";
                                            var contentObj = new ProductContentValue();
                                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                            contentObj.ProductContentId = productContentModel.ProductContentId;
                                            contentListMediaValue.Add(contentObj);
                                        }

                                    }
                                    _dbContext.ProductContentValues.AddRange(contentListMediaValue);
                                }
                                //    var proContentValues = _dbContext.ProductContentValues.Where(e => e.ProductContentId == productContentModel.ProductContentId).ToList();
                                //if (proContentValues != null)
                                //{
                                //    _dbContext.RemoveRange(proContentValues);
                                //}

                            }
                            else
                            {
                                if (item.Media.Count() != 0)
                                {
                                    var contentListMediaValue = new List<ProductContentValue>();
                                    for (int i = 0; i < item.Media.Count(); i++)
                                    {
                                        if (item.Media[i] != null)
                                        {
                                            string folder = "Images/ProductMedia/";
                                            var contentObj = new ProductContentValue();
                                            contentObj.ContentValue = UploadImage(folder, item.Media[i]);
                                            contentListMediaValue.Add(contentObj);
                                        }

                                    }
                                    var ProductContentObj = new ProductContent()
                                    {
                                        ProductId = ProductVM.ProductId,
                                        ProductTemplateConfigId = item.ProductTemplateConfigId,
                                        ProductContentValues = contentListMediaValue

                                    };
                                    _dbContext.ProductContents.Add(ProductContentObj);


                                }

                            }

                        }

                    }
                }
                if (ProductVM.ProductContentVMS != null)
                {
                    if (ProductVM.ProductContentVMS.Count != 0)
                    {
                        foreach (var item in ProductVM.ProductContentVMS)
                        {
                            var productContentModel = _dbContext.ProductContents.Where(e => e.ProductId == ProductVM.ProductId && e.ProductTemplateConfigId == item.ProductTemplateConfigId).FirstOrDefault();
                            if (productContentModel != null)
                            {
                                var fieldTypeId = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == productContentModel.ProductTemplateConfigId).FirstOrDefault().FieldTypeId;
                                if (fieldTypeId == 6)
                                {
                                    var productContentValues = _dbContext.ProductContentValues.Where(e => e.ProductContentId == productContentModel.ProductContentId).ToList();
                                    if (productContentValues != null)
                                    {
                                        _dbContext.RemoveRange(productContentValues);
                                    }
                                    if (item.Values != null)
                                    {
                                        if (item.Values.Count != 0)
                                        {
                                            var contentValList = new List<ProductContentValue>();

                                            foreach (var value in item.Values)
                                            {

                                                var contentValObj = new ProductContentValue()
                                                {
                                                    ContentValue = value,
                                                    ProductContentId = productContentModel.ProductContentId
                                                };
                                                contentValList.Add(contentValObj);
                                            }

                                            _dbContext.ProductContentValues.AddRange(contentValList);
                                        }
                                    }



                                }
                                else
                                {
                                    var productContentValueModel = _dbContext.ProductContentValues.Where(e => e.ProductContentId == productContentModel.ProductContentId).FirstOrDefault();
                                    productContentValueModel.ContentValue = item.Values.FirstOrDefault();
                                    _dbContext.Attach(productContentValueModel).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                var contentValList = new List<ProductContentValue>();

                                foreach (var value in item.Values)
                                {

                                    var contentValObj = new ProductContentValue()
                                    {
                                        ContentValue = value,
                                    };
                                    contentValList.Add(contentValObj);
                                }
                                var ProductContentObj = new ProductContent()
                                {
                                    ProductId = ProductVM.ProductId,
                                    ProductTemplateConfigId = item.ProductTemplateConfigId,
                                    ProductContentValues = contentValList
                                };
                                _dbContext.ProductContents.Add(ProductContentObj);

                            }





                        }


                    }
                }
                //if (ProductVM.ProductContentVMS != null)
                //{
                //    if (ProductVM.ProductContentVMS.Count != 0)
                //    {
                //        foreach (var item in ProductVM.ProductContentVMS)
                //        {
                //            var productContentModel = _dbContext.ProductContents.Where(e => e.ProductId == ProductVM.ProductId && e.ProductTemplateConfigId == item.ProductTemplateConfigId).FirstOrDefault();
                //            if (productContentModel != null)
                //            {
                //                var proContentValues = _dbContext.ProductContentValues.Where(e => e.ProductContentId == productContentModel.ProductContentId).ToList();
                //                if (proContentValues != null)
                //                {
                //                    _dbContext.RemoveRange(proContentValues);
                //                }
                //            }
                //            var contentValList = new List<ProductContentValue>();

                //            foreach (var value in item.Values)
                //            {

                //                var contentValObj = new ProductContentValue()
                //                {
                //                    ContentValue = value,
                //                };
                //                contentValList.Add(contentValObj);
                //            }
                //            var ProductContentObj = new ProductContent()
                //            {
                //                ProductContentId = ProductVM.ProductId,
                //                ProductTemplateConfigId = item.ProductTemplateConfigId,
                //                ProductContentValues = contentValList
                //            };
                //            _dbContext.ProductContents.Add(ProductContentObj);

                //        }
                //    }
                //}
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Product Edited Successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetFilterBD")]
        //public async Task<IActionResult> GetFilterBD([FromForm] BDFilterModelVm BDFilterVm, int page = 1, int pageSize = 10)
        //{


        //    try
        //    {
        //        var BDCatagory = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryId == BDFilterVm.BusinessCategoryId).FirstOrDefault();
        //        if (BDCatagory == null)
        //        {
        //            return Ok(new { Status = false, Message = "Catagory Not Found" });
        //        }

        //        // List<long> BDIds = new List<long>();
        //        var BDList = _dbContext.ClassifiedBusiness.Where(c => c.BusinessCategoryId == BDFilterVm.BusinessCategoryId).Select(c => new
        //        {
        //            ClassifiedBusinessId = c.ClassifiedBusinessId,
        //            BusinessCategoryId = c.BusinessCategoryId,
        //            IsActive = c.IsActive,
        //            Title = c.Title,
        //            Description = c.Description,
        //            MainPic = c.Mainpic,
        //            Businessworkinghours = _dbContext.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(a => new
        //            {
        //                a.Day,
        //                a.EndTime1,
        //                a.EndTime2,
        //                a.Isclosed,
        //                a.StartTime1,
        //                a.StartTime2
        //            }).ToList(),
        //            PublishDate = c.PublishDate,
        //            Views = c.Views,
        //            Reviews = _dbContext.Reviews.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
        //            {
        //                ClassifiedBusinessId = l.ClassifiedBusinessId,
        //                Name = l.Name,
        //                Rating = l.Rating,
        //                ReviewDate = l.ReviewDate,
        //                ReviewId = l.ReviewId,
        //                Title = l.Title,
        //                Email = l.Email,
        //            }).ToList(),
        //            //IsFavourite = _dbContext.FavouriteBusiness.Any(o => o.ClassifiedBusinessId == c.ClassifiedBusinessId && o.UserId == userId),
        //            City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
        //            {
        //                k.CityId,
        //                k.CityTlAr,
        //                k.CityTlEn,

        //            }).FirstOrDefault(),
        //            Area = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
        //            {
        //                k.AreaId,
        //                k.AreaTlAr,
        //                k.AreaTlEn,

        //            }).FirstOrDefault(),
        //            Address = c.Address,
        //            Logo = c.Logo,
        //            phone = c.phone,
        //            Email = c.Email,

        //            BusinessContents = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
        //            {
        //                BusinessContentId = l.BusinessContentId,
        //                BusinessTemplateConfigId = l.BusinessTemplateConfigId,
        //                AdContentValues = l.BusinessContentValues.Select(k => new
        //                {
        //                    BusinessContentValueId = k.BusinessContentValueId,
        //                    ContentValue = (_dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 6) ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
        //                    BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,
        //                })
        //            })
        //                .ToList(),

        //        }).ToList();

        //        if (BDList != null)
        //        {
        //            if (BDFilterVm.CityId != 0)
        //            {
        //                BDList = BDList.Where(e => e.Area.AreaId == BDFilterVm.CityId).ToList();
        //            }
        //            if (BDFilterVm.AreaId != 0)
        //            {
        //                BDList = BDList.Where(e => e.City.CityId == BDFilterVm.AreaId).ToList();
        //            }
        //            if (BDFilterVm.Title != null)
        //            {
        //                BDList = BDList.Where(e => e.Title.ToUpper().Contains(BDFilterVm.Title.ToUpper())).ToList();
        //            }
        //            if (BDFilterVm.Phone != null)
        //            {
        //                BDList = BDList.Where(e => e.phone.ToUpper().Contains(BDFilterVm.Phone.ToUpper())).ToList();
        //            }
        //            if (BDFilterVm.Address != null)
        //            {
        //                BDList = BDList.Where(e => e.Address.ToUpper().Contains(BDFilterVm.Address.ToUpper())).ToList();
        //            }
        //            if (BDFilterVm.Description != null)
        //            {
        //                BDList = BDList.Where(e => e.Description.ToUpper().Contains(BDFilterVm.Description.ToUpper())).ToList();
        //            }
        //        }

        //        if (BDList is null)
        //        {
        //            return NotFound();
        //        }
        //        BDList = BDList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //        return Ok(new { Status = true, BDList = BDList, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetFilterBD")]
        public async Task<IActionResult> GetFilterBD([FromForm] BDFilterModelVm BDFilterVm, int page = 1, int pageSize = 10)
        {


            try
            {
                var BDCatagory = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryId == BDFilterVm.BusinessCategoryId).FirstOrDefault();
                if (BDCatagory == null)
                {
                    return Ok(new { Status = false, Message = "Catagory Not Found" });
                }

                var BDList = _dbContext.ClassifiedBusiness.Include(e => e.BusiniessSubscriptions).Where(c => c.BusinessCategoryId == BDFilterVm.BusinessCategoryId && c.IsActive == true && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().IsActive && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate >= DateTime.Now).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    PublishDate = c.PublishDate,
                    Views = c.Views,
                    City = c.CityId,
                    Area = c.AreaId,
                    Address = c.Address,
                    Logo = c.Logo,
                    phone = c.phone,
                    Email = c.Email,
                }).ToList();

                if (BDList != null)
                {
                    if (BDFilterVm.CityId != 0)
                    {
                        BDList = BDList.Where(e => e.City == BDFilterVm.CityId).ToList();
                    }
                    if (BDFilterVm.AreaId != 0)
                    {
                        BDList = BDList.Where(e => e.Area == BDFilterVm.AreaId).ToList();
                    }
                    if (BDFilterVm.Title != null)
                    {
                        BDList = BDList.Where(e => e.Title.ToUpper().Contains(BDFilterVm.Title.ToUpper())).ToList();
                    }
                    if (BDFilterVm.Phone != null)
                    {
                        BDList = BDList.Where(e => e.phone.ToUpper().Contains(BDFilterVm.Phone.ToUpper())).ToList();
                    }
                    if (BDFilterVm.Address != null)
                    {
                        BDList = BDList.Where(e => e.Address.ToUpper().Contains(BDFilterVm.Address.ToUpper())).ToList();
                    }
                    if (BDFilterVm.Description != null)
                    {
                        BDList = BDList.Where(e => e.Description.ToUpper().Contains(BDFilterVm.Description.ToUpper())).ToList();
                    }
                }

                if (BDList is null)
                {
                    return NotFound();
                }
                BDList = BDList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new { Status = true, BDList = BDList, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }

        [HttpGet]
        [Route("GetFilterService")]
        public async Task<IActionResult> GetFilterService([FromForm] ServiceFilterVm serviceFilterVm, int page = 1, int pageSize = 10)
        {


            try
            {
                var serviceCategory = _dbContext.ServiceCatagories.Where(e => e.ServiceCatagoryId == serviceFilterVm.ServiceCatagoryId).FirstOrDefault();
                if (serviceCategory == null)
                {
                    return Ok(new { Status = false, Message = "Service Category Not Found" });

                }
                var services = _dbContext.Services.Include(c => c.ServiceImages).Where(c => c.ServiceCatagoryId == serviceFilterVm.ServiceCatagoryId && c.IsActive == true).OrderBy(e => e.SortOrder).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceCatagoryId = c.ServiceCatagoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    ServiceTitleAr = c.ServiceTitleAr,
                    ServiceTitleEn = c.ServiceTitleEn,
                    ServiceTypeId = c.ServiceTypeId,
                    MainPic = c.MainPic,
                    //IsFavourite = _dbContext.ServiceFavourites.Any(o => o.ServiceId == c.ServiceId && o.UserId == userId),

                    ServiceImages = c.ServiceImages.Select(i => new
                    {
                        ServiceImageId = i.ServiceImageId,
                        Image = i.Image
                    }).ToList(),


                    ServiceContents = _dbContext.ServiceContents.Where(e => e.ServiceId == c.ServiceId).Select(l => new
                    {
                        ServiceContentId = l.ServiceContentId,
                        ServiceTemplateConfigId = l.ServiceTemplateConfigId,
                        ServiceContentValues = l.ServiceContentValues.Select(k => new
                        {
                            ServiceContentValueId = k.ServiceContentValueId,
                            ContentValue = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.ServiceTemplateOptions.Where(e => e.ServiceTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ServiceTemplateFieldCaptionAr = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionAr,
                            ServiceTemplateFieldCaptionEn = _dbContext.ServiceTemplateConfigs.Where(e => e.ServiceTemplateConfigId == l.ServiceTemplateConfigId).FirstOrDefault().ServiceTemplateFieldCaptionEn,
                        })
                    })
                                  .ToList(),

                }).ToList();

                if (services != null)
                {
                    if (serviceFilterVm.ServiceTitle != null)
                    {
                        services = services.Where(e => e.ServiceTitleAr.ToUpper().Contains(serviceFilterVm.ServiceTitle.ToUpper()) || e.ServiceTitleEn.ToUpper().Contains(serviceFilterVm.ServiceTitle.ToUpper())).ToList();
                    }
                    if (serviceFilterVm.FromPrice != 0 && serviceFilterVm.ToPrice != 0)
                    {
                        services = services.Where(e => e.Price > serviceFilterVm.FromPrice && e.Price < serviceFilterVm.ToPrice).ToList();
                    }

                }

                if (services is null)
                {
                    return NotFound();
                }
                services = services.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new { Status = true, services = services, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetFilterProduct")]
        public async Task<IActionResult> GetFilterProduct([FromForm] FilterProductVm productFilterVm, int page = 1, int pageSize = 10)
        {


            try
            {
                var ProductCatagory = _dbContext.ProductCategories.Where(e => e.ProductCategoryId == productFilterVm.ProductCategoryId).FirstOrDefault();
                if (ProductCatagory == null)
                {
                    return Ok(new { Status = false, Message = "Product Catagory Not Found" });
                }

                List<long> ProductIds = new List<long>();
                var Products = _dbContext.Products.Include(c => c.ProductExtras).Include(c => c.ProductPrices).Where(c => c.ProductCategoryId == productFilterVm.ProductCategoryId && c.IsActive == true).Include(c => c.ProductContents).ThenInclude(c => c.ProductContentValues).Select(c => new
                {
                    ProductId = c.ProductId,
                    ProductCategoryId = c.ProductCategoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    //ProductName = c.ProductName,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    IsFixedPrice = c.IsFixedPrice,
                    ProductTypeId = c.ProductTypeId,
                    MainPic = c.MainPic,
                    ProductExtras = c.ProductExtras,
                    ProductPrices = c.ProductPrices,

                    ProductContents = _dbContext.ProductContents.Where(e => e.ProductId == c.ProductId).Select(l => new
                    {
                        ProductContentId = l.ProductContentId,
                        ProductTemplateConfigId = l.ProductTemplateConfigId,
                        ProductContentValues = l.ProductContentValues.Select(k => new
                        {
                            ProductContentValueId = k.ProductContentValueId,
                            ContentValue = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _dbContext.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                            FieldTypeId = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ProductTemplateFieldCaptionAr = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionAr,
                            ProductTemplateFieldCaptionEn = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionEn,
                        })
                    })
                            .ToList(),

                }).ToList();


                if (Products != null)
                {
                    if (productFilterVm.ProductName != null)
                    {
                        Products = Products.Where(e => e.TitleEn.ToUpper().Contains(productFilterVm.ProductName.ToUpper()) || e.TitleAr.ToUpper().Contains(productFilterVm.ProductName.ToUpper())).ToList();
                    }


                    if (productFilterVm.FromPrice != 0 && productFilterVm.ToPrice != 0)
                    {
                        Products = Products.Where(e => e.Price > productFilterVm.FromPrice && e.Price < productFilterVm.ToPrice).ToList();
                    }
                    if (productFilterVm.productContentModelVms != null)
                    {
                        foreach (var item in productFilterVm.productContentModelVms)
                        {
                            if (item.ProductTemplateConfigId != 0)
                            {
                                var Values = await _dbContext.ProductContentValues.Include(e => e.ProductContent).Where(e => e.ProductContent.ProductTemplateConfigId == item.ProductTemplateConfigId && item.Values.Contains(e.ContentValue)).Select(e => e.ContentValue).ToListAsync();
                                ProductIds = _dbContext.ProductContents.Include(e => e.ProductContentValues).Where(e => Values.Contains(e.ProductContentValues.FirstOrDefault().ContentValue)).Select(e => e.ProductId).ToList();
                                Products = Products.Where(e => ProductIds.Contains(e.ProductId)).ToList();
                            }

                        }
                    }




                }

                if (Products is null)
                {
                    return NotFound();
                }
                Products = Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new { Status = true, Products = Products, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }

        //[HttpGet]
        //[Route("GetFilterBD")]
        //public async Task<IActionResult> GetFilterBD([FromForm] BDFilterModelVm BDFilterVm, int page = 1, int pageSize = 10)
        //{


        //    try
        //    {
        //        var BDCatagory = _dbContext.ClassifiedBusiness.Where(e => e.BusinessCategoryId == BDFilterVm.BusinessCategoryId).FirstOrDefault();
        //        if (BDCatagory == null)
        //        {
        //            return Ok(new { Status = false, Message = "Catagory Not Found" });
        //        }

        //        // List<long> BDIds = new List<long>();
        //        var BDList = _dbContext.ClassifiedBusiness.Where(c => c.BusinessCategoryId == BDFilterVm.BusinessCategoryId).Select(c => new
        //        {
        //            ClassifiedBusinessId = c.ClassifiedBusinessId,
        //            BusinessCategoryId = c.BusinessCategoryId,
        //            IsActive = c.IsActive,
        //            Title = c.Title,
        //            Description = c.Description,
        //            MainPic = c.Mainpic,
        //            Businessworkinghours = _dbContext.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(a => new
        //            {
        //                a.Day,
        //                a.EndTime1,
        //                a.EndTime2,
        //                a.Isclosed,
        //                a.StartTime1,
        //                a.StartTime2
        //            }).ToList(),
        //            PublishDate = c.PublishDate,
        //            Views = c.Views,
        //            Reviews = _dbContext.Reviews.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
        //            {
        //                ClassifiedBusinessId = l.ClassifiedBusinessId,
        //                Name = l.Name,
        //                Rating = l.Rating,
        //                ReviewDate = l.ReviewDate,
        //                ReviewId = l.ReviewId,
        //                Title = l.Title,
        //                Email = l.Email,
        //            }).ToList(),
        //            //IsFavourite = _dbContext.FavouriteBusiness.Any(o => o.ClassifiedBusinessId == c.ClassifiedBusinessId && o.UserId == userId),
        //            City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
        //            {
        //                k.CityId,
        //                k.CityTlAr,
        //                k.CityTlEn,

        //            }).FirstOrDefault(),
        //            Area = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
        //            {
        //                k.AreaId,
        //                k.AreaTlAr,
        //                k.AreaTlEn,

        //            }).FirstOrDefault(),
        //            Address = c.Address,
        //            Logo = c.Logo,
        //            phone = c.phone,
        //            Email = c.Email,

        //            BusinessContents = _dbContext.BusinessContents.Where(e => e.ClassifiedBusinessId == c.ClassifiedBusinessId).Select(l => new
        //            {
        //                BusinessContentId = l.BusinessContentId,
        //                BusinessTemplateConfigId = l.BusinessTemplateConfigId,
        //                AdContentValues = l.BusinessContentValues.Select(k => new
        //                {
        //                    BusinessContentValueId = k.BusinessContentValueId,
        //                    ContentValue = (_dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 13 || _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId == 6) ? _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
        //                    FieldTypeId = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().FieldTypeId,
        //                    BusinessTemplateFieldCaptionAr = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr,
        //                    BusinessTemplateFieldCaptionEn = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == l.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn,
        //                })
        //            })
        //                .ToList(),

        //        }).ToList();

        //        if (BDList != null)
        //        {
        //            if (BDFilterVm.CityId != 0)
        //            {
        //                BDList = BDList.Where(e => e.Area.AreaId == BDFilterVm.CityId).ToList();
        //            }
        //            if (BDFilterVm.AreaId != 0)
        //            {
        //                BDList = BDList.Where(e => e.City.CityId == BDFilterVm.AreaId).ToList();
        //            }
        //            if (BDFilterVm.Title != null)
        //            {
        //                BDList = BDList.Where(e => e.Title.ToUpper().Contains(BDFilterVm.Title.ToUpper())).ToList();
        //            }
        //            if (BDFilterVm.Phone != null)
        //            {
        //                BDList = BDList.Where(e => e.phone.ToUpper().Contains(BDFilterVm.Phone.ToUpper())).ToList();
        //            }
        //            if (BDFilterVm.Address != null)
        //            {
        //                BDList = BDList.Where(e => e.Address.ToUpper().Contains(BDFilterVm.Address.ToUpper())).ToList();
        //            }
        //            if (BDFilterVm.Description != null)
        //            {
        //                BDList = BDList.Where(e => e.Description.ToUpper().Contains(BDFilterVm.Description.ToUpper())).ToList();
        //            }
        //        }

        //        if (BDList is null)
        //        {
        //            return NotFound();
        //        }
        //        BDList = BDList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //        return Ok(new { Status = true, BDList = BDList, Message = "Process completed successfully" });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetProductDetailsByIdForEdit")]
        public async Task<IActionResult> GetProductDetailsByIdForEdit(int ProductId, string userId)
        {
            try
            {


                var Product = _dbContext.Products.Where(c => c.ProductId == ProductId).Include(c => c.ProductExtras).Include(c => c.ProductPrices).Select(c => new
                {
                    ProductId = c.ProductId,
                    ProductCategoryId = c.ProductCategoryId,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    SortOrder = c.SortOrder,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    //ProductName = c.ProductName,
                    IsFixedPrice = c.IsFixedPrice,
                    ProductTypeId = c.ProductTypeId,
                    MainPic = c.MainPic,
                    ProductExtras = c.ProductExtras,
                    ProductPrices = c.ProductPrices,
                    IsFavourite = _dbContext.FavouriteProducts.Any(o => o.ProductId == ProductId && o.UserId == userId),
                    Reel = c.Reel,

                    ProductContents = _dbContext.ProductContents.Where(e => e.ProductId == c.ProductId).Select(l => new
                    {
                        ProductContentId = l.ProductContentId,
                        ProductTemplateConfigId = l.ProductTemplateConfigId,
                        ProductContentValues = l.ProductContentValues.Select(k => new
                        {
                            ProductContentValueId = k.ProductContentValueId,
                            ContentValue = k.ContentValue,
                            FieldTypeId = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId,
                            ProductTemplateFieldCaptionAr = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionAr,
                            ProductTemplateFieldCaptionEn = _dbContext.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionEn,
                        })
                    })
                          .ToList(),

                }).FirstOrDefault();


                if (Product is null)
                {
                    return NotFound(new { Status = false, Message = "There is no Product" });
                }

                return Ok(new { Status = true, Product = Product });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetAllSlidersByInpdexPageId")]
        public ActionResult GetAllSlidersByInpdexPageId(int PageIndexId)
        {

            try
            {

                var Sliders = _dbContext.Sliders.Where(e => e.IsActive == true && e.PageIndex == PageIndexId).OrderBy(e => e.OrderIndex).Select(c => new
                {
                    SliderId = c.SliderId,
                    Pic = c.Pic,
                    EntityId = c.EntityId,
                    EntityTypeId = c.EntityTypeId

                }).ToList();



                return Ok(new { Status = true, Sliders = Sliders });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        //[HttpGet]
        //[Route("GetRemoveDupplicate")]
        //public ActionResult GetRemoveDupplicate(int catId)
        //{
        //    try
        //    {

        //    var cats = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryParentId == catId).ToList();
        //        foreach (var item in cats)
        //        {
        //            var adTemplte = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == item.BusinessCategoryId).ToList();
        //            if (adTemplte != null)
        //            {
        //                if (adTemplte.Count == 34)
        //                {
        //                    for(int i = 1; i <= adTemplte.Count; i++)
        //                    {
        //                        if (i>17&&i<34)
        //                        {
        //                            var options = _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == adTemplte[i].BusinessTemplateConfigId).ToList();
        //                            if (options != null)
        //                            {
        //                                _dbContext.BusinessTemplateOptions.RemoveRange(options);
        //                                _dbContext.SaveChanges();
        //                            }
        //                            _dbContext.BusinessTemplateConfigs.Remove(adTemplte[i]);
        //                            _dbContext.SaveChanges();
        //                        }

        //                    }
        //                }
        //            }

        //        }



        //        return Ok(new { Status = true });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        /// <summary>
        /// //Remove Du
        /// </summary>
        /// <returns></returns>
        //[HttpDelete]
        //[Route("RemoveDupplicateCatagory")]
        //public ActionResult RemoveDupplicateCatagory(int BcatId)
        //{
        //    try
        //    {


        //        var Catgory = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryId == BcatId).FirstOrDefault();
        //        var BDTemplte = _dbContext.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == BcatId).ToList();
        //        if (BDTemplte != null)
        //        {
        //            foreach (var item in BDTemplte)
        //            {

        //                var options = _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == item.BusinessTemplateConfigId).ToList();
        //                if (options != null)
        //                {
        //                    _dbContext.BusinessTemplateOptions.RemoveRange(options);


        //                }
        //                _dbContext.BusinessTemplateConfigs.RemoveRange(item);
        //            }
        //            //_dbContext.BusinessCategories.RemoveRange(Catgory);
        //            _dbContext.SaveChanges();

        //            //for (int i = 1; i <= adTemplte.Count; i++)
        //            //{
        //            //    if (i > 17 && i < 34)
        //            //    {
        //            //        var options = _dbContext.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == adTemplte[i].BusinessTemplateConfigId).ToList();
        //            //        if (options != null)
        //            //        {
        //            //            _dbContext.BusinessTemplateOptions.RemoveRange(options);
        //            //            _dbContext.SaveChanges();
        //            //        }
        //            //        _dbContext.BusinessTemplateConfigs.Remove(adTemplte[i]);
        //            //        _dbContext.SaveChanges();
        //            //    }

        //            //}

        //        }





        //        return Ok(new { Status = true });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { Status = false, Message = ex.Message });
        //    }

        //}
        [HttpGet]
        [Route("GetNewBDSCategories")]
        public async Task<ActionResult> GetNewBDSCategories()
        {
            try
            {

                var categories = _dbContext.BusinessCategories.ToList();

                // get top-level categories
                var parents = categories.Where(c => c.BusinessCategoryParentId == null);

                // map top-level categories and their children to DTOs
                var dtos = parents.Select(c => new
                {
                    BusinessCategoryId = c.BusinessCategoryId,
                    BusinessCategoryTitleAr = c.BusinessCategoryTitleAr,
                    BusinessCategoryTitleEn = c.BusinessCategoryTitleEn,
                    BusinessCategorySortOrder = c.BusinessCategorySortOrder,
                    BusinessCategoryCategoryPic = c.BusinessCategoryCategoryPic,
                    BusinessCategoryIsActive = c.BusinessCategoryIsActive,
                    BusinessCategoryDescAr = c.BusinessCategoryDescAr,
                    BusinessCategoryDescEn = c.BusinessCategoryDescEn,
                    BusinessCategoryParentId = c.BusinessCategoryParentId,
                    Children = categories.Where(a => a.BusinessCategoryParentId == c.BusinessCategoryId)
                                         .Select(b => new
                                         {
                                             BusinessCategoryId = b.BusinessCategoryId,
                                             BusinessCategoryTitleAr = b.BusinessCategoryTitleAr,
                                             BusinessCategoryTitleEn = b.BusinessCategoryTitleEn,
                                             BusinessCategorySortOrder = b.BusinessCategorySortOrder,
                                             BusinessCategoryCategoryPic = b.BusinessCategoryCategoryPic,
                                             BusinessCategoryIsActive = b.BusinessCategoryIsActive,
                                             BusinessCategoryDescAr = b.BusinessCategoryDescAr,
                                             BusinessCategoryDescEn = b.BusinessCategoryDescEn,
                                             BusinessCategoryParentId = b.BusinessCategoryParentId,
                                             Children = categories.Where(M => M.BusinessCategoryParentId == b.BusinessCategoryId)
                                                                  .Select(gc => new
                                                                  {
                                                                      BusinessCategoryId = gc.BusinessCategoryId,
                                                                      BusinessCategoryTitleAr = gc.BusinessCategoryTitleAr,
                                                                      BusinessCategoryTitleEn = gc.BusinessCategoryTitleEn,
                                                                      BusinessCategorySortOrder = gc.BusinessCategorySortOrder,
                                                                      BusinessCategoryCategoryPic = gc.BusinessCategoryCategoryPic,
                                                                      BusinessCategoryIsActive = gc.BusinessCategoryIsActive,
                                                                      BusinessCategoryDescAr = gc.BusinessCategoryDescAr,
                                                                      BusinessCategoryDescEn = gc.BusinessCategoryDescEn,
                                                                      BusinessCategoryParentId = gc.BusinessCategoryParentId,
                                                                      Children = categories.Where(K => K.BusinessCategoryParentId == gc.BusinessCategoryId)
                                                                  .Select(gcM => new
                                                                  {
                                                                      BusinessCategoryId = gcM.BusinessCategoryId,
                                                                      BusinessCategoryTitleAr = gcM.BusinessCategoryTitleAr,
                                                                      BusinessCategoryTitleEn = gcM.BusinessCategoryTitleEn,
                                                                      BusinessCategorySortOrder = gcM.BusinessCategorySortOrder,
                                                                      BusinessCategoryIsActive = gcM.BusinessCategoryIsActive,
                                                                      BusinessCategoryCategoryPic = gcM.BusinessCategoryCategoryPic,
                                                                      BusinessCategoryDescAr = gcM.BusinessCategoryDescAr,
                                                                      BusinessCategoryDescEn = gcM.BusinessCategoryDescEn,
                                                                      BusinessCategoryParentId = gcM.BusinessCategoryParentId,


                                                                  })

                                                                  })

                                         })
                });





                return Ok(new { Status = true, dtos, Message = "Process completed successfully" });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = "Something went wrong" });
            }

        }
        //[HttpGet]
        //[Route("GetTopBDOffers")]
        //public IActionResult GetTopBDOffers()
        //{
        //    try
        //    {


        //        var BDOffers = _dbContext.BDOffers.Include(e => e.BDOfferImages).Include(e => e.ClassifiedBusiness).ThenInclude(e => e.BusinessCategory).OrderByDescending(e => e.BDOfferId).Take(50).Select(i => new
        //        {
        //            BDOfferId = i.BDOfferId,
        //            TitleAr = i.TitleAr,
        //            TitleEn = i.TitleEn,
        //            OfferDescription = i.OfferDescription,
        //            Price = i.Price,
        //            PublishDate = i.PublishDate,
        //            Pic = i.Pic,
        //            BusinessCategoryId = i.ClassifiedBusiness.BusinessCategoryId,
        //            BusinessCategoryTitleAr = i.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleAr,
        //            BusinessCategoryTitleEn = i.ClassifiedBusiness.BusinessCategory.BusinessCategoryTitleEn,

        //            BDOfferImages = i.BDOfferImages.Select(c => new
        //            {
        //                c.BDOfferImageId,
        //                c.Image,
        //            }).ToList(),

        //        }
        //        ).ToList();
        //        return Ok(new { Status = true, BDOffers = BDOffers });

        //    }
        //    catch (Exception ex)
        //    {

        //        return Ok(new { Status = false, Message = ex.Message });
        //    }
        //}
        [HttpGet]
        [Route("GetLatestCompanies")]
        public async Task<IActionResult> GetLatestCompanies(string userId)
        {
            try
            {

                var LtsCompanies = _dbContext.ClassifiedBusiness.Include(e => e.BusiniessSubscriptions).Where(c => c.IsActive == true && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().IsActive && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate >= DateTime.Now).OrderByDescending(c => c.ClassifiedBusinessId).Take(50).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Logo = c.Logo,
                    Rating = c.Rating,
                    UseId = c.UseId,
                    Views = c.Views,
                    IsFavourite = _dbContext.FavouriteBusiness.Any(o => o.ClassifiedBusinessId == c.ClassifiedBusinessId && o.UserId == userId),


                }).ToListAsync();




                return Ok(new { Status = true, LtsCompanies = LtsCompanies });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetLatestProductAndServices")]
        public async Task<IActionResult> GetLatestProductAndServices(string userId)
        {
            try
            {

                var LtsServices = _dbContext.Services.Include(c => c.ServiceCatagory).Where(c => c.IsActive == true).OrderByDescending(c => c.ServiceId).Take(25).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    Price = c.Price,
                    ServiceTitleAr = c.ServiceTitleAr,
                    ServiceTitleEn = c.ServiceTitleEn,
                    Description = c.Description,
                    MainPic = c.MainPic,
                    ServiceCatagoryAr = c.ServiceCatagory.ServiceCatagoryTitleAr,
                    ServiceCatagoryEn = c.ServiceCatagory.ServiceCatagoryTitleEn,
                    IsFavourite = _dbContext.ServiceFavourites.Any(o => o.ServiceId == c.ServiceId && o.UserId == userId)

                }).ToList();
                var LtsProduct = _dbContext.Products.Include(c => c.ProductCategory).Where(c => c.IsActive == true).OrderByDescending(c => c.ProductId).Take(25).Select(c => new
                {
                    ProductId = c.ProductId,
                    Price = c.Price,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    Mainpic = c.MainPic,
                    ProductCategoryAr = c.ProductCategory.TitleAr,
                    ProductCategoryEn = c.ProductCategory.TitleEn,
                    IsFavourite = _dbContext.FavouriteProducts.Any(o => o.ProductId == c.ProductId && o.UserId == userId)

                }).ToList();
                List<LatestProductService> LatestProductServices = new List<LatestProductService>();
                foreach (var ser in LtsServices)
                {

                    var Service = new LatestProductService()
                    {
                        Id = ser.ServiceId,
                        Price = ser.Price,
                        TitleAr = ser.ServiceTitleAr,
                        TitleEn = ser.ServiceTitleEn,
                        Description = ser.Description,
                        MainPic = ser.MainPic,
                        CatagoryTitleAr = ser.ServiceCatagoryAr,
                        CatagoryTitleEn = ser.ServiceCatagoryEn,
                        IsFavourite = ser.IsFavourite,
                        TypeId = 1
                    };
                    LatestProductServices.Add(Service);
                }
                foreach (var pro in LtsProduct)
                {
                    var product = new LatestProductService()
                    {
                        Id = pro.ProductId,
                        Price = pro.Price.Value,
                        TitleAr = pro.TitleAr,
                        TitleEn = pro.TitleEn,
                        Description = pro.Description,
                        MainPic = pro.Mainpic,
                        CatagoryTitleAr = pro.ProductCategoryAr,
                        CatagoryTitleEn = pro.ProductCategoryEn,
                        IsFavourite = pro.IsFavourite,
                        TypeId = 2
                    };
                    LatestProductServices.Add(product);
                }
                var rnd = new Random();
                var RandomLatestProductServices = LatestProductServices.OrderBy(x => rnd.Next());
                return Ok(new { Status = true, LatestProductServices = RandomLatestProductServices });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetBestSeller")]
        public async Task<IActionResult> GetBestSeller(string userId)
        {
            try
            {

                var BestProducts = _dbContext.OrderItems
                                    .GroupBy(q => q.ProductId)
                                    .OrderByDescending(gp => gp.Count())
                                    .Take(50)
                                    .Select(g => g.Key).ToList();

                var BestProductSellerLst = _dbContext.Products.Include(c => c.ProductCategory).Where(c => BestProducts.Contains(c.ProductId) && c.IsActive == true).OrderByDescending(c => c.ProductId).Take(50).Select(c => new
                {
                    ProductId = c.ProductId,
                    Price = c.Price,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,
                    Description = c.Description,
                    Mainpic = c.MainPic,
                    ProductCategoryAr = c.ProductCategory.TitleAr,
                    ProductCategoryEn = c.ProductCategory.TitleEn,
                    IsFavourite = _dbContext.FavouriteProducts.Any(o => o.ProductId == c.ProductId && o.UserId == userId)


                }).ToList();




                return Ok(new { Status = true, BestSellertLst = BestProductSellerLst });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetBDSlidersByInpdexPageId")]
        public ActionResult GetBDSlidersByInpdexPageId(int PageIndexId)
        {

            try
            {

                var BDSliders = _dbContext.Sliders.Where(e => e.IsActive == true && e.PageIndex == PageIndexId && e.EntityTypeId == 2).OrderBy(e => e.OrderIndex).Select(c => new
                {
                    SliderId = c.SliderId,
                    Pic = c.Pic,
                    EntityId = c.EntityId,
                    //EntityTypeId = c.EntityTypeId

                }).ToList();



                return Ok(new { Status = true, BDSliders = BDSliders });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetSearchForBDS")]
        public async Task<IActionResult> GetSearchForBDS(string Search, int page = 1, int pageSize = 10)
        {
            try
            {

                var BDServiceList = _dbContext.Services.Include(e => e.ServiceCatagory).Where(e => e.ServiceCatagory.ServiceCatagoryTitleAr.ToUpper().Contains(Search.ToUpper()) || e.ServiceCatagory.ServiceCatagoryTitleEn.ToUpper().Contains(Search.ToUpper()) || e.ServiceTitleAr.ToUpper().Contains(Search.ToUpper()) || e.ServiceTitleEn.ToUpper().Contains(Search.ToUpper())).Select(e => e.ServiceCatagory.ClassifiedBusinessId).ToList();
                var BDProductList = _dbContext.Products.Include(e => e.ProductCategory).Where(e => e.ProductCategory.TitleAr.ToUpper().Contains(Search.ToUpper()) || e.ProductCategory.TitleEn.ToUpper().Contains(Search.ToUpper()) || e.TitleAr.ToUpper().Contains(Search.ToUpper()) || e.TitleEn.ToUpper().Contains(Search.ToUpper())).Select(e => e.ProductCategory.ClassifiedBusinessId).ToList();
                var BDList = _dbContext.ClassifiedBusiness.Include(e => e.BusiniessSubscriptions).Where(c => c.IsActive == true && BDServiceList.Contains(c.ClassifiedBusinessId) || BDProductList.Contains(c.ClassifiedBusinessId) || c.Title.ToUpper().Contains(Search.ToUpper()) || c.Description.ToUpper().Contains(Search.ToUpper()) && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().IsActive && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate >= DateTime.Now).OrderByDescending(c => c.ClassifiedBusinessId).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Logo = c.Logo,
                    Rating = c.Rating,
                    UseId = c.UseId,
                    Views = c.Views,


                }).Skip((page - 1) * pageSize).Take(pageSize).ToList();




                return Ok(new { Status = true, BDList = BDList });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetBDStatisticsDashboard")]
        public async Task<IActionResult> GetBDStatisticsDashboard(long BDId)
        {
            var BD = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == BDId).FirstOrDefault();
            if (BD == null)
            {
                return Ok(new { Status = false, message = "Bussiness Directory Not Found" });
            }
            try
            {
                var ProductCatagoryCount = _dbContext.ProductCategories.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var ServiceCatagoryCount = _dbContext.ServiceCatagories.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var OffersCount = _dbContext.BDOffers.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var ProductsCount = _dbContext.Products.Include(e => e.ProductCategory).Where(e => e.ProductCategory.ClassifiedBusinessId == BDId).Count();
                var ServicesCount = _dbContext.Services.Include(e => e.ServiceCatagory).Where(e => e.ServiceCatagory.ClassifiedBusinessId == BDId).Count();
                var OrdersCount = _dbContext.Orders.Where(e => e.ClassifiedBusinessId == BDId).Count();
                var OrdersAmount = _dbContext.Orders.Where(e => e.ClassifiedBusinessId == BDId).Sum(e => e.OrderTotal);
                return Ok(new { Status = true, BDViews = BD.Views, ProductCatagoryCount = ProductCatagoryCount, ServiceCatagoryCount = ServiceCatagoryCount, OffersCount = OffersCount, ProductsCount = ProductsCount, ServiceCount = ServicesCount, OrdersCount = OrdersCount, OrdersAmount = OrdersAmount });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = true, message = ex.Message });

            }

        }
		//[HttpGet]
		//[Route("GetSearchForBDSByCatagory")]
		//public async Task<IActionResult> GetSearchForBDSByCatagory(int CatagoryId, string Search, int page = 1, int pageSize = 10)
		//{
		//	try
		//	{
		//		var catObj = _dbContext.BusinessCategories.FirstOrDefault(e => e.BusinessCategoryId == CatagoryId);
		//		if (catObj == null)
		//		{
		//			return Ok(new { Status = false, message = "Category Not Found" });
		//		}

		//		var catList = _dbContext.BusinessCategories
		//			.Where(e => e.BusinessCategoryParentId == CatagoryId)
		//			.Select(e => e.BusinessCategoryId)
		//			.ToList();

		//		var BDList = _dbContext.ClassifiedBusiness
		//			.Include(e => e.BusiniessSubscriptions)
		//			.Where(c =>
		//				c.IsActive == true &&
		//				(c.BusinessCategoryId == CatagoryId || catList.Contains(c.BusinessCategoryId.Value)) &&
		//				(c.Title.ToUpper().Contains(Search.ToUpper()) || c.Description.ToUpper().Contains(Search.ToUpper()))
		//			)
		//			.Select(c => new
		//			{
		//				ClassifiedBusinessId = c.ClassifiedBusinessId,
		//				BusinessCategoryId = c.BusinessCategoryId,
		//				IsActive = c.IsActive,
		//				PublishDate = c.PublishDate,
		//				Title = c.Title,
		//				Description = c.Description,
		//				MainPic = c.Mainpic,
		//				Logo = c.Logo,
		//				Rating = c.Rating,
		//				UseId = c.UseId,
		//				Views = c.Views,
		//				HasSubscription = _dbContext.BusiniessSubscriptions.Any(x => x.ClassifiedBusinessId == c.ClassifiedBusinessId)
		//			})
		//			.Skip((page - 1) * pageSize)
		//			.Take(pageSize)
		//			.ToList();

		//		return Ok(new { Status = true, BDList = BDList });
		//	}
		//	catch (Exception ex)
		//	{
		//		return Ok(new { Status = false, Message = ex.Message });
		//	}
		//}

        [HttpGet]
        [Route("GetSearchForBDSByCatagory")]
        public async Task<IActionResult> GetSearchForBDSByCatagory(int CatagoryId, string Search, int page = 1, int pageSize = 10)
        {
            try
            {
                var catObj = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryId == CatagoryId).FirstOrDefault();
                if (catObj == null)
                {
                    return Ok(new { Status = false, message = "Catagory Not Found" });
                }
                var catList = _dbContext.BusinessCategories.Where(e => e.BusinessCategoryParentId == CatagoryId).Select(e => e.BusinessCategoryId).ToList();

                //var BDServiceList = _dbContext.Services.Include(e => e.ServiceCatagory).ThenInclude(e=>e.ClassifiedBusiness).Where(e =>e.ServiceCatagory.ClassifiedBusiness.BusinessCategoryId==CatagoryId|| catList.Contains(e.ServiceCatagory.ClassifiedBusiness.BusinessCategoryId.Value) && e.ServiceCatagory.ServiceCatagoryTitleAr.ToUpper().Contains(Search.ToUpper()) || e.ServiceCatagory.ServiceCatagoryTitleEn.ToUpper().Contains(Search.ToUpper()) || e.ServiceTitleAr.ToUpper().Contains(Search.ToUpper()) || e.ServiceTitleEn.ToUpper().Contains(Search.ToUpper())).Select(e => e.ServiceCatagory.ClassifiedBusinessId).ToList();
                //var BDProductList = _dbContext.Products.Include(e => e.ProductCategory).ThenInclude(e => e.ClassifiedBusiness).Where(e => e.ProductCategory.ClassifiedBusiness.BusinessCategoryId == CatagoryId|| catList.Contains(e.ProductCategory.ClassifiedBusiness.BusinessCategoryId.Value) && e.ProductCategory.TitleAr.ToUpper().Contains(Search.ToUpper()) || e.ProductCategory.TitleEn.ToUpper().Contains(Search.ToUpper()) || e.TitleAr.ToUpper().Contains(Search.ToUpper()) || e.TitleEn.ToUpper().Contains(Search.ToUpper())).Select(e => e.ProductCategory.ClassifiedBusinessId).ToList();
                var BDList = _dbContext.ClassifiedBusiness.Include(e => e.BusiniessSubscriptions).Where(c => c.IsActive == true && c.BusinessCategoryId == CatagoryId && (c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().IsActive && c.BusiniessSubscriptions.OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate >= DateTime.Now) || catList.Contains(c.BusinessCategoryId.Value) && (c.Title.ToUpper().Contains(Search.ToUpper()) || c.Description.ToUpper().Contains(Search.ToUpper()))).Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    BusinessCategoryId = c.BusinessCategoryId,
                    IsActive = c.IsActive,
                    PublishDate = c.PublishDate,
                    Title = c.Title,
                    Description = c.Description,
                    MainPic = c.Mainpic,
                    Logo = c.Logo,
                    Rating = c.Rating,
                    UseId = c.UseId,
                    Views = c.Views,
                    HasSubscription = _dbContext.BusiniessSubscriptions.Any(x => x.ClassifiedBusinessId == c.ClassifiedBusinessId),

                }).Skip((page - 1) * pageSize).Take(pageSize).ToList();




                return Ok(new { Status = true, BDList = BDList });
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPost]
        [Route("ReOrder")]
        public async Task<IActionResult> ReOrder(int orderId, string userId)
        {
            try
            {
                var Order = _dbContext.Orders.Where(c => c.OrderId == orderId && c.UserId == userId).FirstOrDefault();
                if (Order == null)
                {
                    return Ok(new { Status = false, message = "Order Not Found" });
                }

                var user = await _userManager.FindByIdAsync(Order.UserId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });
                }
                int maxUniqe = 1;
                var newList = _dbContext.Orders.ToList();
                if (newList.Count > 0)
                {
                    maxUniqe = newList.Max(e => e.UniqeId);
                }
                var newOrder = new Order()
                {
                    Adress = Order.Adress,
                    ApartmentNumber = Order.ApartmentNumber,
                    Area = Order.Area,
                    Avenue = Order.Avenue,
                    Building = Order.Building,
                    ClassifiedBusinessId = Order.ClassifiedBusinessId,
                    CustomerAddressId = Order.CustomerAddressId,
                    Deliverycost = Order.Deliverycost,
                    Discount = Order.Discount,
                    Floor = Order.Floor,
                    Governorate = Order.Governorate,
                    IsCancelled = false,
                    IsPaid = false,
                    IsDeliverd = false,
                    Lat = Order.Lat,
                    Lng = Order.Lng,
                    OrderDate = DateTime.Now,
                    OrderNet = Order.OrderNet,
                    OrderNotes = Order.OrderNotes,
                    OrderTotal = Order.OrderTotal,
                    Piece = Order.Piece,
                    Street = Order.Street,
                    UserId = Order.UserId,
                    UniqeId = maxUniqe + 1,
                    OrderSerial = Guid.NewGuid().ToString() + "/" + DateTime.Now.Year,

                };
                var orderItems = _dbContext.OrderItems.Where(e => e.OrderId == orderId).ToList();
                if (orderItems != null)
                {
                    List<OrderItem> OrderItemList = new List<OrderItem>();
                    foreach (var item in orderItems)
                    {
                        List<OrderItemExtraProduct> OrderItemExtraProductList = new List<OrderItemExtraProduct>();
                        var Extra = _dbContext.OrderItemExtraProducts.Where(e => e.OrderItemId == item.OrderItemId).ToList();
                        if (Extra != null)
                        {
                            foreach (var ele in Extra)
                            {
                                OrderItemExtraProduct orderItemExtra = new OrderItemExtraProduct()
                                {
                                    OrderItemId = ele.OrderItemId,
                                    Price = ele.Price,
                                    ProductExtraId = ele.ProductExtraId,
                                };
                                OrderItemExtraProductList.Add(orderItemExtra);
                            }
                        }
                        OrderItem orderItem = new OrderItem()
                        {
                            ProductId = item.ProductId,
                            ItemPrice = item.ItemPrice,
                            ProductPriceId = item.ProductPriceId,
                            Total = item.Total,
                            ProductQuantity = item.ProductQuantity,
                            OrderItemExtraProducts = OrderItemExtraProductList

                        };
                        OrderItemList.Add(orderItem);
                    }
                    newOrder.OrderItems = OrderItemList;
                }
                _dbContext.Orders.Add(newOrder);
                _dbContext.SaveChanges();
                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];
                if (Fattorahstatus) // fattorah live
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = newOrder.OrderNet,
                        CallBackUrl = "https://albaheth.me/FattorahOrderSuccess",
                        ErrorUrl = "https://albaheth.me/FattorahOrderFailed",

                        UserDefinedField = newOrder.UniqeId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://api.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = true, paymentUrl = InvoiceRes.InvoiceURL });


                    }
                    else
                    {
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = false, Message = FattoraRes.Message });

                    }
                }
                else               //fattorah test
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = newOrder.OrderNet,
                        CallBackUrl = "https://albaheth.me/FattorahOrderSuccess",
                        ErrorUrl = "https://albaheth.me/FattorahOrderFailed",

                        UserDefinedField = newOrder.UniqeId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = true, paymentUrl = InvoiceRes.InvoiceURL });


                    }
                    else
                    {
                        //_dbContext.ShopingCartProductExtraFeatures.RemoveRange(customerShoppingCartExtraList);
                        //_dbContext.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        //_dbContext.SaveChanges();
                        return Ok(new { Status = false, Message = FattoraRes.Message });

                    }


                }
            }

            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }

        }
        [HttpPost]
        [Route("AddServiceQuotation")]
        public async Task<IActionResult> AddServiceQuotation(ServiceQuotationVM serviceQuotationVM)
        {
            try
            {
                var BD = _dbContext.ClassifiedBusiness.Where(a => a.ClassifiedBusinessId == serviceQuotationVM.BDId).FirstOrDefault();
                if (BD == null)
                {
                    return Ok(new { Status = false, message = "Bussiness Not Found" });
                }
                var Service = _dbContext.Services.Where(a => a.ServiceId == serviceQuotationVM.ServiceId).FirstOrDefault();
                if (Service == null)
                {
                    return Ok(new { Status = false, message = "Service Not Found" });
                }

                var user = await _userManager.FindByIdAsync(serviceQuotationVM.UserId);
                if (user == null)
                {
                    return Ok(new { Status = false, message = "User Not Found" });

                }
                var serviceQuotation = new ServiceQuotation()
                {
                    ServiceId = serviceQuotationVM.ServiceId,
                    BDId = serviceQuotationVM.BDId,
                    UserId = serviceQuotationVM.UserId,
                    Description = serviceQuotationVM.Description,
                    QuotationDate = DateTime.Now,
                    ServiceQuotationRequestStatusId = 1,
                };
                _dbContext.ServiceQuotations.Add(serviceQuotation);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, message = "Your Service Quotation Send Successfully" });



            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }


        }
        [HttpGet]
        [Route("GetAllServiceQuotationByBD")]
        public async Task<IActionResult> GetAllServiceQuotationByBD(long BDId)
        {
            try
            {
                var BD = _dbContext.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == BDId).FirstOrDefault();
                if (BD == null)
                {
                    return Ok(new { Status = false, message = "Bussiness Directory Not Found" });
                }

                var ServiceQuotations = await _dbContext.ServiceQuotations.Include(e => e.Service).Include(e => e.ServiceQuotationRequestStatus).Where(e => e.BDId == BDId).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceQuotationId = c.ServiceQuotationId,
                    QuotationDate = c.QuotationDate,
                    StatusTitleAr = c.ServiceQuotationRequestStatus.StatusTitleAr,
                    StatusTitleEn = c.ServiceQuotationRequestStatus.StatusTitleEn,
                    ServiceTitleAr = c.Service.ServiceTitleAr,
                    ServiceTitleEn = c.Service.ServiceTitleEn,
                    Description = c.Description,
                    User = _userManager.FindByIdAsync(c.UserId)

                }).ToListAsync();
                return Ok(new { Status = true, ServiceQuotations = ServiceQuotations });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetServiceQuotationById")]
        public async Task<IActionResult> GetServiceQuotationById(int ServiceQuotationId)
        {
            try
            {
                var serviceQuotation = _dbContext.ServiceQuotations.Include(e => e.Service).Include(e => e.ServiceQuotationRequestStatus).Where(e => e.ServiceQuotationId == ServiceQuotationId).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceQuotationId = c.ServiceQuotationId,
                    QuotationDate = c.QuotationDate,
                    StatusTitleAr = c.ServiceQuotationRequestStatus.StatusTitleAr,
                    StatusTitleEn = c.ServiceQuotationRequestStatus.StatusTitleEn,
                    ServiceTitleAr = c.Service.ServiceTitleAr,
                    ServiceTitleEn = c.Service.ServiceTitleEn,
                    Description = c.Description,
                    User = _userManager.FindByIdAsync(c.UserId)

                }).FirstOrDefaultAsync();
                if (serviceQuotation == null)
                {
                    return Ok(new { Status = false, message = "Service Quotation Not Found" });
                }

                return Ok(new { Status = true, serviceQuotation = serviceQuotation });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        [HttpPut]
        [Route("AcceptServiceQuotationRequest")]
        public async Task<IActionResult> AcceptServiceQuotationRequest(int ServiceQuotationId)
        {
            try
            {
                var serviceQuotation = _dbContext.ServiceQuotations.Where(a => a.ServiceQuotationId == ServiceQuotationId).FirstOrDefault();
                if (serviceQuotation == null)
                {
                    return Ok(new { Status = false, message = "Service Quotation Not Found" });
                }
                serviceQuotation.ServiceQuotationRequestStatusId = 2;
                _dbContext.Attach(serviceQuotation).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = true, message = "Service Quotation Request Accepted" });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }


        }
        [HttpPut]
        [Route("RejectServiceQuotationRequest")]
        public async Task<IActionResult> RejectServiceQuotationRequest(int ServiceQuotationId)
        {
            try
            {
                var serviceQuotation = _dbContext.ServiceQuotations.Where(a => a.ServiceQuotationId == ServiceQuotationId).FirstOrDefault();
                if (serviceQuotation == null)
                {
                    return Ok(new { Status = false, message = "Service Quotation Not Found" });
                }
                serviceQuotation.ServiceQuotationRequestStatusId = 3;
                _dbContext.Attach(serviceQuotation).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(new { Status = true, message = "Service Quotation Request Rejected" });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, message = e.Message });
            }


        }
        [HttpDelete]
        [Route("DeleteServiceQuotation")]
        public ActionResult DeleteServiceQuotation(int ServiceQuotationId)
        {
            try
            {
                var serviceQuotation = _dbContext.ServiceQuotations.Where(c => c.ServiceQuotationId == ServiceQuotationId).FirstOrDefault();
                if (serviceQuotation == null)
                {
                    return Ok(new { Status = false, Message = "Service Quotation Not Found" });
                }
                _dbContext.ServiceQuotations.Remove(serviceQuotation);
                _dbContext.SaveChanges();
                return Ok(new { Status = true, Message = "Service Quotation Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });

            }

        }

        [HttpGet]
        [Route("GetServiceQuotationByUserId")]
        public async Task<IActionResult> GetServiceQuotationByUserId(string UserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserId);
                if (user == null)
                {
                    return Ok(new { Status = true, Message = "User Not Found" });
                }
                var serviceQuotation = _dbContext.ServiceQuotations.Include(e => e.Service).Include(e => e.ServiceQuotationRequestStatus).Where(e => e.UserId == UserId).Select(c => new
                {
                    ServiceId = c.ServiceId,
                    ServiceQuotationId = c.ServiceQuotationId,
                    QuotationDate = c.QuotationDate,
                    StatusTitleAr = c.ServiceQuotationRequestStatus.StatusTitleAr,
                    StatusTitleEn = c.ServiceQuotationRequestStatus.StatusTitleEn,
                    ServiceTitleAr = c.Service.ServiceTitleAr,
                    ServiceTitleEn = c.Service.ServiceTitleEn,
                    Description = c.Description,

                }).ToList();

                return Ok(new { Status = true, serviceQuotation = serviceQuotation });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }


        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}




