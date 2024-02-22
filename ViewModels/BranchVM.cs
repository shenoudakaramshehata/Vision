using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.ViewModel
{
    public class BranchVM
    {
        public int BranchId { get; set; }
        public string Title { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int AddListingId { get; set; }
    }
}
