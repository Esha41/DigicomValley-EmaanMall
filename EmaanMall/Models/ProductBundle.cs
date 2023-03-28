using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductBundle
    {
        [Key]
        public int ProductBundleId { get; set; }
        public int ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail productDetail { get; set; }
        public int ProductBundleQuantity { get; set; }
        public int ProductBundlePrice { get; set; }
        public string ProductBundleUnit { get; set; }
        public int? SizeId { get; set; }
        public bool ProductBundleStatus { get; set; } = true;
        public int? DiscountPrice { get; set; } 
    }
}
