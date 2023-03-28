using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class OrderProduct
    {
        [Key]
        public long OrderProductId { get; set; }
        public long OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        public int ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail productDetail { get; set; }
        public int OrderProductQuantity { get; set; } //ye b sir daingy na..qk agr ni dety to hmy kese pata lagy ga k knsa bundle select krna
        public int OrderProductPrice { get; set; }
        public string OrderProductUnit { get; set; }
        public int ProductSizesId { get; set; }
        public int ProductColorsId { get; set; }
    }
}
