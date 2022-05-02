using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Building
{
	public class BuildingUpdateSharedBuildingsEmployeeViewModel
	{
		public int EmployeeId { get; set; }

		public int BuildingId { get; set; }

		public BuildingEmployeeType Type { get; set; }

		public bool IsShared { get; set; }
	}
}
