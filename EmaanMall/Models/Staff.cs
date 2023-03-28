using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
    }
}
