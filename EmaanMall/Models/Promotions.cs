using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class Promotions
    {
        [Key]
        public int PromotionsId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; } = true;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
