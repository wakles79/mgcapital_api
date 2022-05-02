using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ScheduleSettingCategory : AuditableCompanyEntity
    {
        /// <summary>
        /// Category of the Schedule type.
        /// Title
        /// Stone
        /// Carpet
        /// Power_Wash
        /// Other_Music
        /// </summary>
        [Required]
        public ScheduleCategoryType ScheduleCategoryType { get; set; }

        [Required]
        public bool Status { get; set; }

        /// <summary>
        /// Name of the subcategory
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the expense type
        /// </summary>
        [MaxLength(128)]
        public string Description { get; set; }

        public int? Color { get; set; }
    }
}
