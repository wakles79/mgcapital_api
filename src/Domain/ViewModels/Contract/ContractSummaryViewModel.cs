using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractSummaryViewModel
    {
        public int ID { get; set; }

        public string ContractNumber { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public int TotalEstimatedRevenue { get; set; }

        public int TotalEstimatedExpenses { get; set; }

        public int TotalRealRevenue { get; set; }

        public int TotalRealExpenses { get; set; }
    }
}
