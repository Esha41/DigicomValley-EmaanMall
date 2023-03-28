using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class CustomerPromoCodeLog
    {
        [Key]
        public int CustomerPromoCodeLogId { get; set; }
        public int PromoCodesId { get; set; }
        [ForeignKey("PromoCodesId")]
        public virtual PromoCodes PromoCode { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
