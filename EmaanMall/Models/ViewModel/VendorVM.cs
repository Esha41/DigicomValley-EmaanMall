using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models.ViewModel
{
    public class VendorVM
    {
        public Vendor vendors { get; set; }
        public List<int> ProductCategoryIds { get; set; } = new List<int>();
    }
}
