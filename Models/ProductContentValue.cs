using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class ProductContentValue
    {
        public long ProductContentValueId { get; set; }
        public long ProductContentId { get; set; }
        public string? ContentValue { get; set; }

        public virtual ProductContent? ProductContent { get; set; }
    }
}
