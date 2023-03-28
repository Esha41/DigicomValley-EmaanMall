using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryIcon { get; set; }
        public string CategoryName { get; set; }
        public bool CategoryStatus { get; set; } = true;
        
    }
}
