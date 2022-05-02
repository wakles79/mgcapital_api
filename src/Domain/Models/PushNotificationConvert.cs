using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    [Dapper.Table("PushNotificationConverts")]
    public class PushNotificationConvert : BaseEntity
    {
        [Key]
        [Required]
        public int PushNotificationId { get; set; }

        [Key]
        [Required]
        public int EmployeeId { get; set; }

        [Dapper.IgnoreInsert]
        [ForeignKey("PushNotificationId")]
        public PushNotification PushNotification { get; set; }

        [Dapper.IgnoreInsert]
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
