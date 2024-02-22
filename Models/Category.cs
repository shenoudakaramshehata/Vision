using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryTitleAr { get; set; }
        public string CategoryTitleEn { get; set; }
        public string? CategoryPic { get; set; }
        public int? SortOrder{ get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }

    }
}
