using MGCap.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Office
{
    public class OfficeBaseViewModel : EntityViewModel
    {
        [MaxLength(128)]
        public string Name { get; set; }

        public double SquareFoot { get; set; }

        public bool IsActive { get; set; }
    }
}
