using MGCap.Domain.ViewModels.ContractActivityLog;
using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractChildDetailViewModel
    {
        public ContractDetailViewModel BudgetDetail { get; set; }

        public IEnumerable<ContractItemGridViewModel> EstimatedRevenues { get; set; }

        public IEnumerable<ContractExpenseGridViewModel> EstimatedExpenses { get; set; }

        public IEnumerable<ContractNoteGridViewModel> Notes { get; set; }

        public IEnumerable<ContractActivityLogGridViewModel> ActivityLog { get; set; }

        public ContractChildDetailViewModel()
        {
            this.EstimatedRevenues = new HashSet<ContractItemGridViewModel>();
            this.EstimatedExpenses = new HashSet<ContractExpenseGridViewModel>();
            this.Notes = new HashSet<ContractNoteGridViewModel>();
            this.ActivityLog = new HashSet<ContractActivityLogGridViewModel>();
        }
    }
}
