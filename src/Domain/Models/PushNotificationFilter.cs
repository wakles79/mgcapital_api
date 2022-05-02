using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    [Dapper.Table("PushNotificationFilters")]
    public class PushNotificationFilter : Entity
    {
        public int PushNotificationId { get; set; }

        [IgnoreInsert]
        [ForeignKey("PushNotificationId")]
        public PushNotification PushNotification { get; set; }

        [MaxLength(256)]
        public string Key { get; set; }

        [MaxLength(256)]
        public string Field { get; set; }

        [MaxLength(256)]
        public string Value { get; set; }

        [MaxLength(3)]
        public string Relation { get; set; }
    }
}
