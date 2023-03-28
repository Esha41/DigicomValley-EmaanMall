using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models.ViewModel
{
    public class promotionsVM
    {
        public Promotions promotions { get; set; }
        public PromotionsProducts promotionsProducts { get; set; }
        public List<Promotions> promotionsList { get; set; } = new List<Promotions>();
        public List<PromotionsProducts> proProdList { get; set; } = new List<PromotionsProducts>();
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
