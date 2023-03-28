using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductInquiry
    {
        [Key]
        public int ProductInquiryId { get; set; }
        public int ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail productDetail { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer customer { get; set; }
        public int ProductInquiryQuantity { get; set; }
        public string ProductInquiryDescription { get; set; }
        public bool Status { get; set; } = true;
        public string ProductInquiryStatus { get; set; } 
        public DateTime ProductInquiryDate { get; set; }
    }
}
