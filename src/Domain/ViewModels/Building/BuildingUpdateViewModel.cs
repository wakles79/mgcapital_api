using MGCap.Domain.ViewModels.Employee;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.ViewModels.Employee;

namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingUpdateViewModel : BuildingBaseViewModel
    {
        //HACK: Field temporary for the creation of a contract, it is necessary in order that the building can be available 
        // public int CustomerId { get; set; }
        
        public IEnumerable<EmployeeBuildingViewModel> Employees { get; set; }
    }
}
