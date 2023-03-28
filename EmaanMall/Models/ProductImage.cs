using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }
        public int? ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail productDetail { get; set; }
        public string ProductImageName { get; set; }
        public bool ProductImageStatus { get; set; } = true;
    }
}
