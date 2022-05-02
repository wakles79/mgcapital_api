using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.Expense;
using MGCap.Domain.ViewModels.Revenue;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractReportDetailViewModel : ContractBaseViewModel
    {
        public Guid Guid { get; set; }

        public string CustomerName { get; set; }

        public string BuildingName { get; set; }

        public IEnumerable<ContractItemGridViewModel> ContractItems { get; set; }

        public IEnumerable<RevenueGridViewModel> Revenues { get; set; }

        public IEnumerable<ExpenseGridViewModel> Expenses { get; set; }

        public IEnumerable<ContractExpenseGridViewModel> ContractExpenses { get; set; }

        public IEnumerable<ContractOfficeSpaceGridViewModel> OfficeSpaces { get; set; }

        public ContractReportDetailViewModel()
        {
            this.ContractItems = new HashSet<ContractItemGridViewModel>();
            this.ContractExpenses = new HashSet<ContractExpenseGridViewModel>();
            this.OfficeSpaces = new HashSet<ContractOfficeSpaceGridViewModel>();
        }
    }
}
