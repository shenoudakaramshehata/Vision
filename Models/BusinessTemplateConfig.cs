using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class BusinessTemplateConfig
    {
        public BusinessTemplateConfig()
        {
            BusinessContents = new HashSet<BusinessContent>();
            BusinessTemplateOptions = new HashSet<BusinessTemplateOption>();
        }

        [Key]
        public int BusinessTemplateConfigId { get; set; }
        public int FieldTypeId { get; set; }
        [StringLength(150)]
        public string? BusinessTemplateFieldCaptionAr { get; set; }
        [StringLength(150)]
        public string? BusinessTemplateFieldCaptionEn { get; set; }
        public int? BusinessCategoryId { get; set; }
        public int? SortOrder { get; set; }
        public int? Step { get; set; }
		public bool HasChild { get; set; }
		public int ParentId { get; set; }
		public bool IsRequired { get; set; }
        public string? ValidationMessageAr { get; set; }
        public string? ValidationMessageEn { get; set; }
		public bool IsList { get; set; }
		[JsonIgnore]
        [ForeignKey("BusinessCategoryId")]
        public virtual BusinessCategory? BusinessCategories { get; set; }

        [ForeignKey("FieldTypeId")]
        public virtual FieldType? FieldType { get; set; }
        public virtual ICollection<BusinessContent> BusinessContents { get; set; }
        public virtual ICollection<BusinessTemplateOption>? BusinessTemplateOptions { get; set; }
    }
}
