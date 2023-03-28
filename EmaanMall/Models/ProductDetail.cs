using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class ProductDetail
    {
        [Key]
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public string ProductDetailModelNo { get; set; }
        public bool FeatureProduct { get; set; }
        public bool BestSell { get; set; }
        public bool TopRated { get; set; }
        public bool OutOfStock { get; set; }
        public int ProductDetailUnitPrice { get; set; } //min price of product
        public int ProductDetailTotalPrice { get; set; }  
        public string ProductDetailUnit { get; set; }
        public string ProductDetailDescription { get; set; }
        public string GuaranteePolicy { get; set; }
       // public string Wa { get; set; }
        public int? DiscountPrice { get; set; }
        public Guid? ReferenceUserId { get; set; }
        public float ProductDetailRating { get; set; }
        public int ProductDetailReviews { get; set; }
        public bool ProductDetailStatus { get; set; } = true;
        public DateTime ProductDetailDate { get; set; } = DateTime.Now;
    }
}
