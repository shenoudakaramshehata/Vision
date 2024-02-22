using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ListsController : Controller
    {
        private CRMDBContext _context;

        public ListsController(CRMDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessTemplateOption>>> GetBusinessTemplateOptionList()
        {
            var data = await _context.BusinessTemplateOptions.Select(i => new
            {
                BusinessTemplateOptionId = i.BusinessTemplateOptionId,
                OptionEn = i.OptionEn,
                OptionAr = i.OptionAr,
            }).ToListAsync();


            return Ok(new { data });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassifiedAdsCategory>>> GetAdsCategory()
        {


            var data = await _context.ClassifiedAdsCategories.Select(i => new
            {
                ClassifiedAdsCategoryId = i.ClassifiedAdsCategoryId,
                ClassifiedAdsCategoryTitleAr = i.ClassifiedAdsCategoryTitleAr,
                ClassifiedAdsCategoryTitleEn = i.ClassifiedAdsCategoryTitleEn,
                ClassifiedAdsCategoryPic = i.ClassifiedAdsCategoryPic,
                ClassifiedAdsCategoryIsActive = i.ClassifiedAdsCategoryIsActive,
                ClassifiedAdsCategorySortOrder = i.ClassifiedAdsCategorySortOrder,
            }).ToListAsync();


            return Ok(new { data });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SoicialMidiaLink>>> GetSocialLinks()
        {
            var data = await _context.SoicialMidiaLinks.Select(i => new
            {
                Facebook = i.facebooklink,
                Instgram = i.Instgramlink,
                Twitter = i.TwitterLink,
                WhatsApp = i.WhatsApplink,
                LinkedIn = i.LinkedInlink,
                Youtube = i.YoutubeLink,
                SocialMediaLinkId = i.id,

            }).ToListAsync();


            return Ok(new { data });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetMessages()
        {
            var data = await _context.Contacts.Select(i => new
            {
                FullName = i.FullName,
                TransDate = i.SendingDate.Value.ToShortDateString(),
                Email = i.Email,
                ContactId = i.ContactId,
                Msg = i.Message

            }).ToListAsync();


            return Ok(new { data });
        }
       
    }
}