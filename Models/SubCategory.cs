using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryID { get; set; }
        public string? SubCategoryTitleAr { get; set; }
        public string SubCategoryTitleEn { get; set; }
        public string? SubCategoryPic { get; set; }
        public int? SortOrder { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }
    }
}
