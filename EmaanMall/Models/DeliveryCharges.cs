using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class DeliveryCharges
    {
        [Key]
        public int DeliveryChargesId { get; set; }
        public string DeliveryChargesProvince { get; set; }
        public string DeliveryChargesCity { get; set; }
        public int DeliveryChargesAmount { get; set; }
        public bool DeliveryChargesStatus { get; set; } = true;
        public DateTime DeliveryChargesDate { get; set; }
    }
}
