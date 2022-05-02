using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.Service
{
    public class ServiceBaseViewModel : EntityViewModel
    {
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        ///     Could be hours, square feet etc
        /// </summary>
        [MaxLength(10)]
        public string UnitFactor { get; set; }
        public double UnitPrice { get; set; }
        /// <summary>
        ///     The minimum total that a service line could have
        /// </summary>
        public double MinPrice { get; set; }
    }
}
