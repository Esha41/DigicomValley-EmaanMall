using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmaanMall.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationDescription { get; set; }
        public string NotificationImageName { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool NotificationStatus { get; set; } = true;
        public bool IsSeen { get; set; }
        public bool IsOpen { get; set; }
    }
}
