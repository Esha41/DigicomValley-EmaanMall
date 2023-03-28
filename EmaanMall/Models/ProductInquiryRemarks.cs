using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductInquiryRemarks
    {
        [Key]
        public int ProductInquiryRemarksId { get; set; }
        public int ProductInquiryId { get; set; }
        [ForeignKey("ProductInquiryId")]
        public virtual ProductInquiry ProductInquiries { get; set; }
        public string AdminRemarks { get; set; }
        public bool Status { get; set; } = true;
        public DateTime Date { get; set; }
    }
}
