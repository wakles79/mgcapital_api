// EmployeeWorkOrderViewModel.cs
//
// Created by Daileny Hernandez <daileny@axzes.com>
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
    public class EmployeeWorkOrderViewModel : EmployeeListBoxViewModel
    {
        public string Email { get; set; }

        public WorkOrderEmployeeType Type { get; set; }
    }
}
