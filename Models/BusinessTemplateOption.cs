using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class BusinessTemplateOption
    {
        [Key]
        public int BusinessTemplateOptionId { get; set; }
        public int? BusinessTemplateConfigId { get; set; }
        [StringLength(150)]
        public string? OptionAr { get; set; }
        [StringLength(150)]
        public string? OptionEn { get; set; }
		public int ParentId { get; set; }

		[ForeignKey("BusinessTemplateConfigId")]
        [InverseProperty("BusinessTemplateOptions")]
        public virtual BusinessTemplateConfig? BusinessTemplateConfig { get; set; }
    }
}
