// EmployeeBuildingViewModel.cs
//
// Created by Saimel Saez <saimel@axzes.com>
//
// ---------------------------------------------------------------------------------
// This source is Copyright Axzes and MAY NOT be copied, reproduced, published,
// distributed or transmitted to or stored in any manner without prior written
// consent from Axzes (http://www.axzes.com).
// ---------------------------------------------------------------------------------

using System;
using MGCap.Domain.Enums;

namespace MGCap.Domain.ViewModels.Employee
{
    public class EmployeeBuildingViewModel : EmployeeListBoxViewModel
    {
        public string Email { get; set; }

        public BuildingEmployeeType Type { get; set; }

        public string Phone { get; set; }

        public int BuildingId { get; set; }
    }
}
