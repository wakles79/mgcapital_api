using MGCap.Domain.ViewModels.Expense;
using MGCap.Domain.ViewModels.Revenue;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractTrackingDetailViewModel : ContractBaseViewModel
    {
        public string BuildingName { get; set; }

        public string CustomerName { get; set; }

        public IEnumerable<RevenueGridViewModel> Revenues { get; set; }

        public IEnumerable<ExpenseGridViewModel> Expenses { get; set; }

        public ContractTrackingDetailViewModel()
        {
            this.Revenues = new HashSet<RevenueGridViewModel>();
            this.Expenses = new HashSet<ExpenseGridViewModel>();
        }
    }
}
