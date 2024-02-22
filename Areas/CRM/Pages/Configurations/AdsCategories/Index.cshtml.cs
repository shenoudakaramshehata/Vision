using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.AdsCategories
{
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public Models.ClassifiedAdsCategory ClassifiedAdsCategory { get; set; }


        public List<ClassifiedAdsCategory> ClassifiedAdsCategories = new List<ClassifiedAdsCategory>();

        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            ClassifiedAdsCategory = new ClassifiedAdsCategory();
        }
        public void OnGet()
        {
            ClassifiedAdsCategories = _context.ClassifiedAdsCategories.ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }


        public IActionResult OnGetSingleAdsCategoryForEdit(int ClassifiedAdsCategoryId)
        {
            var Result = _context.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == ClassifiedAdsCategoryId).FirstOrDefault();
            return new JsonResult(Result);

        }


        public async Task<IActionResult> OnPostEditAdsCategory(int ClassifiedAdsCategoryId, IFormFile Editfile)
        {
            var error = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter All the Required Data");

                return Redirect("/CRM/Configurations/AdsCategories/Index");

            }
            try
            {
                var model = _context.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == ClassifiedAdsCategoryId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Ads Category is not Existed");

                    return Redirect("/CRM/Configurations/AdsCategories/Index");
                }

                if (Editfile != null)
                {
                    if (model.ClassifiedAdsCategoryPic != null)
                    {
                        var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, model.ClassifiedAdsCategoryPic);
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }

                    string folder = "Images/Category/";
                    model.ClassifiedAdsCategoryPic = await UploadImage(folder, Editfile);
                }
                else
                {
                    model.ClassifiedAdsCategoryPic = ClassifiedAdsCategory.ClassifiedAdsCategoryPic;
                }
                model.ClassifiedAdsCategoryTitleAr = ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr;
                model.ClassifiedAdsCategoryTitleEn = ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn;

                model.ClassifiedAdsCategoryIsActive = ClassifiedAdsCategory.ClassifiedAdsCategoryIsActive;
                model.ClassifiedAdsCategorySortOrder = ClassifiedAdsCategory.ClassifiedAdsCategorySortOrder;
                model.ClassifiedAdsCategoryDescAr = ClassifiedAdsCategory.ClassifiedAdsCategoryDescAr;
                model.ClassifiedAdsCategoryDescEn = ClassifiedAdsCategory.ClassifiedAdsCategoryDescEn;


                var UpdatedADS = _context.ClassifiedAdsCategories.Attach(model);
                UpdatedADS.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Ads Category Edited successfully ");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("SomeThing went Wrong");

            }
            return Redirect("/CRM/Configurations/AdsCategories/Index");
        }


        //public IActionResult OnGetSingleSoundCategoryForView(int Id)
        //{
        //    var Result = _context.SoundCategories.Where(c => c.Id == Id).Include(c => c.SoundMaterials).Select(i => new
        //    {
        //        Id = i.Id,
        //        ArTitle = i.ArTitle,
        //        ENTitle = i.ENTitle,

        //        Image = i.Image,
        //        IsActive = i.IsActive,

        //    }).FirstOrDefault();

        //    return new JsonResult(Result);
        //}


        //public IActionResult OnGetSingleSoundCategoryForDelete(int Id)
        //{
        //    soundCategory = _context.SoundCategories.Where(c => c.Id == Id).FirstOrDefault();
        //    return new JsonResult(soundCategory);
        //}


        //public async Task<IActionResult> OnPostDeleteSoundCategory(int Id)
        //{
        //    try
        //    {
        //        SoundCategory soundCategoryObj = _context.SoundCategories.Where(e => e.Id == Id).FirstOrDefault();


        //        if (soundCategoryObj != null)
        //        {


        //            _context.SoundCategories.Remove(soundCategoryObj);
        //            await _context.SaveChangesAsync();
        //            _toastNotification.AddSuccessToastMessage("Sound Category Deleted successfully");
        //            var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "/" + soundCategory.Image);
        //            if (System.IO.File.Exists(ImagePath))
        //            {
        //                System.IO.File.Delete(ImagePath);
        //            }
        //        }
        //        else
        //        {
        //            _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
        //        }
        //    }
        //    catch (Exception)

        //    {
        //        _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
        //    }

        //    return Redirect("/Admin/Configurations/ManageSoundCategory/Index");
        //}


        //public async Task<IActionResult> OnPostAddSoundCategory(IFormFile file)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _toastNotification.AddErrorToastMessage("Please Enter All the Required Data");
        //        return Redirect("/Admin/Configurations/ManageSoundCategory/Index");
        //    }
        //    try
        //    {
        //        if (file != null)
        //        {
        //            string folder = "Images/SoundCategory/";
        //            soundCategory.Image = await UploadImage(folder, file);
        //        }

        //        _context.SoundCategories.Add(soundCategory);
        //        _context.SaveChanges();
        //        _toastNotification.AddSuccessToastMessage("Sound Category added successfully");

        //    }
        //    catch (Exception)
        //    {

        //        _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
        //    }
        //    return Redirect("/Admin/Configurations/ManageSoundCategory/Index");
        //}


        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
