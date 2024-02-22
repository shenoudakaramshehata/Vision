using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class ProductTemplateOption
    {
        [Key]
        public int ProductTemplateOptionId { get; set; }
        public int? ProductTemplateConfigId { get; set; }
        [StringLength(150)]
        public string? OptionAr { get; set; }
        [StringLength(150)]
        public string? OptionEn { get; set; }
		public int ParentId { get; set; }

		[ForeignKey("ProductTemplateConfigId")]
        [InverseProperty("ProductTemplateOptions")]
        public virtual ProductTemplateConfig? ProductTemplateConfig { get; set; }
    }
}
