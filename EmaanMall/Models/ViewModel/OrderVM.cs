using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models.ViewModel
{
    public class OrderVM
    {
        public Order order { get; set; }
        public List<OrderProduct> orderProduct { get; set; }
        public List<Order> orders { get; set; }
        public List<Order> Allorders { get; set; }
        public List<ProductDetail> products { get; set; }
        public List<Category> categories { get; set; }
        public List<ProductDetail> productDetails { get; set; }

        public List<ProductCategory> productCategories { get; set; }
        public List<ProductCategory> AllproductCategories { get; set; }
        public List<ProductInquiry> productInquiries { get; set; }
        public List<PromoCodes> promocodeList { get; set; }
    }
}
