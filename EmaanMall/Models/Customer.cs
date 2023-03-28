using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }

        public string Image { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerGender { get; set; }
        public DateTime CustomerDateOfBirth { get; set; }
        public string CustomerPassword { get; set; }
        public string CustomerCNIC { get; set; }
        public bool CustomerStatus { get; set; } = true;
        public DateTime CustomerDate { get; set; }

        public string CustomerAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FCMToken { get; set; }
    }
}
