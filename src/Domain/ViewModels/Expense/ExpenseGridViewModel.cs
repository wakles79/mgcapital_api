using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Expense
{
    public class ExpenseGridViewModel : ExpenseBaseViewModel, IGridViewModel
    {
        public bool IsDirect { get; set; }

        public string BuildingName { get; set; }

        public string CustomerName { get; set; }

        public string ContractNumber { get; set; }

        public string TypeName => this.Type.ToString().SplitCamelCase();

        public int EpochDate => this.Date.ToEpoch();

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
