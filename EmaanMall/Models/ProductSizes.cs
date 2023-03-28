using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductSizes
    {
        [Key]
        public int ProductSizesId { get; set; }
        public int ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail ProductDetails { get; set; }
        public string Size { get; set; }
        public int TotalQuantity { get; set; }
    }
}
