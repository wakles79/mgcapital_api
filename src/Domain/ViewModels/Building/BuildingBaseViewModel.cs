using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MGCap.Domain.Models;

using MGCap.Domain.ViewModels.Employee;

namespace MGCap.Domain.ViewModels.Building
{
    public partial class AddressViewModel
    {
        public int? AddressId { get; set; }
        [MaxLength(80)]
        public string AddressLine1 { get; set; }

        [MaxLength(80)]
        public string AddressLine2 { get; set; }

        [MaxLength(80)]
        public string City { get; set; }

        [MaxLength(80)]
        public string State { get; set; }

        [MaxLength(80)]
        public string ZipCode { get; set; }

        [MaxLength(3)]
        public string CountryCode { get; set; }

        public string FullAddress { get; set; }

    }
    public class BuildingBaseViewModel : EntityViewModel
    {
        [MaxLength(128)]
        [Required]
        public string Name { get; set; }

        public int? CustomerId { get; set; }

        // public int? OperationsManagerId { get; set; }

        //public int? SupervisorId { get; set; }

        public bool IsActive { get; set; } = true;

        public string EmergencyPhone { get; set; }

        public string EmergencyPhoneExt { get; set; }

        public string EmergencyNotes { get; set; }

        public string Code { get; set; }

        #region Address Related Fields

        public AddressViewModel Address { get; set; } = new AddressViewModel();

        #endregion
                
    }
}
