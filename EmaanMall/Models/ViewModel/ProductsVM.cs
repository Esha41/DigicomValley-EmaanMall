using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models.ViewModel
{
    public class ProductsVM
    {
      //  public Product products { get; set; }
      //  public ProductImage productImages { get; set; }
        public ProductDetail productDetails { get; set; }
       // public ProductCategory productCategorys { get; set; }
        public List<int> ProductCategoryIds { get; set; }
        public List<int> ProductSizeIds { get; set; }
        public int  ProductDetailId { get; set; }
        public List<ProductBundle> productPricesBundles { get; set; } = new List<ProductBundle>();
        public List<ProductImage> productImages { get; set; } = new List<ProductImage>(); 
        public List<ProductDetail> productDetailsList { get; set; } = new List<ProductDetail>();
        public List<ProductImage> productImagesList { get; set; } = new List<ProductImage>();
        public List<ProductCategory> productCategoriesList { get; set; } = new List<ProductCategory>();

        public ProductInquiry productInquiries { get; set; }
        public List<ProductInquiryRemarks> productInquiryRemarks { get; set; } = new List<ProductInquiryRemarks>();
        public List<ProductColors> ProductColorsList { get; set; } = new List<ProductColors>();
        public List<ProductSizes> ProductSizeList { get; set; } = new List<ProductSizes>();
        public string productName { get; set; }
    }
}
