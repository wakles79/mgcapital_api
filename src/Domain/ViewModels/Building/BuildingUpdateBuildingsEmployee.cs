using MGCap.Domain.ViewModels.Employee;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingUpdateEmployeeBuildingsViewModel
    {
        public int EmployeeId { get; set; }

        public BuildingEmployeeType Type { get; set; }      
       
        public IEnumerable<int> BuildingsId { get; set; }
    }
}
