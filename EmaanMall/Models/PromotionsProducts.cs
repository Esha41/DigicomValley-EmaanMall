using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class PromotionsProducts
    {
        [Key]
        public int Id { get; set; }
        public int PromotionsId { get; set; }
        [ForeignKey("PromotionsId")]
        public virtual Promotions promotions { get; set; }
        public int ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail productDetail { get; set; }
        public bool Status { get; set; } = true;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
