using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class ServiceTemplateConfig
    {
        public ServiceTemplateConfig()
        {
            ServiceContents = new HashSet<ServiceContent>();
            ServiceTemplateOptions = new HashSet<ServiceTemplateOption>();
        }

        [Key]
        public int ServiceTemplateConfigId { get; set; }
        public int FieldTypeId { get; set; }
        public string? ServiceTemplateFieldCaptionAr { get; set; }
        public string? ServiceTemplateFieldCaptionEn { get; set; }
        public int? SortOrder { get; set; }
        public bool IsRequired { get; set; }
        public string? ValidationMessageAr { get; set; }
        public string? ValidationMessageEn { get; set; }
        public long ServiceTypeId { get; set; }
        [JsonIgnore]
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType? ServiceType { get; set; }

        [ForeignKey("FieldTypeId")]
        public virtual FieldType? FieldType { get; set; }
        public virtual ICollection<ServiceContent> ServiceContents { get; set; }
        public virtual ICollection<ServiceTemplateOption>? ServiceTemplateOptions { get; set; }
    }
}
