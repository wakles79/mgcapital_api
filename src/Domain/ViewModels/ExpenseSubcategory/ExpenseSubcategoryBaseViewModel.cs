using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.ExpenseSubcategory
{
    public class ExpenseSubcategoryBaseViewModel : EntityViewModel
    {
        [MaxLength(64)]
        public string Name { get; set; }

        public int ExpenseTypeId { get; set; }

        public double Rate { get; set; }

        public ExpenseRateType RateType { get; set; }

        public string Periodicity { get; set; }

        public bool Status { get; set; }
    }
}
