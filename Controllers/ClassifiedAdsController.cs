using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;

namespace Vision.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ClassifiedAdsController : Controller
    {
        private CRMDBContext _context;

        public ClassifiedAdsController(CRMDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string parseFilterArray, DataSourceLoadOptions loadOptions) {
            //List<NewFilterAds> newFilterAds = JsonConvert.DeserializeObject<List<NewFilterAds>>(parseFilterArray);

            //newFilterAds.RemoveAll(filter => filter.value.Count == 0);

            var classifiedads = _context.ClassifiedAds.Include(e=>e.ClassifiedAdsCategory).Select(i => new {
                i.ClassifiedAdId,
                i.TitleAr,
                i.TitleEn,
                i.Price,
                i.MainPic,
                i.Description,
                i.PublishDate,
                i.UseId,
                i.Views,
                i.IsActive,
                i.CityId,
                i.AreaId,
                i.ClassifiedAdsCategoryId,
                i.ClassifiedAdsCategory
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ClassifiedAdId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(classifiedads, loadOptions));
        }

      
    }
}