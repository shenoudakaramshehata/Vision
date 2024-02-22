using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Vision.Models
{
    public class ServiceTemplateOption
    {
        [Key]
        public int ServiceTemplateOptionId { get; set; }
        public int? ServiceTemplateConfigId { get; set; }
        public string? OptionAr { get; set; }
        public string? OptionEn { get; set; }

        [ForeignKey("ServiceTemplateConfigId")]
        [InverseProperty("ServiceTemplateOptions")]
        public virtual ServiceTemplateConfig? ServiceTemplateConfig { get; set; }
    }
}
