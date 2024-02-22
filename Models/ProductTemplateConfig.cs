using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class ProductTemplateConfig
    {
        public ProductTemplateConfig()
        {
            ProductContents = new HashSet<ProductContent>();
            ProductTemplateOptions = new HashSet<ProductTemplateOption>();
        }

        [Key]
        public int ProductTemplateConfigId { get; set; }
        public int FieldTypeId { get; set; }
        [StringLength(150)]
        public string? ProductTemplateFieldCaptionAr { get; set; }
        [StringLength(150)]
        public string? ProductTemplateFieldCaptionEn { get; set; }
        public int? SortOrder { get; set; }
        public bool IsRequired { get; set; }
        public string? ValidationMessageAr { get; set; }
        public string? ValidationMessageEn { get; set; }
        public long ProductTypeId { get; set; }
		public bool IsList { get; set; }
		public bool HasChild { get; set; }
		public int ParentId { get; set; }
		public int Step { get; set; }
		[JsonIgnore]
        [ForeignKey("ProductTypeId")]
        public virtual ProductType? ProductType { get; set; }

        [ForeignKey("FieldTypeId")]
        public virtual FieldType? FieldType { get; set; }
        public virtual ICollection<ProductContent> ProductContents { get; set; }
        public virtual ICollection<ProductTemplateOption>? ProductTemplateOptions { get; set; }
    }
}
