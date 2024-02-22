using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vision.Models
{
    public partial class BusinessSubCategory
    {
        
        [Key]
        public int BusinessSubCategoryId { get; set; }
        public string? BusinessSubCategoryTitleAr { get; set; }
        public string BusinessSubCategoryTitleEn { get; set; }
        public string? BusinessSubCategoryPic { get; set; }
        public int? SortOrder { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public virtual BusinessCategory BusinessCategory { get; set; }
        public int BusinessCategoryId { get; set; }
       

    }
}
