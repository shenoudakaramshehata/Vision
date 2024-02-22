using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using Vision.Data;
using Vision.Models;
using Vision.Services;


namespace Vision.Pages
{
    public class DisplayDataModel : PageModel
    {
        private readonly CRMDBContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRazorPartialToStringRenderer _renderer;
        
        public List<AdContent> lstColumns { get; set; }
       

        
        public string FormTemplate { get; set; }
        public DisplayDataModel(IRazorPartialToStringRenderer renderer, CRMDBContext Context, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _renderer = renderer;
         

        }
        public async void OnGet()
        {
            var classifiedAds = _context.ClassifiedAds.Where(e => e.ClassifiedAdId == 105).Include(e=>e.AdContents).ThenInclude(e=>e.AdContentValues).FirstOrDefault();

            if (classifiedAds != null)
            {
                lstColumns = _context.AdContents.Include(c => c.AdTemplateConfig).Include(c => c.AdContentValues).Where(c => c.ClassifiedAdId == classifiedAds.ClassifiedAdId).ToList();
            }

            FormTemplate = " <div class=\"container\">\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##973##\r\n</div>\r\n<div class=\"col-sm\">\r\n##974##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##975##\r\n</div>\r\n<div class=\"col-sm\">\r\n##976##\r\n</div>\r\n\r\n</div>\r\n<div class=\"row\">\r\n\r\n<div class=\"col-sm\">\r\n##977##\r\n</div>\r\n<div class=\"col-sm\">\r\n##978##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##979##\r\n</div>\r\n<div class=\"col-sm\">\r\n##980##\r\n</div>\r\n\r\n</div>\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##981##\r\n</div>\r\n<div class=\"col-sm\">\r\n##982##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n\r\n<div class=\"col-sm\">\r\n##983##\r\n</div>\r\n<div class=\"col-sm\">\r\n##984##\r\n</div>\r\n</div>\r\n</div>";

            foreach (var col in classifiedAds.AdContents)
            {
                var AdTemplateConfig = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault();
                switch (col.AdTemplateConfig.FieldTypeId)
                {
                    case 1:
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                        break;
                    case 2:
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                        break;
                    case 3:
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowLookup", col));
                        break;
                    case 4:
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                        break;
                    case 5:
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                        break;
                    //case "CheckBoxGroup":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowLargeText.cshtml", col));
                    //    break;
                    //case 7:
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdContentId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                    //    break;
                    //case 8:
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdContentId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                    //    break;
                    //case 9:
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdContentId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                    //    break;
                    //case 10:
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdContentId), await _renderer.RenderPartialToStringAsync("ComShowLargeText", col));
                    //    break;
                    case 14:
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowMap", col));
                        break;
                    //case "Image":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayImage", col));
                    //    break;
                    //case "Images":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayImages", col));
                    //    break;
                    //case "Video":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayVideo", col));
                    //    break;

                    //case "Videos":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayVideos", col));
                    //    break;
                    ////case "Media":
                    ////    Html.Partial("~/Pages/Shared/Components/InputForm/ComDisplayMedia.cshtml", col);
                    ////    break;
                    //case "RadioGroup":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayRadioGroup", col));
                    //    break;
                    //case "Map (Location)":
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayMap", col));
                    //    break;
                    default:
                        break;
                }
            }

        }
    }
}
