using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class VendorBusiness
    {
        [Key]
        public int BusinessId { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Categories { get; set; }
        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual Vendor Vendors { get; set; }
    }
}
