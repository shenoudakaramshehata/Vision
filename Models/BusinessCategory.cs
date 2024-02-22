using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Vision.Models
{
    [Table("BusinessCategory")]
    [Index("BusinessCategoryParentId", Name = "IX_BusinessCategory_BusinessCategoryParentId")]
    public class BusinessCategory
    {
        
        public BusinessCategory()
        {
            //BusinessSubCategories = new HashSet<BusinessSubCategory>();
           
                BusinessTemplateConfigs = new HashSet<BusinessTemplateConfig>();
                ClassifiedBusiness = new HashSet<ClassifiedBusiness>();
            InverseBusinessCategoryParent = new HashSet<BusinessCategory>();

        }

        [Key]
        public int BusinessCategoryId { get; set; }
        [StringLength(50)]
        public string? BusinessCategoryTitleAr { get; set; }
        [StringLength(50)]
        public string? BusinessCategoryTitleEn { get; set; }
        public int? BusinessCategorySortOrder { get; set; }
       
        public string? BusinessCategoryCategoryPic { get; set; }
        public bool BusinessCategoryIsActive { get; set; }
        [StringLength(50)]
        public string? BusinessCategoryDescAr { get; set; }
        [StringLength(50)]
        public string? BusinessCategoryDescEn { get; set; }
        public int? BusinessCategoryParentId { get; set; }

        [ForeignKey(" BusinessCategoryParentId")]
        public virtual BusinessCategory BusinessCategoryParent { get; set; }

        [InverseProperty("BusinessCategoryParent")]
        public virtual ICollection<BusinessCategory> InverseBusinessCategoryParent { get; set; }

        [InverseProperty("BusinessCategories")]
        public virtual ICollection<BusinessTemplateConfig>? BusinessTemplateConfigs { get; set; }
        [InverseProperty("BusinessCategory")]

        public virtual ICollection<ClassifiedBusiness>? ClassifiedBusiness { get; set; }

        //public virtual ICollection<BusinessSubCategory> BusinessSubCategories { get; set; }
    }
}
