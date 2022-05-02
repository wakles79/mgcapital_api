using System.ComponentModel.DataAnnotations;
using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.OfficeServiceType
{
    public class OfficeServiceTypeBaseViewModel : EntityViewModel
    {
        /// <summary>
        /// Name of office type
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Rate according to type
        /// </summary>		
        [Required]
        public double Rate { get; set; }

        /// <summary>
        /// Type of rate
        /// </summary>
        public ServiceRateType RateType { get; set; }

        /// <summary>
        /// Periodicity of the rate calculation
        /// <para>
        ///	Daily, Monthly, Yearly
        /// </para>
        /// </summary>
        [MaxLength(15)]
        public string Periodicity { get; set; }

        /// <summary>
        /// Status
        /// <para>
        /// false => disabled, true => enabled
        /// </para>
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// SupplyFactor
        /// </summary>	
        public double SupplyFactor { get; set; }
    }
}
