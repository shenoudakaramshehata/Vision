using System.ComponentModel.DataAnnotations.Schema;
using Vision.ViewModels;

namespace Vision.Models
{
    public class ProductCategory
    {
        
        public long ProductCategoryId { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? Pic { get; set; }
        public int? SortOrder { get; set; }
        public bool? Isactive { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
