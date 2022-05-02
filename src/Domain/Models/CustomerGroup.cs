﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CustomerGroup : AuditableCompanyEntity
    {
        /// <summary>
        /// Domestic, Export
        /// </summary>
        [MaxLength(80)]
        [Required]
        public string Name { get; set; }

    }
}
