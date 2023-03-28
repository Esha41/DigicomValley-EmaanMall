using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductColors
    {
        [Key]
        public int ProductColorsId { get; set; }
        public int ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail ProductDetails { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}
