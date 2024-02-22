using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Vision.Areas.CRM.Pages.Bussiness.ManageCategory
{
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        [BindProperty]
        public Category category { get; set; }
        public List<Category> categoriesList = new List<Category>();
        public Category categoryObj { get; set; }



        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            category = new Category();
            categoryObj = new Category();
        }
        public void OnGet()
        {
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            // categoriesList = _context.Categories.ToList();
        }
        public IActionResult OnGetSingleCategoryForEdit(int CategoryId)
        {
            category = _context.Categories.Where(c => c.CategoryId == CategoryId).FirstOrDefault();
            return new JsonResult(category);

        }
        public async Task<IActionResult> OnPostEditCategory(int CategoryId, IFormFile Editfile)
        {
            try
            {
                var model = _context.Categories.Where(c => c.CategoryId == CategoryId).FirstOrDefault();
                if (model == null)
                {
                    return RedirectToPage("/Bussiness/ManageCategory/Index");
                }


                if (Editfile != null)
                {
                    if (model.CategoryPic != null)
                    {
                        var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, model.CategoryPic);
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }

                    string folder = "Images/Category/";
                    model.CategoryPic = await UploadImage(folder, Editfile);
                }
                else
                {
                    model.CategoryPic = category.CategoryPic;
                }
                model.Description = category.Description;
                model.CategoryTitleAr = category.CategoryTitleEn;
                model.SortOrder = category.SortOrder;
                model.CategoryTitleEn = category.CategoryTitleEn;
                model.Tags = category.Tags;
                var UpdatedCategory = _context.Categories.Attach(model);
                UpdatedCategory.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Category Edited successfully");


            }
            catch (Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
               
            }
            return RedirectToPage("/Bussiness/ManageCategory/Index");


            //return new JsonResult(instance);
        }
        public IActionResult OnGetSingleCategoryForView(int CategoryId)
        {
            var Result = _context.Categories.Where(c => c.CategoryId == CategoryId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public IActionResult OnGetSingleCategoryForDelete(int CategoryId)
        {
            category = _context.Categories.Where(c => c.CategoryId == CategoryId).FirstOrDefault();
            return new JsonResult(category);
        }

        public async Task<IActionResult> OnPostDeleteCategory(int CategoryId)
        {
            try
            {
                Category CatObj = _context.Categories.Where(e => e.CategoryId == CategoryId).FirstOrDefault();


                if (CatObj != null)
                {


                    _context.Categories.Remove(CatObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Category Deleted successfully");
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Category/" + category.CategoryPic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                }
                else
                    return Redirect("../Error");
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return RedirectToPage("/Bussiness/ManageCategory/Index");
        }


        public async Task<IActionResult> OnPostAddCategory(IFormFile file)
        {
          
            try
            {
                if (file != null)
                {
                    string folder = "Images/Category/";
                    category.CategoryPic = await UploadImage(folder, file);
                }

                _context.Categories.Add(category);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Category  Added Successfully");

            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return RedirectToPage("/Bussiness/ManageCategory/Index");
        }
        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
