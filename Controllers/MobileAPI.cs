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
using System.Net;
using Microsoft.AspNetCore.Components.RenderTree;
using Email;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Vision.Controllers
{
    [Route("api/[controller]")]
    public class MobileAPIController : ControllerBase
    {
        private readonly CRMDBContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly Email.IEmailSender _emailSender;
        public ApplicationDbContext _applicationDbContext { get; set; }
        public HttpClient httpClient { get; set; }

        public MobileAPIController(CRMDBContext dbContext, IWebHostEnvironment hostEnvironment, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, Email.IEmailSender emailSender, ApplicationDbContext applicationDbContext)
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

        [HttpGet]
        [Route("GetFullConfigurationByCategoryId")]
        public IActionResult GetFullConfigurationByCategoryId(int ClassifiedAdsCategoryId)
        {
            try
            {
                var Data = _dbContext.AdTemplateConfigs.Where(c => c.ClassifiedAdsCategoryId == ClassifiedAdsCategoryId).OrderBy(e => e.SortOrder).AsNoTracking();
                var lookups = _dbContext.AdTemplateOptions.Where(c => c.AdTemplateConfig.ClassifiedAdsCategoryId== ClassifiedAdsCategoryId).AsNoTracking();
                if (Data == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(new { data = Data, Lookups = lookups }));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetPostConfigurationByCategoryId")]
        public IActionResult GetPostConfigurationByCategoryId(int ClassifiedAdsCategoryId)
        {
            try
            {
                var Data = _dbContext.AdTemplateConfigs.Where(c => c.ClassifiedAdsCategoryId == ClassifiedAdsCategoryId).OrderBy(e => e.SortOrder).AsNoTracking();
                if (Data == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(new { data = Data }));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetLookupOptionByConfigId")]
        public IActionResult GetLookupOptionByConfigId(int AdTemplateConfigId)
        {
            try
            {
                var Data = _dbContext.AdTemplateOptions.Where(c => c.AdTemplateConfigId == AdTemplateConfigId).OrderBy(e => e.SortOrder).AsNoTracking();
                if (Data == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(new { data = Data }));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetChildLookupOptionByParentId")]
        public IActionResult GetChildLookupOptionByParentId(int ParentId)
        {
            try
            {
                var Data = _dbContext.AdTemplateOptions.Where(c => c.ParentId == ParentId).OrderBy(e => e.SortOrder).AsNoTracking();
                if (Data == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(new { data = Data }));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}
