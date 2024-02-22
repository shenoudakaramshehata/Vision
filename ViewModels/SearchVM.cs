using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.ViewModel
{
    public class SearchVM
    {
        public string id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string image { get; set; }
        public string CategoryEn { get; set; }
        public string? description { get; set; }
        public string CategoryAr { get; set; }
        public DateTime PublishData { get; set; }
        public double Price { get; set; }
        public int Type { get; set; }
    }
}
