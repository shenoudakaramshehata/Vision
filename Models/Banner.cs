using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class Banner
    {
        public int BannerId { get; set; }
        public string BannerPic { get; set; }
        public string LargePic { get; set; }
        public string EntityId { get; set; }
        public int EntityTypeId { get; set; }
        public virtual EntityType EntityType { get; set; }
        public bool BannerIsActive { get; set; }
        public int BannerOrderIndex { get; set; }
    }
}
