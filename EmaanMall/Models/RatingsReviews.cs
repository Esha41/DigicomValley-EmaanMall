using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class RatingsReviews
    {
        [Key]
        public int RatingsReviewsId { get; set; }
        public int Value { get; set; }
        public string Reviews { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public long OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Orders { get; set; }
       /* public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customers { get; set; }*/

        /* [Display(Name = "UserProfile")]
         public int UserProfileId { get; set; }

         [ForeignKey("UserProfileId")]
         public virtual UserProfile UserProfile { get; set; }*/

    }
}
