using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ScheduleSettingSubCategory : AuditableEntity
    {        
        /// <summary>
        /// Name of the subcategory
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public int ScheduleSettingCategoryId { get; set; }

        [ForeignKey("ScheduleSettingCategoryId")]
        public ScheduleSettingCategory ScheduleSettingCategory { get; set; }
    }
}
