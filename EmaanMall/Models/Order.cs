using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class Order
    {
        [Key]
        public long OrderId { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public bool isCustomerReviewed { get; set; } = false;
        public double OrderSubTotal { get; set; }
        public double OrderTotalPrice { get; set; }
        public int OrderDeliveryCharges { get; set; }    //??
        public float OrderDiscountPrice { get; set; }  //
        public float OrderGST { get; set; }  //
        public string OrderPaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderShippingAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string OrderShippingProvince { get; set; }
        public string OrderShippingCity { get; set; }
        public string OrderRecipentName { get; set; }
        public string OrderRecipentPhone { get; set; }
        public string OrderRecipentEmail { get; set; }
        public string OrderStatus { get; set; }
        public bool Status { get; set; } = true;
        public string OrderNo { get; set; }
        public long OrderReceiveAmount { get; set; }  //
        public long OrderRemainingAmount { get; set; }  // 
        public string Description { get; set; }
    }
}
